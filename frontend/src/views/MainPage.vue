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
        <!-- ROW 1: Pie‐графики -->
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
                  :defaultColor="['#5B00E1', '#7C4DFF', '#FFD740']"
              />
            </div>
          </template>
        </div>

        <!-- ROW 2: PerformanceHistogram -->
        <div class="chart-row">
          <div class="chart-wrapper">
            <PerformanceHistogram
                :data="performanceData"
                :loading="performanceLoading"
            >
              <template #controls>
                <div class="performance-controls-simple">
                  <button
                      @click="prevPage"
                      class="pagination-btn-simple"
                      :disabled="!hasBefore"
                  >
                    Назад
                  </button>
                  <span class="date-range-simple">
                    {{ monthFrom }}/{{ yearFrom }} – {{ monthTo }}/{{ yearTo }}
                  </span>
                  <button
                      @click="nextPage"
                      class="pagination-btn-simple"
                      :disabled="!hasAfter"
                  >
                    Вперёд
                  </button>
                </div>
              </template>
            </PerformanceHistogram>
          </div>
        </div>

        <!-- ROW 3: OrdersHistogram -->
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
              Ошибка загрузки приказов.
              <button @click="fetchOrders">Повторить</button>
            </div>
            <div v-else class="loading-placeholder">
              Загрузка данных приказов...
            </div>
          </div>
        </div>

        <!-- ROW 4: ContingentHistogram -->
        <div class="chart-row">
          <div class="chart-wrapper">
            <!-- 1) Когда есть данные (contingentData.data) → рендерим сам компонент -->
            <ContingentHistogram
                v-if="contingentData?.data"
                :data="contingentData.data"
                :total-orders="contingentData.count"
                type="Контингент"
            >
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

            <!-- 2) Пока идёт загрузка -->
            <div v-else-if="contingentLoading" class="loading-placeholder">
              Загрузка данных контингента...
            </div>

            <!-- 3) Если упала ошибка -->
            <div v-else-if="contingentError" class="error-placeholder">
              Ошибка загрузки контингента.
              <button @click="fetchContingent">Повторить</button>
            </div>

            <!-- 4) На всякий случай: дефолтный плэйсхолдер (если ни одна из веток выше не сработала) -->
            <div v-else class="loading-placeholder">
              Загрузка данных контингента...
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import KpiCard from '@/components/Charts/KpiCard.vue'
import menuItems from '@/router/menuData.js'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import Pie from '@/components/Charts/Pie.vue'
import OrdersHistogram from '@/components/Charts/OrdersHistogram.vue'
import PerformanceHistogram from '@/components/Charts/PerformanceHistogram.vue'
import ContingentHistogram from '@/components/Charts/ContingentHistogram.vue'
import FiltersContainer from '@/components/Filters/FiltersContainer.vue'

import { useAllKpis } from '@/composables/useKpiCards.js'
import { usePieCards } from '@/composables/usePieCards.js'
import { usePerformanceHistogram } from '@/composables/usePerformanceHistogram.js'
import { useOrdersHistogram } from '@/composables/useOrdersHistogram.js'
import { useContingentHistogram } from '@/composables/useContingentHistogram.js'

// 1. РЕАКТИВНЫЕ ФИЛЬТРЫ — единый объект, за которым будут «следить» все хуки
const filters = ref({
  isFullTime: null,
  departmentIds: [],
  SpecializationIds: [],
  GroupKeys: []
})

// Конфигурация для FiltersContainer (UI‐настройки)
const filtersConfig = [
  {
    id: 'departmentIds',
    title: 'Кафедры',
    dataType: 'lookup',
    apiEndpoint: '/api/Department/filter',
    params: { take: 15, activeFilter: true, sortBy: 'Title', descending: false },
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
    dependsOn: ['departmentIds'],
    dependentParams: { DepartmentIds: 'departmentIds' },
    params: { take: 15 },
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
    dependsOn: ['departmentIds', 'SpecializationIds'],
    dependentParams: { DepartmentIds: 'departmentIds', SpecializationIds: 'specializationIds' },
    params: { take: 15 },
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
    id: 'isFullTime',
    title: 'Очно/заочно',
    dataType: 'radio',
    staticOptions: [
      { label: 'Очное отделение', value: true },
      { label: 'Заочное отделение', value: false },
      {label: 'Все', value: null },
    ],
    allowDeselect: true
  }
]

// ========== KPI ========== //
const {
  kpis,
  kpiIsLoading,
  kpiError,
  fetchAllKpis
} = useAllKpis()

const kpiNamingMapping = {
  Students: 'Число студентов',
  Orders: 'Приказы за месяц',
  Foreigners: 'Иностранные граждане'
}

const kpiList = computed(() => {
  return Object.entries(kpis.value).map(([type, data]) => ({
    title: kpiNamingMapping[type] || type,
    value: data ? data.count : '-',
    trend: data ? data.trend : null
  }))
})

// ========== PIE ========== //
const pieTypes = ref(['Gender', 'AgeGroup', 'Nationality'])
const pieNameMapping = {
  gender: 'Распределение по полам',
  agegroup: 'Распределение во возрастам',
  nationality: 'Распределение по гражданству'
}
const piesValuesMapping = {
  Gender: {
    Male: 'Мужской',
    Female: 'Женский'
  },
  Nationality: {
    False: 'РФ',
    True: 'Иное'
  }
}

// Обратите внимание: передаём именно filters (Ref), а не filters.value
const {
  pies,
  pieIsLoading,
  pieError,
  fetchAllPies
} = usePieCards(pieTypes, piesValuesMapping, filters)

// ========== PERFORMANCE ========== //
// Передаём filters (Ref), внутри хук сам «следит» и перезагружает данные
const {
  data: performanceData,
  loading: performanceLoading,
  prevPage,
  nextPage,
  hasBefore,
  hasAfter,
  yearFrom,
  monthFrom,
  yearTo,
  monthTo,
  reload: fetchPage
} = usePerformanceHistogram(filters)

// ========== ORDERS ========== //
const ordersFilters = ref({
  startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 2)
      .toISOString().split('T')[0],
  endDate: new Date().toISOString().split('T')[0]
})

const ordersRequestBody = computed(() => ({
  dateRange: {
    fromDate: ordersFilters.value.startDate,
    toDate: ordersFilters.value.endDate
  },
  // Дополнительно добавляем текущие департаменты/специализации/группы
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

// ========== CONTINGENT ========== //
const contingentFilters = ref({
  date: new Date().toISOString().split('T')[0]
})

const contingentRequestBody = computed(() => ({
  date: contingentFilters.value.date,
  isFullTime: filters.value.isFullTime
}))

const {
  data: contingentData,
  loading: contingentLoading,
  error: contingentError,
  fetchContingent
} = useContingentHistogram(contingentRequestBody)

// ========== ФУНКЦИЯ для обработки смены фильтров ========== //
// При получении нового набора фильтров просто перезаписываем filters.value.
// Все хуки, «подписанные» на filters, автоматически перезагрузят свои данные.
function onFilterChange(newFilter) {
  filters.value = { ...newFilter }
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

.date-range input[type="date"].error {
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

.performance-controls-simple {
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--background-color);
  border: none;
  border-radius: var(--border-radius);
  padding: 0.5rem 1rem;
  gap: 1rem;
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
  box-shadow: 0 0 0 2px rgba(91, 0, 225, 0.1);
}

.pagination-btn-simple:disabled {
  background-color: #f5f5f5;
  color: #a0a0a0;
  cursor: not-allowed;
  opacity: 0.6;
}
</style>
