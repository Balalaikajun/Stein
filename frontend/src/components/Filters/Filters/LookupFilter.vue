<template>
  <div class="filter-item">
    <FilterButton
        ref="btnRef"
        :title="filter.title"
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
              <label>
                <input
                    type="checkbox"
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
import { computed, nextTick, onMounted, ref, toRaw, watch } from 'vue'
import axios from 'axios'
import { debounce } from 'lodash-es'
import FilterButton from '@/components/Filters/FilterButton.vue'
import FilterModal from '@/components/Filters/FilterModal.vue'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

const props = defineProps({
  filter: { type: Object, required: true },
  modelValue: { type: Array, default: () => [] },
  activeFilters: { type: Object, default: () => ({}) }
})
const emit = defineEmits(['update:modelValue'])

// Reactive state
const isOpen = ref(false)
const search = ref('')
const options = ref([])
const localValue = ref([...props.modelValue])
const loading = ref(false)
const skip = ref(0)
const hasMore = ref(true)
const btnRef = ref(null)
const anchorRect = ref(null)
const listContainer = ref(null)

// Flags
const isStatic = computed(() => Array.isArray(props.filter.staticOptions))
const take = props.filter.params?.take || props.filter.take || 50
const debounceMs = props.filter.debounceMs ?? 300

// Computed
const hasValue = computed(() => localValue.value.length > 0)

// Helpers
function uniqueKey(opt) {
  return typeof opt.value === 'object' ? JSON.stringify(opt.value) : opt.value
}

function buildRequestBody() {
  const { params = {}, paramKeys = {}, dependentParams = {} } = props.filter
  const body = {
    [paramKeys.skip || 'skip']: skip.value,
    [paramKeys.take || 'take']: take,
    [paramKeys.search || 'searchText']: search.value,
    ...params
  }
  Object.entries(dependentParams).forEach(([filterId, apiParam]) => {
    const value = props.activeFilters[filterId]
    if (value?.length > 0) {
      body[apiParam] = toRaw(value)
    }
  })
  return body
}

async function loadOptions() {
  if (loading.value) return
  loading.value = true

  // Статичные опции: фильтрация на клиенте
  if (isStatic.value) {
    const all = props.filter.staticOptions
    options.value = search.value
        ? all.filter(opt =>
            opt.label.toLowerCase().includes(search.value.toLowerCase())
        )
        : [...all]
    // Soft reset выбранных
    const valid = options.value.map(o => o.value)
    const filtered = localValue.value.filter(v => valid.includes(v))
    if (filtered.length !== localValue.value.length) {
      localValue.value = filtered
      emit('update:modelValue', [...filtered])
    }
    loading.value = false
    hasMore.value = false
    return
  }

  // Динамические: запрос к API
  if (!hasMore.value) {
    loading.value = false
    return
  }

  try {
    const body = buildRequestBody()
    const { data } = await axios.post(
        `${BACKEND_API_HOST}${props.filter.apiEndpoint}`,
        body
    )
    const items = data.items || []
    options.value.push(...items.map(props.filter.mapOption))
    hasMore.value = data.hasMore
    skip.value += items.length

    // Soft reset
    if (props.filter.softReset) {
      const valid = options.value.map(o => o.value)
      const filtered = localValue.value.filter(v => valid.includes(v))
      if (filtered.length !== localValue.value.length) {
        localValue.value = filtered
        emit('update:modelValue', [...filtered])
      }
    }
  } catch (err) {
    console.error('Ошибка загрузки опций:', err)
  } finally {
    loading.value = false
  }
}

function resetLoad() {
  options.value = []
  skip.value = 0
  hasMore.value = true
}

// Handlers
const toggle = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    nextTick(() => {
      anchorRect.value = btnRef.value.$el.getBoundingClientRect()
      search.value = ''
      resetLoad()
      loadOptions()
    })
  }
}

const apply = () => {
  emit('update:modelValue', [...localValue.value])
  close()
}

const close = () => {
  isOpen.value = false
  search.value = ''
  resetLoad()
  localValue.value = [...props.modelValue]
}

const onSearchInput = debounce(() => {
  resetLoad()
  loadOptions()
}, debounceMs)

const onScroll = () => {
  const c = listContainer.value
  if (
      !isStatic.value && hasMore.value && !loading.value &&
      c.scrollHeight - c.scrollTop <= c.clientHeight + 50
  ) {
    loadOptions()
  }
}

// Lifecycle
onMounted(() => {
  if (isStatic.value) {
    loadOptions()
  } else {
    resetLoad()
    loadOptions()
  }
})

// Sync
watch(
    () => props.modelValue,
    v => { localValue.value = [...v] }
)

watch(
    () => props.activeFilters,
    (newVal, oldVal) => {
      if (!props.filter.dependentParams) return
      const keys = Object.keys(props.filter.dependentParams)
      const changed = keys.some(k =>
          JSON.stringify(newVal[k]) !== JSON.stringify(oldVal[k])
      )
      if (changed && isOpen.value && !isStatic.value) {
        resetLoad()
        loadOptions()
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
  transition: border-color var(--transition-duration) var(--transition-timing);
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
  padding: 8px 12px;
  transition: background-color var(--transition-duration) var(--transition-timing);
  border-radius: var(--border-radius);
}

.options-list__item:hover {
  background-color: var(--hover-color);
}

.options-list__item label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.options-list__item input[type="checkbox"] {
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