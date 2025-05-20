<template>
  <div class="main-layout">
    <Sidebar :items="menuItems"/>

    <div class="content-area">
      <div class="dashboard-header">
        <h1>Главная</h1>
        <FiltersContainer
            :filters="filtersConfig"
            :values="filters"
            @update-filters="onFilterChange"
        />
      </div>



      <!-- Блок KPI -->
      <div class="kpi-container">
        <div v-if="kpiIsLoading" class="kpi-loading">
          Загрузка KPI…
        </div>
        <div v-else-if="kpiError" class="kpi-error">
          Ошибка при загрузке KPI.
          <button @click="fetchAllKpis">Повторить</button>
        </div>
        <template v-else>
          <KpiCard
              v-for="(item, index) in kpiList"
              :key="index"
              :title="item.title"
              :value="item.value"
              :trend="item.trend"
          />
        </template>
      </div>

      <div class="charts-container">
        <!-- Блок Pie-диаграмм -->
        <div class="chart-row pie-row">
          <div
              class="chart-wrapper"
              v-if="pieIsLoading"
          >
            Загрузка Pie‐данных…
          </div>
          <div
              v-else-if="pieError"
              class="chart-wrapper error"
          >
            Ошибка при загрузке Pie.
            <button @click="fetchAllPies">Повторить</button>
          </div>
          <template v-else>
            <div
                class="chart-wrapper"
                v-for="(data, key) in pies"
                :key="key"
            >
              <Pie
                  :pie="{ segments: data.segments, total: data.total }"
                  :title="pieNameMapping[key]"
                  :show-legend="true"
                  :animate="true"
                  :defaultColor="['#5B00E1', '#7C4DFF', '#FFD740 ']"
              />
            </div>
          </template>
        </div>

        <div class="chart-row">
          <div class="chart-wrapper">
            <PerformanceHistogram
                :data="newPerformance.data"
            />
          </div>
        </div>

        <div class="chart-row">
          <div class="chart-wrapper">
            <OrdersHistogram
                v-if="orders.labels?.length"
                :labels="orders.labels"
                :values="orders.values"
                title="Распределение приказов"
            >
              <template #controls>
                <div class="date-range-picker">
                  <div class="date-range">
                    <label>С:</label>
                    <input
                        type="date"
                        v-model="ordersFilters.startDate"
                        :max="ordersFilters.endDate"
                        @change="fetchOrdersData"
                    >
                  </div>
                  <div class="date-range">
                    <label>По:</label>
                    <input
                        type="date"
                        v-model="ordersFilters.endDate"
                        :min="ordersFilters.startDate"
                        @change="fetchOrdersData"
                    >
                  </div>
                </div>
              </template>
            </OrdersHistogram>
            <div v-else class="loading-placeholder">
              Загрузка данных приказов...
            </div>
          </div>
        </div>

        <div class="chart-row">
          <div class="chart-wrapper">
            <ContingentHistogram
                :data="chartData"
                type="Контингент"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import KpiCard from '@/components/Charts/KpiCard.vue'
import menuItems from '@/router/menuData.js'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import Pie from '@/components/Charts/Pie.vue'
import OrdersHistogram from '@/components/Charts/OrdersHistogram.vue'
import PerformanceHistogram from '@/components/Charts/PerformanceHistogram.vue'
import ContingentHistogram from '@/components/Charts/ContingentHistogram.vue'
import { useAllKpis } from '@/composables/useKpiCards.js'
import { usePieCards } from '@/composables/usePieCards.js'
import FiltersContainer from '@/components/Filters/FiltersContainer.vue'

