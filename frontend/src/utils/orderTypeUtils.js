export const ORDER_TYPE_KEYS = {
  1: 'Зачисление',
  2: 'Перевод из другого учреждения',
  3: 'Перевод в другое учреждение',
  4: 'Восстановление после отчисления',
  5: 'Восстановление после академа',
  6: 'Академический отпуск',
  7: 'Перевод между группами',
  8: 'Отчисление',
  9: 'Выпуск'
};

export function formatOrderType(code) {
  return ORDER_TYPE_KEYS[code] || 'Неизвестно';
}

export const ORDER_TYPE_OPTIONS = Object.entries(ORDER_TYPE_KEYS)
  .map(([value, label]) => ({ value: +value, label }));
