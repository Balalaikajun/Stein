// group.config.js
import { generateYearOptions } from '@/utils/dateUtils.js'
import {formatStudentStatus, STUDENT_STATUS_OPTIONS} from '@/utils/studentStatusUtils.js'

export const tableConfig = {
  columns: [
    {
      title: 'ФИО',
      key: 'fullName',
      sortKey: 'Surname',
      formatter: (row) => `${row.surname} ${row.name} ${row.patronymic || ''}`.trim(),
      width: '250px'
    },
    {
      title: 'Группа',
      key: 'groupAcronym',
      sortKey: 'Group',
      width: '120px'
    },
    {
      title: 'Гражданство',
      key: 'isCitizen',
      sortKey: 'IsCitizen',
      formatter: (row) => row.isCitizen ? 'Да' : 'Нет',
      width: '130px'
    },
    {
      title: 'Пол',
      key: 'gender',
      sortKey: 'Gender',
      formatter: (row) => row.gender  === 0 ? 'Мужской' : 'Женский',
      width: '120px'
    },
    {
      title: 'Дата рождения',
      key: 'dateOfBirth',
      sortKey: 'DateOfBirth',
      width: '150px'
    },
    {
      title: 'Статус',
      key: 'status',
      sortKey: 'Status',
      formatter: (row) => formatStudentStatus(row.status),
      width: '150px'
    }
  ]
}

export const filters = [
  {
    id: 'studentStatusesFilter',
    title: 'Статус студента',
    dataType: 'lookup',
    staticOptions: STUDENT_STATUS_OPTIONS
  },
  {
    id: 'isCitizen',
    title: 'Гражданство',
    dataType: 'boolean',
    labels: ['Нет', 'Да']
  },
  {
    id: 'gender',
    title: 'Пол',
    dataType: 'radio',
    staticOptions: [
      { label: 'Мужской', value: 0 },
      { label: 'Женский', value: 1 }
    ]
  },
  {
    id: 'dateRange',
    title: 'Дата рождения',
    dataType: 'dateRange',
    paramKeys: {
      from: 'fromDate',
      to: 'toDate'
    }
  },
  {
    id: 'DepartmentIds',
    title: 'Кафедры',
    dataType: 'lookup',
    apiEndpoint: '/api/Department',
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
  endpoint: '/api/Student',
  paramsMapping: {
    search: 'searchText',
    sortKey: 'sortBy',
    sortOrder: 'descending',
    take: 'take',
    skip: 'skip',
    isCitizen: 'isCitizen',
    gender: 'gender',
    status: 'status',
    years: 'years',
    departmentIds: 'departmentIds',
    specializationIds: 'specializationIds',
    birthDateFrom: 'birthDateFrom',
    birthDateTo: 'birthDateTo'
  }
}

export const initialSort = {
  key: 'Surname',
  descending: false
}

export default {
  tableConfig,
  filters,
  apiConfig,
  initialSort
}