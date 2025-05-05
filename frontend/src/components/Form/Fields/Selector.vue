<template>
  <div class="filter-item">
    <FilterButton
        ref="btnRef"
        :title="selectedLabel"
        @toggle="toggle"
        :isOpen="isOpen"
        :hasValue="hasValue"
        :loading="loading"
    />

    <Teleport to="body">
      <div
          v-if="isOpen"
          class="filter-dropdown"
          :style="dropdownStyle"
          ref="dropdownRef"
      >
        <div class="filter-dropdown__header">
          <input
              v-model="search"
              @input="onSearch"
              placeholder="Поиск..."
              class="filter-input"
              :disabled="loading"
          />
        </div>

        <div class="filter-dropdown__body" ref="listContainer" @scroll="onScroll">
          <div v-if="isEmpty && loading" class="loading-state">
            <span class="loader"></span>
            <span>Загрузка...</span>
          </div>

          <ul v-else class="options-list">
            <li
                v-for="opt in options"
                :key="optKey(opt)"
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

        <div class="filter-dropdown__footer">
          <button class="btn btn--cancel" @click="close">Отмена</button>
          <button class="btn btn--apply" @click="apply">Применить</button>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, computed, nextTick, onMounted, onBeforeUnmount } from 'vue'
import axios from 'axios'
import { debounce } from 'lodash-es'
import FilterButton from '@/components/Filters/FilterButton.vue'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

// Props & Emits
const props = defineProps({
  filter: { type: Object, required: true },
  modelValue: {},
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
const dropdownRef = ref(null)
const listContainer = ref(null)
const modalPosition = ref({ top: 0, left: 0 })

// Config
const isStatic = computed(() => Array.isArray(props.filter.staticOptions))
const pageSize = props.filter.params?.take || props.filter.take || 50
const debouncedSearch = debounce(loadOptions, props.filter.debounceMs || 300)

// Computed
const hasValue = computed(() => localValue.value != null)
const selectedLabel = computed(() => {
  if (localValue.value == null && props.filter.allowDeselect) {
    return props.filter.staticOptions.find(o => o.value === null)?.label || 'Все'
  }
  const found = options.value.find(o => JSON.stringify(o.value) === JSON.stringify(localValue.value))
  return found?.label || props.filter.title
})
const isEmpty = computed(() => options.value.length === 0)
const dropdownStyle = computed(() => ({
  top: modalPosition.value.top + 'px',
  left: modalPosition.value.left + 'px'
}))

// Helpers
const optKey = opt => typeof opt.value === 'object' ? JSON.stringify(opt.value) : opt.value

function buildBody () {
  const body = { skip: skip.value, take: pageSize, searchText: search.value, ...(props.filter.params || {}) }
  Object.entries(props.filter.dependentParams || {}).forEach(([fid, key]) => {
    const v = props.activeFilters[fid]
    if (v?.length) body[key] = v
  })
  return body
}

// Load
async function loadOptions () {
  if (loading.value) return
  loading.value = true
  if (isStatic.value) {
    const all = props.filter.staticOptions
    options.value = all.filter(o => !search.value || o.label.toLowerCase().includes(search.value.toLowerCase()))
    hasMore.value = false
  } else {
    if (!hasMore.value) {
      loading.value = false
      return
    }
    try {
      const { data } = await axios.post(`${BACKEND_API_HOST}${props.filter.apiEndpoint}`, buildBody())
      const items = data.items || []
      options.value.push(...items.map(props.filter.mapOption))
      hasMore.value = data.hasMore
      skip.value += items.length
    } catch {}
  }
  loading.value = false
}

// Reset & events
function reset () {
  options.value = []
  skip.value = 0
  hasMore.value = true
}

function onSearch () {
  reset()
  debouncedSearch()
}

function onScroll () {
  const el = listContainer.value
  if (!isStatic.value && hasMore.value && !loading.value && el.scrollTop + el.clientHeight >= el.scrollHeight - 50) {
    loadOptions()
  }
}

function toggle () {
  isOpen.value = !isOpen.value
  if (!isOpen.value) return
  search.value = ''
  localValue.value = props.modelValue
  nextTick(() => {
    const btn = btnRef.value.$el.getBoundingClientRect()
    const height = isStatic.value ? Math.min(400, 48 + 48 + 32 * options.value.length) : 400
    const below = window.innerHeight - btn.bottom
    const top = below >= height + 8 ? btn.bottom + 8 : btn.top - height - 8
    modalPosition.value = { top, left: btn.left }
    reset()
    loadOptions()
  })
}

function apply () {
  emit('update:modelValue', typeof localValue.value === 'string' ? localValue.value === 'true' : localValue.value)
  close()
}

function close () {
  isOpen.value = false
  search.value = ''
}

function handleClickOutside (e) {
  if (isOpen.value && dropdownRef.value && !dropdownRef.value.contains(e.target) && !btnRef.value.$el.contains(e.target)) {
    close()
  }
}

onMounted(() => document.addEventListener('click', handleClickOutside))
onBeforeUnmount(() => document.removeEventListener('click', handleClickOutside))

// init
reset()
loadOptions()
</script>

<style scoped>
.filter-item {
  width: 100%;
}

.filter-dropdown {
  position: fixed;
  z-index: 2000;
  min-width: 240px;
  max-height: 400px;
  background: #fff;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  border-radius: var(--border-radius);
  display: flex;
  flex-direction: column
}

.filter-dropdown__header {
  padding: 8px;
  border-bottom: 1px solid var(--secondary-background-color)
}

.filter-dropdown__body {
  flex: 1;
  overflow-y: auto;
  padding: 8px
}

.filter-dropdown__footer {
  padding: 8px;
  border-top: 1px solid var(--secondary-background-color);
  display: flex;
  justify-content: flex-end;
  gap: 8px
}

.btn {
  padding: 6px 12px;
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer
}

.btn--cancel {
  background: var(--secondary-background-color);
  color: var(--secondary-text-color)
}

.btn--apply {
  background: var(--primary-color);
  color: #fff
}

.filter-input {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid var(--secondary-background-color);
  border-radius: var(--border-radius)
}

.options-list {
  list-style: none;
  margin: 0;
  padding: 0
}

.options-list__item {
  padding: 4px 0
}

.radio-option {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px;
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: background-color .2s
}

.radio-option:hover {
  background-color: var(--hover-color)
}

.empty-state, .loading-more {
  padding: 12px;
  text-align: center;
  color: var(--secondary-text-color)
}

.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 16px;
  color: var(--secondary-text-color)
}

.loader {
  width: 18px;
  height: 18px;
  border: 2px solid var(--secondary-background-color);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 1s linear infinite
}

@keyframes spin {
  to {
    transform: rotate(360deg)
  }
}
</style>
