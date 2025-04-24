<script setup>
import { ref, watch } from 'vue'
import axios from 'axios'
import StatusBadge from '@/components/Table/StatusBadge.vue'
import FiltersContainer from '@/components/Filters/FiltersContainer.vue'
import Table from '@/components/Table/Table.vue'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import menuItems from '@/router/menuData.js'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'
import FormModal from '@/components/Form/FormModal.vue'

const props = defineProps({
  config: {
    type: Object,
    required: true
  },
  pageTitle: {
    type: String,
    required: true
  }
})

const {
  tableConfig,
  filters,
  apiConfig,
  initialSort,
  createFormConfig,
  editFormConfig = createFormConfig
} = props.config

// Реактивные переменные
const searchQuery = ref('')
const currentFilters = ref({})
const currentSort = ref({ ...initialSort })
const tableItems = ref([])
const loading = ref(false)
const hasMore = ref(false)
const skip = ref(0)
const isOpen = ref(false)
const total = ref(0)
const isCreateModalVisible = ref(false)

// Модалки
const isEditModalVisible = ref(false)
const isEditing = ref(false)
const editingData = ref({})

// Методы
function handleFilterChange (f) {
  currentFilters.value = f
  loadData(true)
}

function handleSort (key) {
  const desc = currentSort.value.key === key && !currentSort.value.descending
  currentSort.value = { key, descending: desc }
  loadData(true)
}

const loadMore = () => {
  if (hasMore.value) {
    loadData()
  }
}

const buildRequestParams = (resetPagination = false) => {
  if (resetPagination) {
    skip.value = 0
  }

  return {
    [apiConfig.paramsMapping.search]: searchQuery.value,
    [apiConfig.paramsMapping.sortKey]: currentSort.value.key,
    [apiConfig.paramsMapping.sortOrder]: currentSort.value.descending,
    [apiConfig.paramsMapping.take]: 10,
    [apiConfig.paramsMapping.skip]: skip.value,
    ...currentFilters.value
  }
}

const loadData = async (reset = false) => {
  try {
    loading.value = true
    const params = buildRequestParams(reset)
    console.log(params)
    const { data } = await axios.post(
        `${BACKEND_API_HOST}${apiConfig.endpoint}`,
        params
    )

    tableItems.value = reset ? data.items : [...tableItems.value, ...data.items]
    if (params.Skip === 0) {
      total.value = data.total
    }
    hasMore.value = data.hasMore
    skip.value += data.items.length

  } catch (error) {
    console.error('Ошибка загрузки данных:', error)
  } finally {
    loading.value = false
  }
}

const toggleVisibility = () => {
  isOpen.value = !isOpen.value
}

function openCreateModal () {
  editingData.value = {}
  isCreateModalVisible.value = true
}

function openEditModal (item) {
  editingData.value = { ...item }
  isEditModalVisible.value = true
}

function onFormSubmit () {
  loadData(true)
}

// Инициализация
loadData(true)
watch(searchQuery, () => loadData(true))
</script>

<template>
  <div class="main-layout">
    <Sidebar :items="menuItems"/>

    <div class="content-area">

      <div class="selection">
        <div class="selection-left">
          <div class="search-block">
            <h1>{{ pageTitle }}</h1>
          </div>
          <slot name="options"/>
        </div>
        <div class="selection-right">
          <button @click="openCreateModal" class="button">Создать</button>
        </div>
      </div>

      <div class="selection">
        <div class="selection-left">
          <div class="search-block">
            <input
                v-model.lazy="searchQuery"
                type="text"
                :placeholder="`Поиск по ${pageTitle.toLowerCase()}`"
                class="search-input"
            />
          </div>
          <slot name="options"/>
        </div>
        <div class="selection-right">
          <button class="button primary" @click="toggleVisibility">
            <span>{{ isOpen ? '▲' : '▼' }}</span>
            <span>Фильтры</span>
          </button>
        </div>
      </div>

      <FiltersContainer
          v-if="isOpen"
          :filters="filters"
          :values="currentFilters"
          @update-filters="handleFilterChange"
      />

      <div class="table-container">
        <Table
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
            @row-click="openEditModal"
        >
          <template #cell-isActive="{ item }">
            <StatusBadge :status="item.isActive"/>
          </template>
          <slot name="custom-cells"/>
        </Table>
      </div>
    </div>
  </div>

  <FormModal
      :config="{ ...createFormConfig}"
      :isVisible="isCreateModalVisible"
      :isEditing="false"
      @update:isVisible="isCreateModalVisible = $event"
      @submit="onFormSubmit"
  />
  <FormModal
      :config="{ ...editFormConfig, initialData: editingData }"
      :isVisible="isEditModalVisible"
      :isEditing="isEditing"
      @update:isVisible="isEditModalVisible = $event"
      @submit="onFormSubmit"
  />
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