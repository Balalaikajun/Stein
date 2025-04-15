<script setup>
import FunButton from '@/components/FunButton.vue'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import menuItems from '@/router/menuData.js'
import TestTable from '@/components/Table/TestTable.vue'


const tableConfig = {
  columns: [
    {
      title: 'Наименование',
      key: 'title',
      width: '200px',
      sortKey: 'Title' // Явное указание поля для сортировки
    },
    {
      title: 'Статус',
      key: 'isActive',
      disableSort: false, // Отключаем сортировку для колонки
      formatter: (value) => value ? 'Активен' : 'Неактивен'
    }
  ]
}
const filters = [
  {
    id: 'activeFilter',
    title: 'Статус',
    dataType: 'boolean',
    paramKey: 'activeFilter'
  }
]

const apiConfig = {
  endpoint: '/api/Department',
  paramsMapping: {
    search: 'searchText',
    sortKey: 'sortBy',
    sortOrder: 'descending',
    limit: 'limit',
    lastSeenId: 'lastSeenId',
    lastSeenValue: 'lastSeenValue'
  },
  responseAdapter: (response) => ({
    items: response.items,
    hasMore: response.hasMore,
    lastSeenId: response.lastSeenId,
    lastSeenValue: response.lastSeenValue
  })
}

const initialSort = {
  key: 'Title',
  ascending: true
}

</script>

<template>
  <div class="mainBox">
    <div>
      <Sidebar :items="menuItems"/>
    </div>

    <div class="box2">
      <div>
        <h1>Номенклатура</h1>
      </div>
      <TestTable
          :config="tableConfig"
          :filters="filters"
          :api-config="apiConfig"
          :initial-sort="initialSort"
          editable
      >
        <template #cell-isActive="{ item }">
      <span :class="['status-badge', item.isActive ? 'active' : 'inactive']">
        {{ item.isActive ? 'Активен' : 'Неактивен' }}
      </span>
        </template>
      </TestTable>
    </div>
  </div>
</template>

<style scoped>


.mainBox {
  display: flex;
}


.box2 {
  flex: 1;
  padding: 16px;
  display: flex;
  flex-direction: column;
}
 .status-badge {
   padding: 4px 8px;
   border-radius: 12px;
   font-size: 12px;
 }

.status-badge.active {
  background: #e8f5e9;
  color: #2e7d32;
}

.status-badge.inactive {
  background: #ffebee;
  color: #c62828;
}
</style>