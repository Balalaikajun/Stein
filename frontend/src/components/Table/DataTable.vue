 <template>
    <div class="table-container" ref="containerRef" @scroll="onScroll">
      <table class="data-table">
        <TableHeader
            :columns="columns"
            :sort-by="sortBy"
            :sort-descending="sortDescending"
            :editable="editable"
            @sort="handleSort"
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
  </template>

<script setup>
import { toRefs, ref, watch, nextTick } from 'vue'
import TableHeader from '@/components/Table/TableHeader.vue'
import TableBody from '@/components/Table/TableBody.vue'

const props = defineProps({
  columns: Array,
  items: Array,
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
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.data-table {
  border-collapse: collapse;
  min-width: 600px;
}

.data-table thead th {
  background: var(--background-color);
  border-bottom: 1px solid var(--secondary-background-color);
  z-index: 5;
}
</style>

