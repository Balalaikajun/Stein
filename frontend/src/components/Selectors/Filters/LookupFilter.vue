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
      <template #body>
        <div class="filter-modal-content">
          <input
              v-if="filter.searchable"
              v-model="search"
              @input="handleSearch"
              placeholder="Поиск..."
              class="filter-input"
              :disabled="loading"
          />

          <div v-if="loading" class="loading-state">
            <span class="loader"></span>
            <span>Загрузка...</span>
          </div>

          <div v-else-if="error" class="error-state">
            Ошибка загрузки: {{ error }}
            <button @click="loadOptions" class="retry-button">Повторить</button>
          </div>

          <ul v-else class="options-list">
            <li
                v-for="opt in options"
                :key="getOptionKey(opt)"
                @click="select(opt.value)"
                :class="{ selected: isSelected(opt.value) }"
            >
              {{ opt.label }}
            </li>

            <li v-if="options.length === 0" class="empty-state">
              Ничего не найдено
            </li>
          </ul>
        </div>
      </template>
    </FilterModal>
  </div>
</template>

<script setup>
import { ref, watch, onMounted, nextTick, computed } from 'vue'
import axios from 'axios'
import isEqual from 'lodash.isequal'
import FilterButton from '@/components/Selectors/FilterButton.vue'
import FilterModal from '@/components/Selectors/FilterModal.vue'
import { debounce } from 'lodash-es'

const props = defineProps({
  filter: {
    type: Object,
    required: true,
    validator: (f) => f.api && f.mapOption
  },
  modelValue: {
    type: [Object, Number, Array, String, Boolean],
    default: null
  }
})

const emit = defineEmits(['update:modelValue'])

// State
const isOpen = ref(false)
const search = ref('')
const options = ref([])
const local = ref(null)
const anchorRect = ref(null)
const btnRef = ref(null)
const loading = ref(false)
const error = ref(null)

// Computed
const hasValue = computed(() => {
  return !isEqual(props.modelValue, null) &&
      !isEqual(props.modelValue, undefined) &&
      !(Array.isArray(props.modelValue) && props.modelValue.length === 0)
})

// Methods
const getOptionKey = (opt) => {
  return typeof opt.value === 'object'
      ? JSON.stringify(opt.value)
      : opt.value
}

const loadOptions = async () => {
  try {
    loading.value = true
    error.value = null

    const params = {
      q: search.value,
      // Можно добавить другие параметры из props.filter
      ...(props.filter.apiParams || {})
    }

    console.log(params)
    const { data } = await axios.get(props.filter.api, {
      params
    })

    // Обработка разных форматов ответа
    const items = Array.isArray(data)
        ? data
        : data?.results || data?.items || data?.data || []

    options.value = items.map(props.filter.mapOption)
  } catch (err) {
    console.error('Ошибка загрузки опций:', err)
    error.value = err.message
    options.value = []
  } finally {
    loading.value = false
  }
}

const toggle = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    nextTick(() => {
      const rect = btnRef.value?.$el?.getBoundingClientRect?.()
      if (rect) anchorRect.value = rect
    })
  }
}

const select = (value) => {
  local.value = isEqual(local.value, value) ? null : value
}

const isSelected = (value) => {
  return isEqual(local.value, value)
}

const apply = () => {
  emit('update:modelValue', local.value)
  close()
}

const close = () => {
  isOpen.value = false
  search.value = ''
  local.value = props.modelValue
}

const handleSearch = debounce(() => {
  loadOptions()
}, 300)

// Hooks
onMounted(async () => {
  await loadOptions()
  local.value = props.modelValue
})

// Watchers
watch(search, () => {
  if (!isOpen.value) return
  loadOptions()
})

watch(() => props.modelValue, (newVal) => {
  local.value = newVal
})
</script>

<style scoped>
.filter-item {
  position: relative;
  display: inline-block;
}

.filter-modal-content {
  padding: 12px;
  min-width: 200px;
}

.filter-input {
  width: 100%;
  padding: 8px 12px;
  margin-bottom: 12px;
  border: 1px solid var(--secondary-background-color);
  border-radius: var(--border-radius);
}

.options-list {
  max-height: 300px;
  overflow-y: auto;
  list-style: none;
  padding: 0;
  margin: 0;
}

.options-list li {
  padding: 8px 12px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.options-list li:hover {
  background-color: var(--hover-color);
}

.options-list li.selected {
  background-color: var(--active-bg-color);
  font-weight: 500;
}

.options-list li.empty-state {
  cursor: default;
  color: var(--secondary-text-color);
  text-align: center;
  padding: 16px;
}

.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 16px;
  color: var(--secondary-text-color);
}

.loader {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid var(--primary-color);
  border-radius: 50%;
  border-top-color: transparent;
  animation: spin 1s linear infinite;
  margin-right: 8px;
}

.error-state {
  color: var(--error-color);
  padding: 12px;
  text-align: center;
}

.retry-button {
  margin-top: 8px;
  padding: 4px 8px;
  background: var(--primary-color);
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>