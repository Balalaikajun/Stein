// course.config.js
export const tableConfig = {
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
      sortKey: 'Code',
      width: '150px'
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
}

export const filters = [
  {
    id: 'ActiveFilter',
    title: 'Статус',
    dataType: 'boolean'
  },
  {
    id: 'DepartmentIds',
    title: 'Отделения',
    dataType: 'lookup',
    api: '/api/Department', // Эндпоинт для загрузки отделений
    searchable: true,
    mapOption: (department) => ({
      label: department.title, // Отображаемое название
      value: department.id     // Значение для фильтрации
    })
  }
]

export const apiConfig = {
  endpoint: '/api/Specialization',
  paramsMapping: {
    search: 'SearchText',
    sortKey: 'SortBy',
    sortOrder: 'Descending',
    take: 'Take',
    skip: 'Skip'
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