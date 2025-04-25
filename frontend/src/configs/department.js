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
      dataType: 'radio',
      staticOptions: [
        { label: 'Да', value: true },
        { label: 'Нет', value: false },
        { label: 'Все', value: null }
      ],
      allowDeselect: true
    }
  ],
  apiConfig: {
    endpoint: '/api/Department/filter',
    deleteEndpoint:'/api/Department',
    paramsMapping: {
      id: "id",
      search: 'SearchText',
      sortKey: 'SortBy',
      sortOrder: 'Descending',
      take: 'Take',
      skip: 'Skip'
    }
  },
  initialSort: {
    key: 'Title',
    descending: false
  },
  createFormConfig: {
    title: 'пользователя',
    fields: [
      {
        name: 'title',
        label: 'Наименование',
        type: 'text',
        required: true,
        validate: (v) => v.length <= 125,
        errorMessage: 'Максимум 125 символов'
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
  },
  editFormConfig: {
    title: 'пользователя',
    apiEndpoint: '/api/Department',
    fields: [
      {
        name: 'title',
        label: 'Наименование',
        type: 'text',
        required: true,
        validate: (v) => v.length <= 125,
        errorMessage: 'Максимум 125 символов'
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
