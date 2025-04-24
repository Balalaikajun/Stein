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
  },
  createFormConfig: {
    title: 'пользователя',
    fields: [
      {
        name: 'name',
        label: 'Имя',
        type: 'date',
        required: true,
        validate: (value) => value.length >= 3,
        errorMessage: 'Минимум 3 символа'
      },
      {
        name: 'email',
        label: 'Email',
        type: 'select',
        required: true,
        errorMessage: 'Неверный формат email',
        filter: {
          id: 'SpecializationIds',
          title: 'Специализации',
          dataType: 'lookup',
          apiEndpoint: '/api/Specialization',
          dependsOn: ['DepartmentIds'],
          dependentParams: {
            DepartmentIds: 'departmentIds'
          },
          params: {
            take: 15,
          },
          paramKeys: {
            skip: 'skip',
            take: 'take',
            search: 'searchText',
            sortKey: 'sortBy',
            sortOrder: 'descending'
          },
          mapOption: opt => ({ label: opt.title, value: opt.id })
        }
      }
    ],
    submitHandler: async (data) => {
      // API запрос для сохранения данных
      console.log('Saving data:', data)
    }
  },
  editFormConfig: {
    title: 'пользователя',
    fields: [
      {
        name: 'name',
        label: 'Имя',
        type: 'text',
        required: true,
        validate: (value) => value.length >= 3,
        errorMessage: 'Минимум 3 символа'
      },
      {
        name: 'email',
        label: 'Email',
        type: 'select',
        required: true,
        errorMessage: 'Неверный формат email',
        filter: {
          id: 'SpecializationIds',
          title: 'Специализации',
          dataType: 'lookup',
          apiEndpoint: '/api/Specialization',
          dependsOn: ['DepartmentIds'],
          dependentParams: {
            DepartmentIds: 'departmentIds'
          },
          params: {
            take: 15,
          },
          paramKeys: {
            skip: 'skip',
            take: 'take',
            search: 'searchText',
            sortKey: 'sortBy',
            sortOrder: 'descending'
          },
          mapOption: opt => ({ label: opt.title, value: opt.id })
        }
      }
    ],
    submitHandler: async (data) => {
      // API запрос для сохранения данных
      console.log('Saving data:', data)
    }
  }
}