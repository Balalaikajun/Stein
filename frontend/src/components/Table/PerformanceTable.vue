<template>
  <div class="table-container" ref="containerRef" @scroll="onScroll">
    <table class="data-table" ref="tableRef">
      <thead>
      <tr>
        <!-- Student Columns -->
        <th
            v-for="(col, index) in studentColumns"
            :key="col.key"
            :style="{ width: col.width || 'auto' }"
            :class="[
              { 'sticky-col': index === 0 },
              'sortable',
              { sorted: isSorted(col), disabled: col.disableSort || !editable },
              { 'text-center': col.align === 'center', 'text-right': col.align === 'right' }
            ]"
            @click="onSort(col)"
        >
          <div class="th-content">
            <span class="title-text">{{ col.title }}</span>
            <span class="sort-icon-wrapper">
                <span v-if="isSorted(col) && !col.disableSort" class="sort-icon">
                  {{ sortDescending ? '▲' : '▼' }}
                </span>
                <span v-else-if="!col.disableSort" class="sort-icon-placeholder"></span>
              </span>
          </div>
        </th>
        <!-- Month Columns -->
        <th
            v-for="month in monthColumns"
            :key="month.key"
            :style="{ width: monthWidth }"
            class="month-column"
        >
          {{ month.title }}
        </th>
      </tr>
      </thead>

      <tbody ref="bodyRef">
      <tr v-for="student in students" :key="student.id">
        <!-- Student Data -->
        <td
            v-for="(col, index) in studentColumns"
            :key="col.key"
            :style="{ width: col.width || 'auto' }"
            :class="{
              'sticky-col': index === 0,
              'text-center': col.align === 'center',
              'text-right': col.align === 'right'
            }"
        >
          <slot :name="`student-cell-${col.key}`" :student="student">
            {{ col.formatter ? col.formatter(student) : student[col.key] }}
          </slot>
        </td>
        <!-- Performance Data -->
        <td
            v-for="month in monthColumns"
            :key="month.key"
            class="text-center month-column"
            :style="{ width: monthWidth }"
        >
          <slot :name="`month-cell-${month.key}`" :performance="getPerformance(student.id, month.key)">
            {{ getPerformance(student.id, month.key) }}
          </slot>
        </td>
      </tr>
      <tr v-if="!students.length && !loading">
        <td :colspan="studentColumns.length + monthColumns.length" class="no-data">
          Нет данных для отображения
        </td>
      </tr>
      <tr v-if="loading">
        <td :colspan="studentColumns.length + monthColumns.length" class="loading-indicator">
          Загрузка данных...
        </td>
      </tr>
      </tbody>


    </table>
  </div>
</template>


<script setup>
import { ref, computed, toRefs, watch, nextTick, onMounted } from 'vue'

const props = defineProps({
  studentColumns: { type: Array, required: true },
  students: { type: Array, default: () => [] },
  performanceItems: { type: Array, default: () => [] },
  total: [Number, Object],
  loading: Boolean,
  hasMore: Boolean,
  hasMoreMonths: Boolean,
  sortBy: String,
  sortDescending: Boolean,
  editable: Boolean,
  monthWidth: { type: String, default: '120px' }
})

const emit = defineEmits(['sort', 'load-more', 'load-more-months'])
const { loading, hasMore, hasMoreMonths } = toRefs(props)
const containerRef = ref(null)
const bodyRef = ref(null)
const tableRef = ref(null)

const performanceMap = computed(() =>
    props.performanceItems.reduce((map, item) => {
      const key = `${item.year}-${String(item.month).padStart(2, '0')}`
      if (!map[item.studentId]) map[item.studentId] = {}
      map[item.studentId][key] = item.performance
      return map
    }, {})
)

const getPerformance = (studentId, monthKey) =>
    performanceMap.value[studentId]?.[monthKey] ?? '-'

