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
td { padding: 12px 16px; border-bottom: 1px solid #e0e0e0; text-align: left; }
tr:hover { background-color: #f5f5f5; }
tr:nth-child(even) { background-color: #fafafa; }
.no-data { padding: 24px; color: #757575; text-align: center; font-style: italic; }
.loading-indicator { text-align: center; padding: 16px 0; font-size: 14px; color: #757575; }
</style>
