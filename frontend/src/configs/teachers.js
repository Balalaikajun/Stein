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
    endpoint: '/api/Teacher/filter',
    deleteEndpoint: '/api/Teacher',
    paramsMapping: {
      id: 'id',
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
  },
  createFormConfig: {
    title: 'преподавателя',
    apiEndpoint: '/api/Teacher',
    fields: [
      {
        name: 'surname',
        label: 'Фамилия',
        type: 'text',
        required: true,
        validate: (v) => v.length <= 32,
        errorMessage: 'Максимум 32 символов'
      },
      {
        name: 'name',
        label: 'Имя',
        type: 'text',
        required: true,
        validate: (v) => v.length <= 32,
        errorMessage: 'Максимум 32 символов'
      },
      {
        name: 'patronymic',
        label: 'Отчество',
        type: 'text',
        required: true,
        validate: (v) => v.length <= 32,
        errorMessage: 'Максимум 32 символов'
      },
      {
        name: 'isActive',
        label: 'Статус',
        type: 'select',
        required: true,
        errorMessage: 'Необходимо выбрать статус',
        filter: {
          id: 'statusFilter', // Уникальный идентификатор фильтра
          title: 'Статус',
          staticOptions: [     // Статические данные
            { label: 'Активен', value: true },
            { label: 'Неактивен', value: false }
          ],
          allowDeselect: false  // Разрешить снятие выбора
        }
      }
    ],
  }
}