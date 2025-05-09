export default [
  {
    title: 'Главная',
    path: '/',
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
      { title: 'Группы', path: '/groups' },
      { title: 'Студенты', path: '/students' },
      { title: 'Преподаватели', path: '/teachers' },
      { title: 'Приказы', path: '/orders' },
      { title: 'Успеваемость', path: '/performance' }
    ]
  },
  {
    title: 'Дашборд',
    path: '/dashboard',
    icon: '📊'
  }
]
