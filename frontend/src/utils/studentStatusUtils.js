export const STUDENT_STATUS_KEYS = {
  'Active': 'Активный',
  'Expelled': 'Отчислен',
  'Vacation': 'Академ',
  'Released': 'Выпускник',
  'TransferToOtherInstitution': 'Переведён в другое ОУ',
  'NotActive': 'Не активен'
}

export function formatStudentStatus (code) {
  return STUDENT_STATUS_KEYS[code] || 'Неизвестно'
}

export const STUDENT_STATUS_OPTIONS = Object.entries(STUDENT_STATUS_KEYS)
  .map(([value, label]) => ({ value, label }))
