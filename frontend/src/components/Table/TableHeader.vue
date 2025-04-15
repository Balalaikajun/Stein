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
/* Гарантируем, что header-строка ведёт себя корректно */
thead {
  display: table-header-group;
}

th {
  padding: 12px 16px;
  border-bottom: 1px solid #e0e0e0;
  text-align: left;
  background-color: #f8f9fa;
  font-weight: 600;
  color: #424242;
  position: sticky;
  top: 0;
  z-index: 3;
}

.sortable {
  cursor: pointer;
  transition: background 0.2s;
}
.sortable:hover {
  background-color: #f0f0f0;
}
.sorted {
  background-color: #ececec;
}
.th-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
}
.sort-icon-wrapper {
  width: 24px;
  text-align: center;
}
.title-text {
  flex: 1;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
.sort-icon {
  color: #1976d2;
}
.sort-icon-placeholder {
  width: 16px;
  display: inline-block;
}
.text-center { text-align: center; }
.text-right { text-align: right; }
</style>