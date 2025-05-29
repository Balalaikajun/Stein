<template>
  <div class="filter-item">
    <FilterButton
        ref="btnRef"
        :title="filter.title ||selectedLabel "
        @toggle="toggle"
        :isOpen="isOpen"
        :hasValue="hasValue"
        :loading="loading"
    />

    <FilterModal
        v-if="isOpen"
        :visible="isOpen"
        :anchorRect="anchorRect"
        @apply="apply"
        @cancel="close"
    >
      <template #header>
        <input
            v-model.lazy="search"
            @change="onSearchInput"
            placeholder="Поиск..."
            class="filter-input"
            :disabled="loading"
        />
      </template>

      <template #body>
        <div class="filter-modal-content" ref="listContainer" @scroll="onScroll">
          <div v-if="loading && !options.length" class="loading-state">
            <span class="loader"></span>
            <span>Загрузка...</span>
          </div>

          <ul v-else class="options-list">
            <li
                v-for="opt in options"
                :key="uniqueKey(opt)"
                class="options-list__item"
            >
              <label class="radio-option">
                <input
                    type="radio"
                    :name="filter.id"
                    :value="opt.value"
                    v-model="localValue"
                />
                {{ opt.label }}
              </label>
            </li>

            <li v-if="!loading && !options.length" class="empty-state">
              Ничего не найдено
            </li>
            <li v-if="loading && options.length" class="loading-more">
              Загрузка...
            </li>
          </ul>
        </div>
      </template>
    </FilterModal>
  </div>
</template>

<script setup>
import { ref, computed, nextTick, watch } from 'vue'
import axios from '@/api/api.js'
import { debounce, isEqual } from 'lodash-es'
import FilterButton from '@/components/Filters/FilterButton.vue'
import FilterModal from '@/components/Filters/FilterModal.vue'

const props = defineProps({
  filter: { type: Object, required: true },
  modelValue: { type: [String, Number, Boolean, Object], default: null },
  activeFilters: { type: Object, default: () => ({}) }
})
const emit = defineEmits(['update:modelValue'])

// State
const isOpen = ref(false)
const search = ref('')
const options = ref([])
const localValue = ref(props.modelValue)
const loading = ref(false)
const skip = ref(0)
const hasMore = ref(true)
const btnRef = ref(null)
const anchorRect = ref(null)
const listContainer = ref(null)

// Config
const isStatic = computed(() => Array.isArray(props.filter.staticOptions))
const take = props.filter.params?.take || props.filter.take || 50
const debounceMs = props.filter.debounceMs ?? 300
const allowDeselect = computed(() => props.filter.allowDeselect ?? false)

// Computed labels and state
const hasValue = computed(() => localValue.value !== null)
const selectedLabel = computed(() => {
  const found = options.value.find(o => isEqual(o.value, localValue.value))
  return found ? found.label : ''
})

// Helpers
function uniqueKey (opt) {
  return typeof opt.value === 'object' ? JSON.stringify(opt.value) : opt.value
}

function buildRequestBody () {
  const { params = {}, paramKeys = {}, dependentParams = {} } = props.filter
  const body = {
    [paramKeys.skip || 'skip']: skip.value,
    [paramKeys.take || 'take']: take,
    [paramKeys.search || 'searchText']: search.value,
    ...params
  }
  Object.entries(dependentParams).forEach(([fid, apiParam]) => {
    const val = props.activeFilters[fid]
    if (val?.length > 0) body[apiParam] = val
  })
  return body
}

// Load options (static or via API)
async function loadOptions () {
  if (loading.value) return
  loading.value = true

  if (isStatic.value) {
    const all = props.filter.staticOptions
    options.value = search.value
        ? all.filter(o => o.label.toLowerCase().includes(search.value.toLowerCase()))
        : [...all]
    loading.value = false
    hasMore.value = false
    return
  }

  if (!hasMore.value) {
    loading.value = false
    return
  }

  try {
    const body = buildRequestBody()
    const { data } = await axios.post(
        `${props.filter.apiEndpoint}`,
        body
    )
    const items = data.items || []
    options.value.push(...items.map(props.filter.mapOption))
    hasMore.value = data.hasMore
    skip.value += items.length
  } catch (err) {
    console.error('Error loading options:', err)
  } finally {
    loading.value = false
  }
}

function resetLoad () {
  options.value = []
  skip.value = 0
  hasMore.value = true
}

// Handlers
function toggle () {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    localValue.value = props.modelValue
    nextTick(() => {
      anchorRect.value = btnRef.value.$el.getBoundingClientRect()
      resetLoad()
      loadOptions()
    })
  }
}

function apply () {
  emit('update:modelValue', localValue.value)
  close()
}

function close () {
  isOpen.value = false
  search.value = ''
}

const onSearchInput = debounce(() => {
  resetLoad()
  loadOptions()
}, debounceMs)

function onScroll () {
  const c = listContainer.value
  if (!isStatic.value && hasMore.value && !loading.value && c.scrollHeight - c.scrollTop <= c.clientHeight + 50) {
    loadOptions()
  }
}

// Initial load
resetLoad()
loadOptions()

// Watchers
watch(() => props.modelValue, v => { localValue.value = v })
watch(
    () => props.activeFilters,
    (nv, ov) => {
      if (!props.filter.dependentParams) return
      const keys = Object.keys(props.filter.dependentParams)
      if (keys.some(k => JSON.stringify(nv[k]) !== JSON.stringify(ov[k]))) {
        if (isOpen.value) {
          resetLoad()
          loadOptions()
        }
      }
    },
    { deep: true }
)
</script>

<style scoped>
.filter-modal-content {
  max-height: 400px;
  overflow-y: auto;
  padding: 8px;
}

.filter-input {
  width: 100%;
  padding: 8px 12px;
  margin-bottom: 12px;
  border: 1px solid var(--secondary-background-color);
  border-radius: var(--border-radius);
  transition: border-color 0.2s;
}

.filter-input:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(91, 0, 225, 0.1);
}

.filter-input:disabled {
  background-color: var(--secondary-background-color);
  cursor: not-allowed;
}

.options-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.options-list__item {
  padding: 4px 0;
}

.radio-option {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px;
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: background-color 0.2s;
}

.radio-option:hover {
  background-color: var(--hover-color);
}

input[type="radio"] {
  width: 16px;
  height: 16px;
  accent-color: var(--primary-color);
}

.empty-state,
.loading-more {
  padding: 12px;
  text-align: center;
  color: var(--secondary-text-color);
}

.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 16px;
  color: var(--secondary-text-color);
}

.loader {
  display: inline-block;
  width: 18px;
  height: 18px;
  border: 2px solid var(--secondary-background-color);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
