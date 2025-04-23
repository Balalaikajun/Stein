import { generateYearOptions } from '@/utils/dateUtils.js'
import { formatStudentStatus, STUDENT_STATUS_OPTIONS } from '@/utils/studentStatusUtils.js'

export const tableConfig = {
  studentColumns: [
    {
      title: 'ФИО',
      key: 'fullName',
      sortKey: 'Surname',
      formatter: row => `${row.surname} ${row.name[0]}.${row.patronymic[0] || ''}`.trim(),
      width: '250px'
    }
  ]
}

export const filters = [
  {
    id: 'groupFilter',
    title: 'Группа',
    dataType: 'radio',
    apiEndpoint: '/api/Group',
    params: {
      take: 15,
      sortBy: 'acronym',
      sortOrder: 'descending'
    },
    paramKeys: {
      skip: 'skip',
      take: 'take',
      search: 'searchText'
    },
    mapOption: opt => ({
      label: opt.acronym,
      value: [opt.key]
    }),
    allowDeselect: false
  },
  {
    id: 'Year',
    title: 'Год',
    dataType: 'select',
    options: generateYearOptions(5),
    paramKeys: {
      value: 'year'
    }
  }
]

export const apiConfig = {
  student: {
    endpoint: '/api/Student',
    paramsMapping: {
      take: 'take',
      skip: 'skip',
      search: 'search',
      sortKey: 'sortBy',
      sortOrder: 'sortOrder',
      group: 'groupFilter',
      // сюда будут попадать все текущие фильтры, включая фильтр по году
    }
  },
  performance: {
    endpoint: '/api/AcademicPerformance',
    paramsMapping: {
      take: 'take',
      skip: 'skip',
      group: 'groupFilter',
      descending: 'descending',
      // год возьмём из того же фильтра year
    }
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
