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
        title: 'Внесение данных',
        path: '/admin/migration',
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
  }
]