const monthColumns = computed(() => {
  const months = new Map()
  props.performanceItems.forEach(item => {
    const key = `${item.year}-${String(item.month).padStart(2, '0')}`
    if (!months.has(key)) {
      months.set(key, { key, title: `${item.year}.${item.month}` })
    }
  })
  return Array.from(months.values()).sort((a, b) => a.key.localeCompare(b.key))
})

const isSorted = col => props.sortBy === (col.sortKey || col.key)
const onSort = col => {
  if (!props.editable || col.disableSort) return
  emit('sort', col.sortKey || col.key)
}

const onScroll = () => {
  const c = containerRef.value
  if (!c) return
  const { scrollTop, scrollHeight, clientHeight, scrollLeft, scrollWidth, clientWidth } = c
  const threshold = 100
  if (scrollHeight - scrollTop <= clientHeight + threshold && hasMore.value) emit('load-more')
  if (scrollWidth - scrollLeft <= clientWidth + threshold && hasMoreMonths.value) emit('load-more-months')
}

watch(() => props.students, (newItems, oldItems) => {
  nextTick(() => {
    const c = containerRef.value
    if (!c) return
    if (oldItems && newItems.length < oldItems.length) c.scrollTop = 0
    if (!props.loading && props.hasMore && c.scrollHeight <= c.clientHeight) emit('load-more')
  })
})

onMounted(() => {
  nextTick(() => {
    const c = containerRef.value
    if (c && !props.loading && props.hasMore && c.scrollHeight <= c.clientHeight) emit('load-more')
  })
})
</script>

<style scoped>
.table-container {
  height: 100%;
  overflow: auto;
  background: var(--background-color);
  border-radius: var(--border-radius);
  border: 1px solid var(--secondary-background-color);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.data-table {
  width: max-content;
  border-collapse: collapse;
  table-layout: fixed;
  white-space: nowrap;
}

/* Зафиксированный первый столбец (заголовок) */
.data-table thead th.sticky-col {
  position: sticky;
  left: 0;
  background: var(--background-color);
  z-index: 3;
  /* При желании можно добавить бордер справа, чтобы отделить от остальных столбцов */
  border-right: 1px solid var(--secondary-background-color);
}

/* Зафиксированные ячейки первого столбца */
.data-table tbody td.sticky-col {
  position: sticky;
  left: 0;
  background: var(--background-color);
  z-index: 1;
  /* Бордер справа (необязательно, но визуально разграничивает) */
  border-right: 1px solid var(--secondary-background-color);
}

.data-table thead th {
  position: sticky;
  top: 0;
  background: var(--background-color);
  border-bottom: 2px solid var(--secondary-background-color);
  padding: 12px 16px;
  font-weight: 600;
  color: var(--text-color);
  z-index: 2;
}

.data-table tbody td {
  padding: 12px 16px;
  border-bottom: 1px solid var(--secondary-background-color);
  background: var(--background-color);
  vertical-align: top;
}

.data-table tfoot td {
  position: sticky;
  bottom: 0;
  background: var(--background-color);
  border-top: 2px solid var(--secondary-background-color);
  padding: 12px 16px;
  font-size: 0.9em;
  color: var(--secondary-text-color);
  text-align: left;
  z-index: 2;
}

.month-column { text-align: center; }

.data-table tbody tr:hover td {
  background-color: var(--hover-color);
}
.sortable { cursor: pointer; transition: background var(--transition-duration) var(--transition-timing); }
.sortable:hover { background-color: var(--hover-color); }
.sorted { background-color: var(--active-bg-color); }
.disabled { cursor: default; opacity: 0.6; }

.th-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.5rem;
}

.no-data, .loading-indicator {
  text-align: center;
  padding: 2rem !important;
  color: var(--secondary-text-color);
}

@media (max-width: 768px) {
  .data-table th,
  .data-table td {
    padding: 8px 12px;
    font-size: 0.85em;
  }
}
</style>
