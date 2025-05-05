// group.config.js
import { generateYearOptions } from '@/utils/dateUtils.js'

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
    apiEndpoint: '/api/Department/filter',
    params: {
      take: 15,
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
    dataType: 'lookup',
    apiEndpoint: '/api/Specialization/filter',
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
  },
  {
    id: 'Years',
    title: 'Годы обучения',
    dataType: 'lookup',
    staticOptions: generateYearOptions({
      from: 2018,
      to: new Date().getFullYear()
    }),
    searchPlaceholder: 'Введите год'
  }
]

export const apiConfig = {
  endpoint: '/api/Group/filter',
  deleteEndpoint: '/api/Group',
  paramsMapping: {
    id: 'key',
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

export const createFormConfig = {
title:'группу',
  apiEndpoint:'/api/Group',
  fields:[
    {
      name: 'specializationId',
      label: 'Специальность',
      type: 'select',
      required: true,
      filter: {
        id: 'SpecializationId',
        title: 'Специальность',
        dataType: 'lookup',
        apiEndpoint: '/api/Specialization/filter',
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
      name: 'year',
      label: 'год',
      type: 'select',
      required: true,
      filter: {
        id: 'year',
        title: 'год',
        staticOptions: generateYearOptions({
          from: 2018,
          to: new Date().getFullYear()
        }),
        allowDeselect: false
      }
    },
    {
      name: 'id',
      label: 'Идентификатор',
      type: 'text',
      required: true,
      validate: (v) => v.length <= 3,
      errorMessage: 'Максимум 3 символа'
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

export default {
  tableConfig,
  filters,
  apiConfig,
  initialSort,
  createFormConfig
}


