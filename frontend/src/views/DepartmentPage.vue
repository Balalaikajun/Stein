<script setup>
import { ref, watch } from 'vue'
import axios from 'axios'
import DepartmentConfig from '@/configs/department'
import StatusBadge from '@/components/Table/StatusBadge.vue'
import DataTable from '@/components/Table/DataTable.vue'
import FiltersContainer from '@/components/Selectors/FiltersContainer.vue'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import menuItems from '@/router/menuData.js'

const {
  tableConfig,
  filters,
  apiConfig,
  initialSort
} = DepartmentConfig

// Реактивные переменные
const searchQuery = ref('')
const currentFilters = ref({})
const currentSort = ref({ ...initialSort })
const tableItems = ref([])
const loading = ref(false)
const hasMore = ref(false)
const skip = ref(0)
const isOpen = ref(false)
const total = ref(0);

// Обработчики событий
const handleFilterChange = (filters) => {
  currentFilters.value = filters
  loadData(true)
}

const handleSort = (key) => {
  currentSort.value = {
    key,
    descending: currentSort.value.key === key ? !currentSort.value.descending : false
  }
  loadData(true)
}

const loadMore = () => {
  if (hasMore.value) {
    loadData()
  }
}

// Логика загрузки данных
const buildRequestParams = (resetPagination = false) => {
  if (resetPagination) {
    skip.value = 0
  }

  // Всегда явно передаем параметры сортировки
  const params = {
    [apiConfig.paramsMapping.search]: searchQuery.value,
    [apiConfig.paramsMapping.sortKey]: currentSort.value.key,
    [apiConfig.paramsMapping.sortOrder]: currentSort.value.descending,
    [apiConfig.paramsMapping.take]: 10,
    [apiConfig.paramsMapping.skip]: skip.value,
    ...currentFilters.value
  }

  console.log(params)
  console.log(currentFilters.value)

  return params
}

const loadData = async (reset = false) => {
  try {
    loading.value = true
    const params = buildRequestParams(reset)

    const { data } = await axios.post(
        `https://localhost:7203${apiConfig.endpoint}`,
        params
    )

    if (reset) {
      tableItems.value = data.items
    } else {
      tableItems.value.push(...data.items)
    }


    if(params.Skip===0){
      total.value = data.total
    }
console.log(total.value)

    hasMore.value = data.hasMore
    skip.value += data.items.length

  } catch (error) {
    console.error('Ошибка загрузки данных:', error)
  } finally {
    loading.value = false
  }
}

// Начальная загрузка данных
loadData(true)

// Реакция на изменение поискового запроса
watch(searchQuery, () => {
  loadData(true)
})

const toggleVisibility = () => {
  isOpen.value = !isOpen.value
}
</script>

<template>
  <div class="main-layout">
    <Sidebar :items="menuItems"/>

    <div class="content-area">
      <h1>Номенклатура</h1>

      <div class="selection">
        <div class="selection-left">
          <div class="search-block">
            <input
                id="searchQuery"
                v-model.lazy="searchQuery"
                type="text"
                placeholder="Поиск по тексту"
                @change="onFilterChange"
                class="search-input"
            />
          </div>
          <slot name="options"></slot>
        </div>
        <div class="selection-right">
          <button class="button primary" @click="toggleVisibility">
            <span>{{ isOpen ? '▲' : '▼' }}</span>
            <span>Фильтры</span>
          </button>
        </div>
      </div>

      <div v-if="isOpen">
        <FiltersContainer
            :filters="filters"
            :values="currentFilters"
            @update-filters="handleFilterChange"
        />
      </div>

      <div class="table-container">
        <DataTable
            :columns="tableConfig.columns"
            :items="tableItems"
            :loading="loading"
            :has-more="hasMore"
            :sort-by="currentSort.key"
            :sort-descending="currentSort.descending"
            :total="total"
            editable
            @sort="handleSort"
            @load-more="loadMore"
        >
          <template #cell-isActive="{ item }">
            <StatusBadge :status="item.isActive"/>
          </template>
        </DataTable>
      </div>
    </div>
  </div>
</template>

<style scoped>
.main-layout {
  height: 100vh;
}

.content-area {
  padding: 2rem;
  gap: 1.5rem;
  background: var(--secondary-background-color);
}

h1 {
  margin-bottom: 0;
  color: var(--text-color);
}

.selection {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.selection-left {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.search-input {
  width: 100%;
  max-width: 300px;
  padding: 0.5rem 1rem;
  border: 1px solid var(--secondary-text-color);
  border-radius: var(--border-radius);
  transition: border-color var(--transition-duration) var(--transition-timing);
}

.search-input:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(91, 0, 225, 0.1);
}

.button {
  padding: 0.5rem 1rem;
  border-radius: var(--border-radius);
  border: 1px solid transparent;
  transition: all var(--transition-duration) var(--transition-timing);
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.button.primary {
  background: var(--primary-color);
  color: var(--background-color);
}

.button.primary:hover {
  background: var(--secondary-color);
}

.table-container {
  flex: 1;
  background: var(--background-color);
  border-radius: var(--border-radius);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

@media (max-width: 768px) {
  .content-area {
    padding: 1rem;
  }

  .selection {
    flex-direction: column;
    align-items: stretch;
  }

  .search-input {
    max-width: none;
  }
}
</style>