const filters = ref({})
const filtersConfig = [
  {
    id: 'departmentIds',
    title: 'Кафедры',
    dataType: 'lookup',
    apiEndpoint: '/api/Department/filter',
    params: {
      take: 15,
      activeFilter: true,
      sortBy: 'Title',
      descending: false
    },
    paramKeys: {
      skip: 'skip',
      take: 'take',
      search: 'searchText',
      sortKey: 'sortBy',
      sortOrder: 'descending',
      activeFilter: 'activeFilter'
    },
    mapOption: opt => ({ label: opt.title, value: opt.id })
  },
  {
    id: 'SpecializationIds',
    title: 'Специализации',
    dataType: 'lookup',
    apiEndpoint: '/api/Specialization/filter',
    dependsOn: ['DepartmentIds'],
    dependentParams: {
      DepartmentIds: 'departmentIds'
    },
    params: {
      take: 15,
    },
    paramKeys: {
      skip: 'skip',
      take: 'take',
      search: 'searchText',
      sortKey: 'sortBy',
      sortOrder: 'descending'
    },
    mapOption: opt => ({ label: opt.title, value: opt.id })
  },
  {
    id: 'GroupKeys',
    title: 'Группы',
    dataType: 'lookup',
    apiEndpoint: '/api/Group/filter',
    dependsOn: ['DepartmentIds', 'SpecializationIds'],
    dependentParams: {
      DepartmentIds: 'departmentIds',
      SpecializationIds: 'specializationIds'
    },
    params: {
      take: 15,
    },
    paramKeys: {
      skip: 'skip',
      take: 'take',
      search: 'searchText',
      sortKey: 'sortBy',
      sortOrder: 'descending'
    },
    mapOption: opt => ({ label: opt.acronym, value: opt.id })
  },
]

const {
  kpis,
  kpiIsLoading, // Было isLoading
  kpiError,     // Было error
  fetchAllKpis
} = useAllKpis()

// Заголовки для каждого типа KPI
const kpiNamingMapping = {
  Students: 'Число студентов',
  Orders: 'Приказы за месяц',
  Foreigners: 'Иностранные граждане'
}

// Формируем массив для перебора в шаблоне
const kpiList = computed(() => {
  // kpis.value = { Students: { count: ... }, Orders: { count: ... }, Foreigners: { count: ... } }
  return Object.entries(kpis.value).map(([type, data]) => ({
    title: kpiNamingMapping[type] || type,
    value: data ? data.count : '-',
    trend: data ? data.trend : null
  }))
})

const pieTypes = ref(['Gender', 'AgeGroup', 'Nationality'])
const pieNameMapping = {
  gender: 'Распределение по полам',
  agegroup: 'Распределение во возрастам',
  nationality: 'Распределение по гражданству'
}
const {
  pies,
  pieIsLoading,
  pieError,
  fetchAllPies
} = usePieCards(pieTypes, filters.value)




const performanceFilter = ref({
  page: 1,
  pageSize: 6,
  totalPages: 1
})

const performance = ref({
  labels: [],
  values: [],
  performanceData: []
})

const ordersFilters = ref({
  startDate: new Date().toISOString().split('T')[0],
  endDate: new Date().toISOString().split('T')[0]
})
const orders = ref({
  labels: [
    'Зачисление',
    'Перевод из вуза',
    'Перевод в вуз',
    'Восст. из отчисления',
    'Восст. из академа',
    'Академотпуск',
    'Перевод между группами',
    'Отчисление',
    'Выпуск'
  ],
  values: [12, 5, 3, 2, 4, 7, 6, 9, 15]
})

const chartData = ref([
  // Физико-математический факультет
  {
    department: 'Физико-математический факультет',
    specialization: 'Прикладная математика',
    course: '1',
    group: 'ПМ-101',
    count: 10
  },
  {
    department: 'Физико-математический факультет',
    specialization: 'Прикладная математика',
    course: '1',
    group: 'ПМ-102',
    count: 15
  },
  {
    department: 'Физико-математический факультет',
    specialization: 'Прикладная математика',
    course: '2',
    group: 'ПМ-201',
    count: 12
  },
  {
    department: 'Физико-математический факультет',
    specialization: 'Прикладная математика',
    course: '2',
    group: 'ПМ-202',
    count: 8
  },
  {
    department: 'Физико-математический факультет',
    specialization: 'Теоретическая физика',
    course: '1',
    group: 'ТФ-101',
    count: 8
  },
  {
    department: 'Физико-математический факультет',
    specialization: 'Теоретическая физика',
    course: '1',
    group: 'ТФ-102',
    count: 10
  },

  // Филологический факультет
  {
    department: 'Филологический факультет',
    specialization: 'Русская литература',
    course: '1',
    group: 'РЛ-101',
    count: 20
  },
  {
    department: 'Филологический факультет',
    specialization: 'Русская литература',
    course: '1',
    group: 'РЛ-102',
    count: 18
  }
])

const specialtiesData = ref([
  {
    name: 'Информатика',
    total: 120,
    courses: {
      '1': 30,
      '2': 28,
      '3': 35,
      '4': 27
    }
  },
  {
    name: 'Математика',
    total: 90,
    courses: {
      '1': 25,
      '2': 22,
      '3': 24,
      '4': 19
    }
  }
])

