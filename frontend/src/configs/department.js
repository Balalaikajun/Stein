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
    id: 'ActiveFilter', // Должно совпадать с ключом в данных
    title: 'Статус',
    dataType: 'boolean'
  }
]

export const apiConfig = {
  endpoint: '/api/Department',
  paramsMapping: {
    search: 'SearchText',
    sortKey: 'SortBy',
    sortOrder: 'Descending',
    take: 'Take',
    skip: 'Skip',
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