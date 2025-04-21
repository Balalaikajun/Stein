// group.config.js
export const tableConfig = {
  columns: [
    {
      title: 'Сокращение',
      key: 'acronym',
      sortKey: 'Acronym',
      width: '150px'
    },
    {
      title: 'Преподаватель',
      key: 'teacherFullname',
      sortKey: 'TeacherFullname',
      width: '200px'
    },
    {
      title: 'Количество студентов',
      key: 'studentCount',
      sortKey: 'StudentCount',
      width: '180px'
    },
    {
      title: 'Статус',
      key: 'isActive',
      sortKey: 'IsActive',
      formatter: (value) => value ? 'Активна' : 'Неактивна',
      width: '120px'
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
    apiEndpoint: '/api/Department',
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
  },
  {
    id: 'SpecializationIds',
    title: 'Специализации',
    dependsOn: ['DepartmentIds'],
    dataType: 'lookup',
    apiEndpoint: '/api/Specialization',
    params: {
      take: 100,
      sortBy: 'Title',
      descending: false
    },
    paramKeys: {
      skip: 'skip',
      take: 'take',
      search: 'searchText',
      sortKey: 'sortBy',
      sortOrder: 'descending'
    },
    mapOption: opt => ({ label: opt.title, value: opt.id })
  },
  {
    id: 'Years',
    title: 'Годы обучения',
    dataType: 'number[]'
  }
]

export const apiConfig = {
  endpoint: '/api/Group',
  paramsMapping: {
    search: 'searchText',
    sortKey: 'sortBy',
    sortOrder: 'descending',
    take: 'take',
    skip: 'skip'
  }
}

export const initialSort = {
  key: 'Acronym',
  descending: false
}

export default {
  tableConfig,
  filters,
  apiConfig,
  initialSort
}
