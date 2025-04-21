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
import { computed, nextTick, onMounted, ref, watch } from 'vue'
import axios from 'axios'
import { debounce } from 'lodash-es'
import FilterButton from '@/components/Selectors/FilterButton.vue'
import FilterModal from '@/components/Selectors/FilterModal.vue'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

// Props include filter config and v-model value
const props = defineProps({
  filter: { type: Object, required: true },
  modelValue: { type: Array, default: () => [] }
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

// Computed
const hasValue = computed(() => localValue.value.length > 0)

// Generate a stable key for v-for
function uniqueKey (opt) {
  return typeof opt.value === 'object' ? JSON.stringify(opt.value) : opt.value
}

// Build request payload using config
function buildRequestBody () {
  const { params = {}, paramKeys = {} } = props.filter
  return {
    [paramKeys.skip || 'skip']: skip.value,
    [paramKeys.take || 'take']: params.take || 50,
    [paramKeys.search || 'searchText']: search.value,
    ...(params.activeFilter !== undefined && {
      [paramKeys.activeFilter || 'activeFilter']: params.activeFilter
    }),
    ...(params.sortBy && {
      [paramKeys.sortKey || 'sortBy']: params.sortBy,
      [paramKeys.sortOrder || 'descending']: params.descending || false
    })
  }
}

// Fetch options from API
async function loadOptions () {
  if (!hasMore.value || loading.value) return
  loading.value = true
  try {
    const body = buildRequestBody()
    const { data } = await axios.post(
        `${BACKEND_API_HOST}${props.filter.apiEndpoint}`,
        body
    )
    const items = data.items || []
    // Map and append
    options.value.push(...items.map(props.filter.mapOption))
    hasMore.value = data.hasMore
    skip.value += items.length
  } catch (err) {
    console.error('Ошибка загрузки опций:', err)
  } finally {
    loading.value = false
  }
}

// Reset pagination and options
function resetLoad () {
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
}, 300)

const onScroll = () => {
  const c = listContainer.value
  if (
      hasMore.value && !loading.value &&
      c.scrollHeight - c.scrollTop <= c.clientHeight + 50
  ) {
    loadOptions()
  }
}

// Initial load
onMounted(() => {
  resetLoad()
  loadOptions()
})

// Sync external changes
watch(
    () => props.modelValue,
    v => { localValue.value = [...v] }
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