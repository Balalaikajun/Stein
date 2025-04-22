export const STUDENT_STATUS_KEYS = {
  1: 'Активный',
  2: 'Отчислен',
  3: 'Академ',
  4: 'Выпускник'
};

export function formatStudentStatus(code) {
  return STUDENT_STATUS_KEYS[code] || 'Неизвестно';
}

export const STUDENT_STATUS_OPTIONS = Object.entries(STUDENT_STATUS_KEYS)
  .map(([value, label]) => ({ value: +value, label }));
