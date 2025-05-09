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
                :labels="pie.labels"
                :values="pie.values"
                :colors="pie.colors"
                :title="pie.title"
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
                        @change="handleDateChange"
                    >
                  </div>
                  <div class="date-range">
                    <label>По:</label>
                    <input
                        type="date"
                        v-model="ordersFilters.endDate"
                        :min="ordersFilters.startDate"
                        @change="handleDateChange"
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
import { computed, ref } from 'vue'
import KPICard from '@/components/Charts/KPICard.vue'
import menuItems from '@/router/menuData.js'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import Pie from '@/components/Charts/Pie.vue'
import Trend from '@/components/Charts/Trend.vue'
import OrdersHistogram from '@/components/Charts/OrdersHistogram.vue'
import { OrderTypes } from '@/utils/OrderTypes.js'
import SpecializationHistogram from '@/components/Charts/SpecializationHistogram.vue'

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

const pies = ref([
  {
    labels: ['Мужчины', 'Женщины'],
    values: [68, 32],
    colors: ['#5B00E1', '#7C4DFF'],
    title: 'Распределение по полу'
  },
  {
    labels: ['17-20', '21-25', '26+'],
    values: [45, 50, 5],
    colors: ['#5B00E1', '#7C4DFF', '#FFD740'],
    title: 'Возрастные группы'
  },
  {
    labels: ['РФ', 'СНГ', 'Другое'],
    values: [85, 12, 3],
    colors: ['#5B00E1', '#7C4DFF', '#FFD740'],
    title: 'Гражданство'
  }
])

const performance = ref({
  labels: ['Сентябрь', 'Октябрь'],
  values: [82.5, 78.3],
  performanceData: [
    { Excellent: 12, Good: 18, Normal: 5, Falling: 2 },
    { Excellent: 10, Good: 20, Normal: 7, Falling: 3 }
  ]
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

const handleDateChange = async () => {
  try {
    const params = new URLSearchParams({
      start: ordersFilters.value.startDate,
      end: ordersFilters.value.endDate
    })

  } catch (error) {
    console.error('Ошибка загрузки:', error)
  }
}
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