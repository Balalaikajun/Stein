<template>
  <div class="table-container" ref="containerRef" @scroll="onScroll">
    <table class="data-table" ref="tableRef">
      <thead>
      <tr>
        <th
            v-for="col in columns"
            :key="col.key"
            :style="{ width: col.width || 'auto' }"
            :class="{
              sortable: editable && !col.disableSort,
              sorted: sortBy === col.sortKey || sortBy === col.key,
              'text-center': col.align === 'center',
              'text-right': col.align === 'right'
            }"
            @click="$emit('sort', col.sortKey || col.key)"
        >
          <div class="th-content">
            <span class="title-text">{{ col.title }}</span>
            <span class="sort-icon-wrapper">
                <span
                    v-if="(sortBy === col.sortKey || sortBy === col.key) && !col.disableSort"
                    class="sort-icon"
                >{{ sortDescending ? '▲' : '▼' }}</span>
                <span
                    v-else-if="!col.disableSort"
                    class="sort-icon-placeholder"
                ></span>
              </span>
          </div>
        </th>
      </tr>
      </thead>

      <tbody ref="bodyRef">
      <tr v-for="item in items" :key="item.id">
        <td
            v-for="col in columns"
            :key="col.key"
            :style="{ width: col.width || 'auto' }"
            :class="{
              'text-center': col.align === 'center',
              'text-right': col.align === 'right'
            }"
        >
          <slot :name="`cell-${col.key}`" :item="item">
            {{ col.formatter ? col.formatter(item[col.key]) : item[col.key] }}
          </slot>
        </td>
      </tr>
      <tr v-if="!items.length && !loading">
        <td :colspan="columns.length" class="no-data">
          Нет данных для отображения
        </td>
      </tr>
      <tr v-if="loading">
        <td :colspan="columns.length" class="loading-indicator">
          Загрузка данных...
        </td>
      </tr>
      </tbody>

      <tfoot ref="footerRef">
      <tr>
        <td :colspan="columns.length" class="footer-cell">
          Элементов: {{ total?.value ?? total }}
        </td>
      </tr>
      </tfoot>
    </table>
  </div>
</template>

<script setup>
import { ref, toRefs, watch, nextTick, onMounted } from 'vue'

const props = defineProps({
  columns: Array,
  items: Array,
  total: [Number, Object],
  loading: Boolean,
  hasMore: Boolean,
  sortBy: String,
  sortDescending: Boolean,
  editable: Boolean
})
const emit = defineEmits(['sort', 'load-more'])

const { loading, hasMore, items } = toRefs(props)
const containerRef = ref(null)
const bodyRef = ref(null)
const tableRef = ref(null)

// Обработчик скролла: подгрузка при приближении к низу
const onScroll = () => {
  const container = containerRef.value
  if (!loading.value && hasMore.value &&
      container.scrollHeight - container.scrollTop <= container.clientHeight + 50) {
    emit('load-more')
  }
}

// При изменении items — сброс скролла и докачка, если нужно
watch(items, (newItems, oldItems) => {
  nextTick(() => {
    const container = containerRef.value
    if (!container) return

    if (oldItems && newItems.length < oldItems.length) {
      container.scrollTop = 0
    }
    if (!loading.value && hasMore.value && container.scrollHeight <= container.clientHeight) {
      emit('load-more')
    }
  })
})

onMounted(() => {
  // если нужно докачать сразу при монтировании
  nextTick(() => {
    const container = containerRef.value
    if (container && !loading.value && hasMore.value &&
        container.scrollHeight <= container.clientHeight) {
      emit('load-more')
    }
  })
})
</script>

<style scoped>
.table-container {
  height: 100%;
  overflow-y: auto;
  background: var(--background-color);
  border-radius: var(--border-radius);
  border: 1px solid var(--secondary-background-color);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  table-layout: fixed;
}

/* Шапка */
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

/* Тело */
.data-table tbody td {
  padding: 12px 16px;
  border-bottom: 1px solid var(--secondary-background-color);
  background: var(--background-color);
  vertical-align: top;
}

/* Футер */
.data-table tfoot td {
  position: sticky;
  bottom: 0;
  background: var(--background-color);
  border-top: 2px solid var(--secondary-background-color);
  padding: 12px 16px;
  font-size: 0.9em;
  color: var(--secondary-text-color);
  text-align: right;
  z-index: 2;
}

/* Ховер и состояния */
.data-table tbody tr:hover td {
  background-color: var(--hover-color);
}
.sortable { cursor: pointer; transition: background var(--transition-duration) var(--transition-timing); }
.sortable:hover { background-color: var(--hover-color); }
.sorted { background-color: var(--active-bg-color); }

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
