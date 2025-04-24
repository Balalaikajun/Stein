<template>
  <div class="main-layout">
    <Sidebar :items="menuItems" />

    <div class="content-area">
      <h1>{{ pageTitle }}</h1>

      <div class="filters-container">
        <FiltersContainer
            :filters="filters"
            :values="currentFilters"
            @update-filters="onFilterChange"
        />
      </div>

      <PerformanceTable
          class="table-container"
          :student-columns="tableConfig.studentColumns"
          :students="students"
          :performance-items="performanceItems"
          :total="totalStudents"
          :loading="loading"
          :has-more="hasMoreStudents"
          :has-more-months="hasMoreMonths"
          :sort-by="currentSort.key"
          :sort-descending="currentSort.descending"
          editable
          @sort="onSort"
          @load-more="() => loadStudents(false)"
          @load-more-months="() => loadPerformance(false)"
      >
        <template #student-cell-status="{ student }">
          <StatusBadge :status="student.status" />
        </template>
        <slot name="custom-cells" />
      </PerformanceTable>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import axios from 'axios'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import FiltersContainer from '@/components/Filters/FiltersContainer.vue'
import PerformanceTable from '@/components/Table/PerformanceTable.vue'
import StatusBadge from '@/components/Table/StatusBadge.vue'
import menuItems from '@/router/menuData.js'
import academicConfig from '@/configs/performance.js'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

// Props
defineProps({
  pageTitle: { type: String, required: true }
})

// Configs
const { tableConfig, filters, apiConfig, initialSort } = academicConfig

// Students state
const students = ref([])
const studentSkip = ref(0)
const studentTake = 10
const hasMoreStudents = ref(false)
const totalStudents = ref(0)

// Performance (months) state
const performanceItems = ref([])
const perfSkip = ref(0)
const perfTake = 10
const hasMoreMonths = ref(false)

// UI state
const loading = ref(false)
const currentFilters = ref({})
const currentSort = ref({ ...initialSort })

// Common params builder
function buildParams(mapping, skip, take) {
  const params = {
    [mapping.take]: take,
    [mapping.skip]: skip
  }
  if (mapping.search)    params[mapping.search] = ''
  if (mapping.sortKey)   params[mapping.sortKey] = currentSort.value.key
  if (mapping.sortOrder) params[mapping.sortOrder] = currentSort.value.descending
  Object.assign(params, currentFilters.value)
  return params
}

// Load students
async function loadStudents(reset = false) {
  if (reset) studentSkip.value = 0
  loading.value = true
  try {
    const params = buildParams(apiConfig.student.paramsMapping, studentSkip.value, studentTake)
    const res = await axios.post(
        `${BACKEND_API_HOST}${apiConfig.student.endpoint}`,
        params
    )
    const { items, hasMore, total } = res.data
    students.value = reset ? items : students.value.concat(items)
    hasMoreStudents.value = hasMore
    totalStudents.value = reset ? total : totalStudents.value
    studentSkip.value += items.length
  } catch (e) {
    console.error('Error loading students:', e)
  } finally {
    loading.value = false
  }
}

// Load performance items
async function loadPerformance(reset = false) {
  if (reset) perfSkip.value = 0
  loading.value = true
  try {
    const params = buildParams(apiConfig.performance.paramsMapping, perfSkip.value, perfTake)
    console.log(params)
    const res = await axios.post(
        `${BACKEND_API_HOST}${apiConfig.performance.endpoint}`,
        params
    )
    const { items, hasMore } = res.data
    performanceItems.value = reset ? items : performanceItems.value.concat(items)
    hasMoreMonths.value = hasMore
    perfSkip.value += params.take
  } catch (e) {
    console.error('Error loading performance:', e)
  } finally {
    loading.value = false
  }
}

// Handlers
function onFilterChange(newFilters) {
  currentFilters.value = newFilters
  loadStudents(true)
  loadPerformance(true)
}

function onSort(key) {
  if (currentSort.value.key === key) {
    currentSort.value.descending = !currentSort.value.descending
  } else {
    currentSort.value = { key, descending: false }
  }
  loadStudents(true)
  loadPerformance(true)
}


</script>

<style scoped>
.main-layout { display: flex; height: 100vh; }
.content-area {
  flex: 1;
  padding: 2rem;
  background: var(--secondary-background-color);
  overflow: hidden;
  display: flex;
  flex-direction: column;
}
h1 { margin: 0 0 1rem; color: var(--text-color); }
.filters-container { margin-bottom: 1.5rem; }
.table-container { flex: 1; }
</style>
