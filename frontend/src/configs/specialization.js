export default {
  tableConfig: {
    columns: [
      {
        title: 'Название курса',
        key: 'title',
        width: '300px',
        sortKey: 'Title'
      },
      {
        title: 'Код',
        key: 'code',
        width: '150px',
        sortKey: 'Code'
      },
      {
        title: 'Сокращение',
        key: 'acronym',
        sortKey: 'Acronym'
      },
      {
        title: 'Отделение',
        key: 'departmentTitle',
        sortKey: 'Department'
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
        { label: 'Активен', value: true },
        { label: 'Неактивен', value: false },
        { label: 'Все', value: null }
      ],
      allowDeselect: true
    },
    {
      id: 'DepartmentIds',
      title: 'Отделения',
      dataType: 'lookup',
      apiEndpoint: '/api/Department/filter',
      params: {
        take: 100,
        activeFilter: true,
        sortBy: 'Title',
        descending: false
      },
      paramKeys: {
        skip: 'skip',
        take: 'take',
        search: 'searchText',
        sortKey: 'sortBy',
        sortOrder: 'descending',
        activeFilter: 'activeFilter'
      },
      mapOption: opt => ({ label: opt.title, value: opt.id })
    }
  ],
  apiConfig: {
    endpoint: '/api/Specialization/filter',
    deleteEndpoint: '/api/Specialization',
    paramsMapping: {
      id: 'id',
      search: 'searchText',
      sortKey: 'sortBy',
      sortOrder: 'descending',
      take: 'take',
      skip: 'skip'
    }
  },
  initialSort: {
    key: 'Title',
    descending: false
  },
  editFormConfig: {
    title: 'курс',
    apiEndpoint: '/api/Specialization',
    fields: [
      {
        name: 'title',
        label: 'Название специальности',
        type: 'text',
        required: true,
        validate: (v) => v.length <= 150,
        errorMessage: 'Максимум 300 символов'
      },
      {
        name: 'code',
        label: 'Код',
        type: 'text',
        required: true,
        validate: (v) => v.length <= 25,
        errorMessage: 'Максимум 50 символов'
      },
      {
        name: 'acronym',
        label: 'Сокращение',
        type: 'text',
        required: true,
        validate: (v) => v.length <= 15,
        errorMessage: 'Максимум 50 символов'
      },
      {
        name: 'departmentId',
        label: 'Отделение',
        type: 'select',
        required: true,
        filter: {
          id: 'DepartmentId',
          title: 'Отделение',
          dataType: 'lookup',
          apiEndpoint: '/api/Department/filter',
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
      },
      {
        name: 'isActive',
        label: 'Статус',
        type: 'select',
        required: true,
        errorMessage: 'Необходимо выбрать статус',
        filter: {
          id: 'statusFilter',
          title: 'Статус',
          staticOptions: [
            { label: 'Активен', value: true },
            { label: 'Неактивен', value: false }
          ],
          allowDeselect: false
        }
      }
    ]
  }
}