const newPerformance = ref({
  data: [
    { Year: 2024, Month: 1, Count: 50, ExcellentCount: 15, GoodCount: 20, NormalCount: 10, FallingCount: 5 },
    { Year: 2024, Month: 2, Count: 48, ExcellentCount: 12, GoodCount: 22, NormalCount: 9, FallingCount: 5 },
    { Year: 2024, Month: 3, Count: 52, ExcellentCount: 18, GoodCount: 18, NormalCount: 10, FallingCount: 6 },
  ]
})

const defaultColors = ['#5B00E1', '#7C4DFF', '#FFD740', '#00B8D4', '#FF6D00']

const fetchOrdersData = async () => {
  try {
    const params = new URLSearchParams({
      start: ordersFilters.value.startDate,
      end: ordersFilters.value.endDate
    })
    // Ваш код для загрузки данных приказов
  } catch (error) {
    console.error('Ошибка загрузки:', error)
  }
}

async function fetchPerformance (p) {
  const allLabels = ['Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь', 'Январь', 'Февраль']
  const allValues = [82.5, 78.3, 75, 88.2, 90.1, 85.5]
  const allPerf = [
    { Excellent: 12, Good: 18, Normal: 5, Falling: 2 },
    { Excellent: 10, Good: 20, Normal: 7, Falling: 3 },
    { Excellent: 9, Good: 17, Normal: 6, Falling: 4 },
    { Excellent: 14, Good: 15, Normal: 8, Falling: 1 },
    { Excellent: 13, Good: 16, Normal: 9, Falling: 2 },
    { Excellent: 12, Good: 18, Normal: 5, Falling: 2 }
  ]
  const start = (p - 1) * performanceFilter.value.pageSize
  const end = start + performanceFilter.value.pageSize
  performance.value.labels = allLabels.slice(start, end)
  performance.value.values = allValues.slice(start, end)
  performance.value.performanceData = allPerf.slice(start, end)
  performanceFilter.value.totalPages = Math.ceil(allLabels.length / performanceFilter.value.pageSize)
}

function onMetricsPageChange (newPage) {
  performanceFilter.value.page = newPage
  fetchPerformance(newPage)
}

function onFilterChange (newFilter) {
  console.log(newFilter)

  fetchAllPies(newFilter)
}

onMounted(() => {
  fetchPerformance(performanceFilter.value.page)
})
</script>

<style scoped>
.main-layout {
  display: flex;
  height: 100vh;
}

.content-area {
  flex: 1;
  overflow-y: auto;
  max-height: 100vh;
  padding-bottom: 1rem;
}

.dashboard-header {
  padding: 0 1rem;
  margin-bottom: 1.5rem;
}

.kpi-container {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
  padding: 0 1rem;
}

.charts-container {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  padding: 0 1rem;
}

.chart-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 1.5rem;
}

/* Flexbox-ряд для Pie-блоков */
.pie-row {
  display: flex;
  flex-wrap: nowrap;
  gap: 1.5rem;
  overflow-x: auto;
}

.pie-row .chart-wrapper {
  flex: 1 1 0;
  min-width: 300px;
}

.chart-wrapper {
  min-height: 400px;
}

.date-range-picker {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 0.75rem 1rem;
}

.date-range {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  position: relative;
}

.date-range label {
  font-size: 0.9rem;
  color: var(--secondary-text-color);
  white-space: nowrap;
}

.date-range input[type="date"] {
  padding: 0.5rem 1rem;
  border: 1px solid var(--text-color);
  border-radius: var(--border-radius);
  background: var(--background-color);
  color: var(--text-color);
  font-family: inherit;
  font-size: 0.9rem;
  transition: all 0.2s ease;
  cursor: pointer;
  max-width: 160px;
}

.date-range input[type="date"]::-webkit-calendar-picker-indicator {
  filter: invert(0.6);
  cursor: pointer;
}

.date-range input[type="date"]:hover {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(91, 0, 225, 0.1);
}

.date-range input[type="date"]:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(91, 0, 225, 0.2);
}

/* Обработка ошибок */
.date-range.error input[type="date"] {
  border-color: #ff4444;
  background: rgba(255, 68, 68, 0.05);
}

.kpi-loading,
.kpi-error {
  grid-column: 1 / -1;
  text-align: center;
  color: #666;
}

.kpi-error button {
  margin-top: 0.5rem;
}
</style>
