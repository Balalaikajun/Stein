<template>
  <div class="main-layout">
    <Sidebar :items="menuItems" />

    <div class="content-area">
      <div class="dashboard-header">
        <h1>Главная</h1>
        <FiltersContainer
            :filters="filtersConfig"
            :values="filters"
            @update-filters="onFilterChange"
        />
      </div>

      <div class="kpi-container">
        <div v-if="kpiIsLoading" class="kpi-loading">Загрузка KPI…</div>
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
        <div class="chart-row pie-row">
          <div v-if="pieIsLoading" class="chart-wrapper">Загрузка Pie‐данных…</div>
          <div v-else-if="pieError" class="chart-wrapper error">
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
                    :data="performance.data.value"
                    :loading="performance.loading"
                >
                  <template #controls>
                    <div class="performance-controls-simple">
                      <button
                          @click="performance.prevPage"
                          class="pagination-btn-simple"
                          :disabled="!performance.hasBefore"
                      >
                        Назад
                      </button>
                      <span class="date-range-simple">
                    {{ performance.monthFrom }}/{{ performance.yearFrom }}
                    – {{ performance.monthTo }}/{{ performance.yearTo }}
                  </span>
                      <button
                          @click="performance.nextPage"
                          class="pagination-btn-simple"
                          :disabled="!performance.hasAfter"
                      >
                        Вперёд
                      </button>
                    </div>
                  </template>
                </PerformanceHistogram>
              </div>
            </div>

        <div class="chart-row">
          <div class="chart-wrapper">
            <OrdersHistogram
                v-if="ordersData?.data"
                :order-data="ordersData.data"
                :total-orders="ordersData.count"
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
                    />
                  </div>
                  <div class="date-range">
                    <label>По:</label>
                    <input
                        type="date"
                        v-model="ordersFilters.endDate"
                        :min="ordersFilters.startDate"
                    />
                  </div>
                </div>
              </template>
            </OrdersHistogram>
            <div v-else-if="ordersLoading" class="loading-placeholder">
              Загрузка данных приказов...
            </div>
            <div v-else-if="ordersError" class="error-placeholder">
              Ошибка загрузки приказов. <button @click="fetchOrders">Повторить</button>
            </div>
            <div v-else class="loading-placeholder">Загрузка данных приказов...</div>
          </div>
        </div>

        <div class="chart-row">
          <div class="chart-wrapper">
            <!-- Передаём в компонент нужные пропсы и вставляем слот controls -->
            <ContingentHistogram
                :data="contingentData?.data"
                :total-orders="contingentData.count"
                type="Контингент">
              <template #controls>
                <div class="date-range-picker">
                  <div class="date-range">
                    <label>На дату:</label>
                    <input
                        type="date"
                        v-model="contingentFilters.date"
                        :max="new Date().toISOString().split('T')[0]"
                        class="contingent-date-input"
                    />
                  </div>
                </div>
              </template>
            </ContingentHistogram>
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
import { usePerformanceHistogram } from '@/composables/usePerformanceHistogram.js'
import { useOrdersHistogram } from '@/composables/useOrdersHistogram.js'
import {useContingentHistogram} from '@/composables/useContingentHistogram.js'

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
  {
    id: 'isFulltime',
    title: 'Очно/заочно',
    dataType: 'radio',
    staticOptions: [
      { label: 'Очное отделение', value: true },
      { label: 'Заочное отделение', value: false }
    ],
    allowDeselect: true,
  }
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
const piesValuesMapping = {
  Gender: {
    Male:   'Мужской',
    Female: 'Женский'
  },
  Nationality: {
    False:   "Рф",
    True:"Иное"
  }
};
const {
  pies,
  pieIsLoading,
  pieError,
  fetchAllPies
} = usePieCards(pieTypes,piesValuesMapping, filters.value)


const performance = usePerformanceHistogram(filters)

// Фильтры для приказов
const ordersFilters = ref({
  startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 2)
      .toISOString().split('T')[0],
  endDate: new Date().toISOString().split('T')[0]
})

// Собираем тело запроса реактивно
const ordersRequestBody = computed(() => ({
  dateRange:{
    fromDate: ordersFilters.value.startDate,
    toDate: ordersFilters.value.endDate
  },
  // Пример: можно добавить дополнительные фильтры
  departments: filters.value.departmentIds ?? null,
  specializations: filters.value.SpecializationIds ?? null,
  groups: filters.value.GroupKeys ?? null
}))

const {
  data: ordersData,
  loading: ordersLoading,
  error: ordersError,
  fetchOrders
} = useOrdersHistogram(ordersRequestBody)

const contingentFilters = ref({
  date: new Date().toISOString().split('T')[0]
})

// Собираем тело запроса реактивно
const contingentRequestBody = computed(() => ({
  date: contingentFilters.value.date,
  // Пример: можно добавить дополнительные фильтры
  isFullTime:filters.value.isFullTime ?? null
}))

const {
  data: contingentData,
  loading: contingentLoading,
  error: contingentError,
  fetchContingent
} = useContingentHistogram(contingentRequestBody)

function onFilterChange (newFilter) {



  fetchAllPies(newFilter)
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

.kpi-loading,
.kpi-error {
  grid-column: 1 / -1;
  text-align: center;
  color: #666;
}

.kpi-error button {
  margin-top: 0.5rem;
}


/* === Упрощённый стиль блока успеваемости === */
.performance-controls-simple {
  display: flex;
  align-items: center;
  justify-content: center; /* выровнять элементы по центру */
  background: var(--background-color);
  border: none;              /* Убрали рамку */
  border-radius: var(--border-radius);
  padding: 0.5rem 1rem;
  gap: 1rem;                 /* расстояние между кнопками и датой */
}

.date-range-simple {
  color: var(--text-color);
  font-size: 0.95rem;
  font-weight: 500;
}

.pagination-btn-simple {
  padding: 0.4rem 0.8rem;
  border: 1px solid var(--text-color);
  border-radius: var(--border-radius);
  background: var(--background-color);
  color: var(--text-color);
  font-size: 0.9rem;
  cursor: pointer;
  transition: background-color 0.15s ease;
}

.pagination-btn-simple:hover:not(:disabled) {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(91, 0, 225, 0.1); /* чуть светлее, без тени */
}

.pagination-btn-simple:disabled {
  background-color: #f5f5f5;
  color: #a0a0a0;
  cursor: not-allowed;
  opacity: 0.6;
}

</style>
