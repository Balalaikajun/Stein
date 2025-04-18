<template>
  <tbody>
  <tr v-for="item in items" :key="item.Id">
    <td
        v-for="col in columns"
        :key="col.key"
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
</template>

<script setup>
defineProps({ columns: Array, items: Array, loading: Boolean, hasMore: Boolean })
</script>

<style scoped>
td {
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--secondary-background-color);
  color: var(--text-color);
}

tr:hover {
  background-color: var(--hover-color);
}

tr:nth-child(even) {
  background-color: var(--secondary-background-color);
}

.no-data {
  color: var(--secondary-text-color);
}

.loading-indicator {
  color: var(--secondary-text-color);
}
</style>
