<template>
  <div class="container">
    <!-- Фильтрация и поиск -->
    <div class="content-selection">
      <div class="selection">
        <div class="selection-left">
          <div class="search-block">
            <input
                id="searchQuery"
                v-model="searchQuery"
                type="text"
                placeholder="Поиск по тексту"
                @change="onFilterChange"
            />
          </div>
          <slot name="options"></slot>
        </div>
        <div class="selection-right">
          <button class="exit-button" @click="toggleFiltersOpen">
            <span>{{ filtersIsOpen ? '▲' : '▼' }}</span>
            <span>Фильтры</span>
          </button>
        </div>
      </div>
      <div v-if="filtersIsOpen">
        <filters-container :filters="filters" @update-filters="onFilterChange" />
      </div>
    </div>

    <!-- Таблица данных -->
    <div class="content-table">
      <table class="data-table">
        <thead>
        <tr>
          <th
              v-for="col in config.columns"
              :key="col.key"
              :style="{ width: col.width || 'auto' }"
              :class="{
                sortable: editable,
                sorted: sortBy === col.key,
                'text-center': col.align === 'center',
                'text-right': col.align === 'right'
              }"
              @click="editable && handleSort(col.key)"
          >
            <div class="th-content">
              <span class="title-text">{{ col.title }}</span>
              <span class="sort-icon-wrapper">
                  <span v-if="sortBy === col.key" class="sort-icon">
                    {{ sortAscending ? '▲' : '▼' }}
                  </span>
                  <span v-else class="sort-icon-placeholder"></span>
                </span>
            </div>
          </th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="item in tableItems" :key="item.Id">
          <td
              v-for="col in config.columns"
              :key="col.key"
              :class="{
                'text-center': col.align === 'center',
                'text-right': col.align === 'right'
              }"
          >
            <slot :name="`cell-${col.key}`" :item="item">
              {{ item[col.key] }}
            </slot>
          </td>
        </tr>
        <tr v-if="!tableItems.length && !loading">
          <td :colspan="config.columns.length" class="no-data">
            Нет данных для отображения
          </td>
        </tr>
        </tbody>
      </table>

      <!-- Кнопка подгрузки данных -->
      <div v-if="!loading && hasMore" class="load-more">
        <button @click="loadMoreData">Загрузить ещё</button>
      </div>
      <!-- Индикатор загрузки -->
      <div v-if="loading" class="loading-indicator">
        Загрузка данных...
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import FiltersContainer from '@/components/Selectors/FiltersContainer.vue'

