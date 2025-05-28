export const ORDER_TYPE_KEYS = {
  'Enrollment': 'Зачисление',
  'TransferFromOtherInstitution': 'Перевод из другого учреждения',
  'TransferToOtherInstitution': 'Перевод в другое учреждение',
  'ReinstatementFromExpelled': 'Восстановление после отчисления',
  'ReinstatementFromAcademy': 'Восстановление после академа',
  'AcademicLeave': 'Академический отпуск',
  'TransferBetweenGroups': 'Перевод между группами',
  'Expulsion': 'Отчисление',
  'Graduation' : 'Выпуск'
};

export function formatOrderType(code) {
  return ORDER_TYPE_KEYS[code] || 'Неизвестно';
}

export const ORDER_TYPE_OPTIONS = Object.entries(ORDER_TYPE_KEYS)
  .map(([value, label]) => ({ value, label }));
