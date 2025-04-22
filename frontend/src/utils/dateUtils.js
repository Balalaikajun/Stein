export function generateYearOptions({ from = 2018, to = new Date().getFullYear() }) {
  const years = []
  for (let y = to; y >= from; y--) {
    years.push({ value: y, label: String(y) })
  }
  return years
}