// order.config.js
import { formatOrderType, ORDER_TYPE_OPTIONS } from '@/utils/orderTypeUtils'

export const tableConfig = {
  columns: [
    {
      title: 'Тип приказа',
      key: 'orderType',
      sortKey: 'OrderType',
      formatter: (row) => formatOrderType(row.orderType),
      width: '150px'
    },
    {
      title: 'Номер приказа',
      key: 'orderNumber',
      sortKey: 'OrderNumber',
      width: '150px'
    },
    {
      title: 'Студент',
      key: 'studentFullName',
      sortKey: 'StudentFullName',
      width: '200px'
    },
    {
      title: 'Дата приказа',
      key: 'date',
      sortKey: 'Date',
      formatter: (row) => row.date,
      width: '150px'
    },
    {
      title: 'Исходная группа',
      key: 'groupFromAcronym',
      formatter: (row) => row.groupFromAcronym ? row.groupFromAcronym: '---',
      width: '200px'
    },
    {
      title: 'Новая группа',
      key: 'groupToAcronym',
      formatter: (row) => row.groupToAcronym ? row.groupToAcronym: '---',
      width: '200px'
    }
  ]
}

export const filters = [
  {
    id: 'orderTypesFilter',
    title: 'Тип приказа',
    dataType: 'lookup',
    staticOptions: ORDER_TYPE_OPTIONS,
    paramKeys: {
      value: 'orderTypesFilter'
    }
  },
  {
    id: 'dateRangeFilter',
    title: 'Дата приказа',
    dataType: 'dateRange',
    paramKeys: {
      from: 'fromDate',
      to: 'toDate'
    }
  }
]

export const apiConfig = {
  endpoint: '/api/Order',
  paramsMapping: {
    search: 'searchText',
    sortKey: 'sortBy',
    sortOrder: 'descending',
    take: 'take',
    skip: 'skip',
    orderTypesFilter: 'orderTypesFilter',
    fromDate: 'fromDate',
    toDate: 'toDate',
    groupIds: 'groupIds'
  }
}

export const initialSort = {
  key: 'Date',
  descending: true
}

export default {
  tableConfig,
  filters,
  apiConfig,
  initialSort
}