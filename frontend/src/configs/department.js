export default {
  tableConfig: {
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
  },
  filters: [
    {
      id: 'ActiveFilter',
      title: 'Статус',
      dataType: 'boolean'
    }
  ],
  apiConfig: {
    endpoint: '/api/Department',
    paramsMapping: {
      search: 'SearchText',
      sortKey: 'SortBy',
      sortOrder: 'Descending',
      take: 'Take',
      skip: 'Skip',
    }
  },
  initialSort: {
    key: 'Title',
    descending: false
  }
}