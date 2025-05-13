<template>
  <div class="main-layout">
    <Sidebar :items="menuItems"/>

    <div class="content-area">
      <div class="dashboard-header">
        <h1>Главная</h1>
      </div>

      <div class="kpi-container">
        <KPICard
            v-for="(kpi, index) in kpis"
            :key="index"
            :title="kpi.title"
            :value="kpi.value"
            :trend="kpi.trend"
        />
      </div>

      <div class="charts-container">


        <div class="chart-row pie-row">
          <div
              class="chart-wrapper"
              v-for="(pie, index) in pies"
              :key="index"
          >
            <Pie
                :pie="pie"
                :show-legend="true"
                :animate="true"
            />
          </div>
        </div>


        <div class="chart-row">
          <div class="chart-wrapper">
            <Trend
                v-if="performance.labels?.length"
                :labels="performance.labels"
                :values="performance.values"
                :performance-data="performance.performanceData"
                :page="performanceFilter.page"
                :total-pages="performanceFilter.totalPages"
                @update:page="onPerformancePageChange"
            />
            <div v-else class="loading-placeholder">
              Загрузка данных...
            </div>
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
            <SpecializationHistogram
                :data="specialtiesData"
                title="Распределение студентов по специальностям и курсам на данный момент"
            />
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import KPICard from '@/components/Charts/KPICard.vue'
import menuItems from '@/router/menuData.js'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import Pie from '@/components/Charts/Pie.vue'
import Trend from '@/components/Charts/Trend.vue'
import OrdersHistogram from '@/components/Charts/OrdersHistogram.vue'
import SpecializationHistogram from '@/components/Charts/SpecializationHistogram.vue'
import axios from 'axios'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

const kpis = ref({
  students: {
    title: 'Число студентов',
    value: '2 456',
  },
  orders: {
    title: 'Приказы за месяц',
    value: 42,
  },
  foreign: {
    title: 'Иностранные граждане',
    value: 183,
  }
})

const pies = ref({
  gender: {
    title: 'Распределение по полу',
    segments: []
  },
  age: {
    title: 'Возрастные группы',
    segments: []
  },
  citizenship: {
    title: 'Гражданство',
    segments: []
  }
})
const piesMap ={
  genderLabelMap: {
    Male: 'Мужчины',
    Female: 'Женщины'
  },
  citizenshipLabelMap:{
    True: 'Иностранцы',
    False: 'Граждане РФ'
  }
}

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
  },
  {
    name: 'Физика',
    total: 75,
    courses: {
      '1': 20,
      '2': 18,
      '3': 22,
      '4': 15
    }
  },
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
  },
  {
    name: 'Физика',
    total: 75,
    courses: {
      '1': 20,
      '2': 18,
      '3': 22,
      '4': 15
    }
  },
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
  },
  {
    name: 'Физика',
    total: 75,
    courses: {
      '1': 20,
      '2': 18,
      '3': 22,
      '4': 15
    }
  },
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
  },
  {
    name: 'Физика',
    total: 75,
    courses: {
      '1': 20,
      '2': 18,
      '3': 22,
      '4': 15
    }
  },
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
  },
  {
    name: 'Физика',
    total: 75,
    courses: {
      '1': 20,
      '2': 18,
      '3': 22,
      '4': 15
    }
  },
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
  },
  {
    name: 'Физика',
    total: 75,
    courses: {
      '1': 20,
      '2': 18,
      '3': 22,
      '4': 15
    }
  }
])

const defaultColors = ['#5B00E1', '#7C4DFF', '#FFD740', '#00B8D4', '#FF6D00']

const fetchOrdersData = async () => {
  try {
    const params = new URLSearchParams({
      start: ordersFilters.value.startDate,
      end: ordersFilters.value.endDate
    })

  } catch (error) {
    console.error('Ошибка загрузки:', error)
  }
}

function mapSegments(segments, labelMap = {}) {
  return segments.map((s, index) => ({
    label: labelMap[s.label] ?? s.label,
    value: s.value,
    color: defaultColors[index % defaultColors.length]
  }))
}

async function fetchKPIs () {
  try {
    const data = (await axios.get(`${BACKEND_API_HOST}/api/Metrics/counts`)).data

    kpis.value.students.value = data.students.count
    kpis.value.orders.value = data.orders.count
    kpis.value.foreign.value = data.foreign.count

  } catch (error) {
    console.log(error)
  }
}

async function fetchPies () {
  try {
    const { data } = await axios.get(`${BACKEND_API_HOST}/api/Metrics/pies`)

    pies.value.gender.segments = mapSegments(data.gender.segments, piesMap.genderLabelMap)
    pies.value.age.segments = mapSegments(data.age.segments)
    pies.value.citizenship.segments = mapSegments(data.citizenship.segments, piesMap.citizenshipLabelMap)

  } catch (error) {
    console.error('Ошибка при загрузке пай-чартов:', error)
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
    { Excellent: 12, Good: 18, Normal: 5, Falling: 2 },
  ]
  // Срез по странице:
  const start = (p - 1) * performanceFilter.value.pageSize
  const end = start + performanceFilter.value.pageSize
  performance.value.labels = allLabels.slice(start, end)
  performance.value.values = allValues.slice(start, end)
  performance.value.performanceData = allPerf.slice(start, end)
  // Обновляем общее количество страниц
  performanceFilter.value.totalPages = Math.ceil(allLabels.length / performanceFilter.value.pageSize)
}

function onPerformancePageChange (newPage) {
  console.log('fetchPerformance, page =', p)
  performanceFilter.value.page = newPage
  fetchPerformance(newPage)
}

onMounted(() => {
  fetchPerformance(performanceFilter.value.page)
  fetchKPIs()
  fetchPies()
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


</style>