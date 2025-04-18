<template>
  <thead>
  <tr>
    <th
        v-for="col in columns"
        :key="col.key"
        :style="{ width: col.width || 'auto' }"
        :class="{
          'sortable': editable && !col.disableSort,
          'sorted': sortBy === col.sortKey || sortBy === col.key,
          'text-center': col.align === 'center',
          'text-right': col.align === 'right'
        }"
        @click="editable && !col.disableSort && $emit('sort', col.sortKey || col.key)"
    >
      <div class="th-content">
        <span class="title-text">{{ col.title }}</span>
        <span class="sort-icon-wrapper">
            <span
                v-if="(sortBy === col.sortKey || sortBy === col.key) && !col.disableSort"
                class="sort-icon"
            >
              {{ sortDescending ? '▲' : '▼' }}
            </span>
            <span v-else-if="!col.disableSort" class="sort-icon-placeholder"></span>
          </span>
      </div>
    </th>
  </tr>
  </thead>
</template>

<script setup>
defineProps({
  columns: Array,
  sortBy: String,
  sortDescending: Boolean,
  editable: Boolean
})
defineEmits(['sort'])
</script>

<style scoped>
th {
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--secondary-background-color);
  background-color: var(--background-color);
  color: var(--text-color);
  font-weight: 600;
}

.sortable {
  cursor: pointer;
  transition: background-color var(--transition-duration) var(--transition-timing);
}

.sortable:hover {
  background-color: var(--hover-color);
}

.sorted {
  background-color: var(--active-bg-color);
}

.th-content {
  gap: 0.5rem;
}

.sort-icon {
  color: var(--primary-color);
}

.text-center { text-align: center; }
.text-right { text-align: right; }
</style>