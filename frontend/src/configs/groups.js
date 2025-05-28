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
    },
    {
      title: 'Дата выпуска',
      key: 'releaseDate',
      sortKey: 'ReleaseDate'
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
    id: ['specializationId', 'year', 'index'],
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

export const editFormConfig = {
  title: 'группу',
  apiEndpoint: '/api/Group',
  fields: [
    {
      name: 'newIsActive',
      label: 'Статус',
      type: 'select',
      required: true,
      errorMessage: 'Необходимо выбрать статус',
      filter: {
        staticOptions: [
          { label: 'Активна', value: true },
          { label: 'Неактивна', value: false }
        ]
      }
    },
    {
      name: 'newAcronym',
      label: 'Сокращение',
      type: 'text',
      required: true,
      validate: (v) => v.length <= 10,
      errorMessage: 'Максимум 10 символов'
    },
    {
      name: 'newTeacherId',
      label: 'Классный руководитель',
      type: 'select',
      required: true,
      filter: {
        apiEndpoint: '/api/Teacher/filter',
        mapOption: opt => ({
          label: `${opt.surname} ${opt.name[0]}.${opt.patronymic[0]}.`,
          value: opt.id
        })
      }
    }
  ]
};

export default {
  tableConfig,
  filters,
  apiConfig,
  initialSort,
  editFormConfig,
}


