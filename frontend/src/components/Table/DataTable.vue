<template>
  <div class="table-container" ref="containerRef" @scroll="onScroll">
    <div class=table-wrapper>

      <table class="data-table">
        <TableHeader
            :columns="columns"
            :sort-by="sortBy"
            :sort-descending="sortDescending"
            :editable="editable"
            @sort="handleSort"
            class="table-header"
        />
        <TableBody
            :columns="columns"
            :items="items"
            :loading="loading"
            :has-more="hasMore"
            :slots="$slots"
        />
      </table>
    </div>
    <tfoot class="table-footer">
    <tr>
      <td :colspan="columns.length" class="footer-cell" :data-count="items.length">
        Элементов: {{ total?.value ?? total }}
      </td>
    </tr>
    </tfoot>
  </div>
</template>

<script setup>
import { nextTick, ref, toRefs, watch } from 'vue'
import TableHeader from '@/components/Table/TableHeader.vue'
import TableBody from '@/components/Table/TableBody.vue'

const props = defineProps({
  columns: Array,
  items: Array,
  total: Object,
  loading: Boolean,
  hasMore: Boolean,
  sortBy: String,
  sortDescending: Boolean,
  editable: Boolean
})
const emit = defineEmits(['sort', 'load-more'])

const { loading, hasMore, items } = toRefs(props)
const containerRef = ref(null)

// Обработчик скролла: подгрузка при приближении к низу
const onScroll = () => {
  const container = containerRef.value
  if (!loading.value && hasMore.value &&
      container.scrollHeight - container.scrollTop <= container.clientHeight + 50) {
    emit('load-more')
  }
}

// Следим за изменением items: при сбросе — прокрутить вверх, а при заполнении недостаточно элементов — докачать
watch(items, (newItems, oldItems) => {
  nextTick(() => {
    const container = containerRef.value
    if (!container) return

    // Если уменьшилось количество элементов (сброс фильтров/сортировки) — сброс скролла
    if (oldItems && newItems.length < oldItems.length) {
      container.scrollTop = 0
    }

    // Если после загрузки элементов контейнер не прокручивается и есть что докачать — автоматически подгрузить ещё
    if (!loading.value && hasMore.value && container.scrollHeight <= container.clientHeight) {
      emit('load-more')
    }
  })
})

const handleSort = (key) => {
  emit('sort', key)
}
</script>

<style scoped>
.table-container {
  background: var(--background-color);
  border-radius: var(--border-radius);
  border: 1px solid var(--secondary-background-color);
  overflow: auto;
  max-height: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}


.data-table {
  width: 100%;
  min-width: 600px;
  border-collapse: collapse;
  table-layout: fixed;
}

/* Стили для хедера */
.table-header :deep(th) {
  padding: 12px 16px;
  text-align: left;
  font-weight: 600;
  color: var(--text-color);
  background: var(--background-color);
  border-bottom: 2px solid var(--secondary-background-color);
  position: sticky;
  top: 0;
  z-index: 2;
}

/* Стили для тела таблицы */
.data-table :deep(td) {
  padding: 12px 16px;
  border-bottom: 1px solid var(--secondary-background-color);
  color: var(--text-color);
  vertical-align: top;
  background: var(--background-color);
}

/* Стили для футера */
.data-table tfoot td {
  width: 100%;
  min-width: 600px;
  border-collapse: collapse;
  table-layout: fixed;
  position: sticky;
  bottom: 0;
  background: transparent;
  z-index: 2;
  border-top: 2px solid var(--secondary-background-color);
  padding: 12px 16px;
  font-size: 0.9em;
  color: var(--secondary-text-color);
  text-align: right;
}

/* Состояния при наведении */
.data-table :deep(tr:hover td) {
  background-color: var(--hover-color);
}


/* Адаптивность */
@media (max-width: 768px) {
  .data-table {
    min-width: 100%;
  }

  .table-header :deep(th),
  .data-table :deep(td),
  .data-table tfoot td {
    padding: 8px 12px;
    font-size: 0.85em;
  }
}

.table-wrapper {
  flex: 1;
  overflow: auto;
}

.table-footer {
  position: sticky;
  bottom: 0;
  background: var(--background-color);
  border-top: 2px solid var(--secondary-background-color);
  z-index: 2;
}

.table-footer td {
  padding: 12px 16px;
  font-size: 0.9em;
  color: var(--secondary-text-color);
  text-align: right;
}
</style>