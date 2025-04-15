export const tableConfig = {
  columns: [
    {
      title: 'Наименование',
      key: 'title',
      width: '300px',
      sortKey: 'Title'
    },
    {
      title: 'Статус',
      key: 'isActive',
      sortKey: 'IsActive',
      formatter: (value) => value ? 'Активен' : 'Неактивен'
    }
  ]
}

export const filters = [
  {
    id: 'activeFilter',
    title: 'Статус',
    dataType: 'boolean',
    paramKey: 'activeFilter'
  }
]

export const apiConfig = {
  endpoint: '/api/Department',
  paramsMapping: {
    search: 'SearchText',
    sortKey: 'SortBy',
    sortOrder: 'Descending',
    limit: 'Limit',
    lastSeenId: 'LastSeenId',
    lastSeenValue: 'LastSeenValue'
  }
}

export const initialSort = {
  key: 'Title',
  descending: false
}

export default {
  tableConfig,
  filters,
  apiConfig,
  initialSort
}