const props = defineProps({
  config: {
    type: Object,
    required: true
  },
  filters: {
    type: Array,
    default: () => []
  },
  editable: {
    type: Boolean,
    default: false
  },
  initialSort: {
    type: Object,
    default: () => ({ key: 'Title', ascending: true })
  },
  apiConfig: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['sort-change'])

// Список полученных данных
const tableItems = ref([])
const loading = ref(false)
const sortBy = ref(props.initialSort.key)
const sortAscending = ref(props.initialSort.ascending)
const searchQuery = ref('')
const selectedFilters = ref({})
const filtersIsOpen = ref(true)
const lastSeenId = ref(null)
const lastSeenValue = ref(null)
const hasMore = ref(false)

const buildRequestParams = () => {
  const params = {
    [props.apiConfig.paramsMapping.search]: searchQuery.value,
    [props.apiConfig.paramsMapping.sortKey]: sortBy.value,
    [props.apiConfig.paramsMapping.sortOrder]: !sortAscending.value,
    [props.apiConfig.paramsMapping.limit]: 10
  }

  Object.entries(selectedFilters.value).forEach(([filterKey, value]) => {
    const filterConfig = props.filters.find(f => f.id === filterKey)
    if (!filterConfig || value === null || value === undefined) return

    if (typeof value === 'object' && !Array.isArray(value)) {
      Object.entries(value).forEach(([subKey, subValue]) => {
        params[`${filterConfig.paramKey}_${subKey}`] = subValue
      })
    } else {
      params[filterConfig.paramKey] = value
    }
  })

  if (lastSeenId.value !== null) params[props.apiConfig.paramsMapping.lastSeenId] = lastSeenId.value
  if (lastSeenValue.value !== null) params[props.apiConfig.paramsMapping.lastSeenValue] = lastSeenValue.value

  return params
}

const loadData = async (reset = false) => {
  if (reset) {
    tableItems.value = []
    lastSeenId.value = null
    lastSeenValue.value = null
  }

  loading.value = true
  try {

    const { data } = await axios.post(
        `https://localhost:7203${props.apiConfig.endpoint}`,
        buildRequestParams()
    )

    tableItems.value.push(...data.items)
    hasMore.value = data.hasMore
    lastSeenId.value = data.lastSeenId
    lastSeenValue.value = data.lastSeenValue
  } catch (error) {
    console.error('Ошибка загрузки данных:', error)
  } finally {
    loading.value = false
  }
}


// Подгрузка следующей порции данных
const loadMoreData = () => {
  if (hasMore.value) {
    loadData()
  }
}

// Смена сортировки
const handleSort = (colKey) => {
  if (sortBy.value === colKey) {
    sortAscending.value = !sortAscending.value
  } else {
    sortBy.value = colKey
    sortAscending.value = true
  }
  emit('sort-change', { key: sortBy.value, ascending: sortAscending.value })
  loadData(true)
}

// Обработка изменений фильтрации и поиска
const onFilterChange = (newFilters) => {
  // Объединяем фильтры вместо перезаписи
  selectedFilters.value = { ...selectedFilters.value, ...newFilters }
  loadData(true)
}

// Переключение видимости блока фильтров
const toggleFiltersOpen = () => {
  filtersIsOpen.value = !filtersIsOpen.value
}

// Начальная загрузка данных при монтировании компонента
onMounted(() => {
  loadData(true)
})
</script>

<style scoped>
.container {
  padding: 24px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  font-family: 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', sans-serif;
}

.content-selection {
  margin-bottom: 16px;
}

.selection {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  margin-bottom: 12px;
}

.selection-left {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.selection-right {
  display: flex;
  align-items: center;
}

.search-block input {
  padding: 8px 12px;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  width: 240px;
  transition: border-color 0.2s;
}

.search-block input:focus {
  outline: none;
  border-color: #1976d2;
  box-shadow: 0 0 0 2px rgba(25,118,210,0.2);
}

.exit-button {
  background: #1976d2;
  color: white;
  padding: 8px 16px;
  border-radius: 4px;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  transition: background 0.2s;
}

.exit-button:hover {
  background: #1565c0;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
  color: #333;
}

.data-table th,
.data-table td {
  padding: 12px 16px;
  border-bottom: 1px solid #e0e0e0;
  text-align: left;
}

.data-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #424242;
  position: relative;
}

.data-table tr:hover {
  background-color: #f5f5f5;
}

.data-table tr:nth-child(even) {
  background-color: #fafafa;
}

.sortable {
  cursor: pointer;
  transition: background 0.2s;
}

.sortable:hover {
  background-color: #f0f0f0;
}

.sorted {
  background-color: #ececec;
}

.th-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
}

.sort-icon-wrapper {
  width: 24px;
  text-align: center;
}

.title-text {
  flex: 1;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}

.sort-icon {
  color: #1976d2;
}

.sort-icon-placeholder {
  width: 16px;
  display: inline-block;
}

.no-data {
  padding: 24px;
  color: #757575;
  text-align: center;
  font-style: italic;
}

.load-more {
  text-align: center;
  margin: 16px 0;
}

.load-more button {
  padding: 8px 16px;
  font-size: 14px;
  background-color: #1976d2;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.2s;
}

.load-more button:hover {
  background-color: #1565c0;
}

.loading-indicator {
  text-align: center;
  padding: 16px 0;
  font-size: 14px;
  color: #757575;
}

.text-center {
  text-align: center;
}

.text-right {
  text-align: right;
}

@media (max-width: 768px) {
  .container {
    padding: 12px;
  }
  .selection {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;
  }
  .selection-left {
    flex-direction: column;
    align-items: stretch;
    gap: 8px;
  }
  .search-block input {
    width: 100%;
  }
  .data-table th,
  .data-table td {
    padding: 8px 12px;
  }
}
</style>
