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
  console.log(currentSort.value.descending)
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

  console.log(skip.value)
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


  return params
}

const loadData = async (reset = false) => {
  try {
    loading.value = true
    const params = buildRequestParams(reset)

    console.log(params)
    const { data } = await axios.post(
        `https://localhost:7203${apiConfig.endpoint}`,
        params
    )

    if (reset) {
      tableItems.value = data.items
    } else {
      tableItems.value.push(...data.items)
    }

    hasMore.value = data.hasMore
    skip.value += data.items.length

    console.log(data)
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
                v-model="searchQuery"
                type="text"
                placeholder="Поиск по тексту"
                @change="onFilterChange"
            />
          </div>
          <slot name="options"></slot>
        </div>
        <div class="selection-right">
          <button class="exit-button" @click="toggleVisibility">
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

      <DataTable
          :columns="tableConfig.columns"
          :items="tableItems"
          :loading="loading"
          :has-more="hasMore"
          :sort-by="currentSort.key"
          :sort-descending="currentSort.descending"
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
</template>

<style scoped>
.main-layout {
  display: flex;
  min-height: 100vh;
}

.content-area {
  flex: 1;
  padding: 2rem;
  background: #f5f5f5;
}

h1 {
  margin-bottom: 1.5rem;
  color: #333;
  font-size: 24px;
}

.search-block {
  margin-bottom: 1rem;
}

.search-input {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  width: 300px;
  max-width: 100%;
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
</style>