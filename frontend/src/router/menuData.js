export default [
  {
    title: 'Главная',
    path: '/home',
    icon: '🏠'
  },
  {
    title: 'Администрация',
    icon: '👔',
    isOpen: false,
    children: [
      {
        title: 'Добавить приказ',
        path: '/admin/add-order'
      },
      {
        title: 'Внести данные',
        path: '/admin/performance'
      }
    ]
  },
  {
    title: 'Справочники',
    icon: '📋',
    isOpen: false,
    children: [
      { title: 'Отделения', path: '/departments' },
      { title: 'Специальности', path: '/specializations' },
      { title: 'Группы', path: '/nomenclature/groups' },
      { title: 'Студенты', path: '/nomenclature/students' },
      { title: 'Преподаватели', path: '/nomenclature/teachers' },
      { title: 'Приказы', path: '/nomenclature/orders' },
      { title: 'Успеваемость', path: '/nomenclature/performance' }
    ]
  },
  {
    title: 'Дашборд',
    path: '/dashboard',
    icon: '📊'
  }
]
