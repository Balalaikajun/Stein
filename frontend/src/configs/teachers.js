export default {
  tableConfig: {
    columns: [
      {
        title: 'Фамилия',
        key: 'surname',
        sortKey: 'Surname',
        width: '200px'
      },
      {
        title: 'Имя',
        key: 'name',
        sortKey: 'Name',
        width: '200px'
      },
      {
        title: 'Отчество',
        key: 'patronymic',
        sortKey: 'Patronymic',
        width: '200px'
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
    endpoint: '/api/Teacher',
    paramsMapping: {
      search: 'searchText',
      sortKey: 'sortBy',
      sortOrder: 'descending',
      take: 'take',
      skip: 'skip',
    }
  },
  initialSort: {
    key: 'Surname',
    descending: false
  }
}