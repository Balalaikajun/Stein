// src/composables/usePerformanceHistogram.js
import { computed, isRef, ref, unref, watch } from 'vue'
import axios from '@/api/api.js'

/**
 * Сдвигает год/месяц на заданное число месяцев.
 * @param {number} year — четырёхзначный год (например, 2025)
 * @param {number} month — месяц от 1 до 12
 * @param {number} offset — смещение в месяцах (может быть отрицательным)
 * @returns {{ year: number, month: number }} — новый год и месяц
 */
function shiftMonth (year, month, offset) {
  const totalMonths = year * 12 + (month - 1) + offset
  const newYear = Math.floor(totalMonths / 12)
  const newMonth = (totalMonths % 12 + 12) % 12 + 1
  return { year: newYear, month: newMonth }
}

/**
 * Хук для построения гистограммы успеваемости.
 *
 * @param {object|Ref<object>} filtersSource
 *   — объект фильтров (например, { isFullTime, departmentIds, SpecializationIds, GroupKeys });
 * @param {object} options
 *   — настройки пагинации: { pageSize, initialYearTo, initialMonthTo }.
 *
 * Пример использования:
 * const filters = ref({ isFullTime: true, departmentIds: [1,2], SpecializationIds: null, GroupKeys: ['A'] })
 * const {
 *   data,
 *   hasBefore,
 *   hasAfter,
 *   yearFrom, monthFrom, yearTo, monthTo,
 *   loading, error,
 *   prevPage, nextPage, reload
 * } = usePerformanceHistogram(filters, { pageSize: 12 })
 *
 * При любом изменении filters автоматически сбросится окно на последние pageSize месяцев
 * и данные будут перезагружены.
 */
export function usePerformanceHistogram (filtersSource, options = {}) {
  // Если filtersSource не Ref, оборачиваем в ref
  const filtersRef = isRef(filtersSource) ? filtersSource : ref(filtersSource)

  const pageSize = options.pageSize ?? 12
  const now = new Date()
  const initialToYr = options.initialYearTo ?? now.getFullYear()
  const initialToMo = options.initialMonthTo ?? (now.getMonth() + 1)

  // --- Состояние "окна" дат (span) ---
  const yearTo = ref(initialToYr)
  const monthTo = ref(initialToMo)
  const { year: fromYr, month: fromMo } = shiftMonth(yearTo.value, monthTo.value, -(pageSize - 1))
  const yearFrom = ref(fromYr)
  const monthFrom = ref(fromMo)

  // --- Состояние запроса / ответа ---
  const data = ref([])    // [{ Year, Month, Count, ExcellentCount, ... }, ...]
  const hasBefore = ref(false)
  const hasAfter = ref(false)
  const loading = ref(false)
  const error = ref(null)

  // ― Вычисляемое тело запроса с учётом фильтров и текущего "окна" дат
  const requestBody = computed(() => {
    // Берём распакованный объект фильтров
    const f = unref(filtersRef) || {}

    return {
      // Границы по датам
      yearFrom: yearFrom.value,
      monthFrom: monthFrom.value,
      yearTo: yearTo.value,
      monthTo: monthTo.value,

      // Фильтры из внешнего объекта
      isFullTime: f.isFullTime ?? null,
      departments: Array.isArray(f.departmentIds) ? f.departmentIds : null,
      specializations: Array.isArray(f.SpecializationIds) ? f.SpecializationIds : null,
      groups: Array.isArray(f.GroupKeys) ? f.GroupKeys : null,
    }
  })

  // === Функция для вызова API ===
  async function fetchPage () {
    loading.value = true
    error.value = null

    try {
      const body = requestBody.value
      const resp = await axios.post(
        `/api/Metrics/histogram/performance`,
        body,
        { headers: { 'Content-Type': 'application/json' } }
      )
      const dto = resp.data

      // Маппим DTO → формат для графика
      data.value = (dto.elements ?? []).map(el => ({
        Year: el.year,
        Month: el.month,
        Count: el.count,
        ExcellentCount: el.data?.Excellent ?? 0,
        GoodCount: el.data?.Good ?? 0,
        NormalCount: el.data?.Normal ?? 0,
        FallingCount: el.data?.Falling ?? 0,
      }))

      hasBefore.value = Boolean(dto.hasBefore)
      hasAfter.value = Boolean(dto.hasAfter)
    } catch (err) {
      console.error('Ошибка загрузки успеваемости:', err)
      error.value = err
      data.value = []
      hasBefore.value = false
      hasAfter.value = false
    } finally {
      loading.value = false
    }
  }

  // === Навигация по страницам ===
  function prevPage () {
    if (!hasBefore.value) return
    // Новый край "To" = месяц до текущего from
    const newTo = shiftMonth(yearFrom.value, monthFrom.value, -1)
    yearTo.value = newTo.year
    monthTo.value = newTo.month

    // Новый "From" = newTo − (pageSize − 1) месяцев
    const newFrom = shiftMonth(newTo.year, newTo.month, -(pageSize - 1))
    yearFrom.value = newFrom.year
    monthFrom.value = newFrom.month

    fetchPage()
  }

  function nextPage () {
    if (!hasAfter.value) return
    // Новый край "From" = месяц после текущего to
    const newFrom = shiftMonth(yearTo.value, monthTo.value, +1)
    yearFrom.value = newFrom.year
    monthFrom.value = newFrom.month

    // Новый "To" = newFrom + (pageSize − 1) месяцев
    const newTo = shiftMonth(newFrom.year, newFrom.month, +(pageSize - 1))
    yearTo.value = newTo.year
    monthTo.value = newTo.month

    fetchPage()
  }

  // === Сброс окна на последние pageSize месяцев ===
  function resetToLatest () {
    const nowDate = new Date()
    yearTo.value = nowDate.getFullYear()
    monthTo.value = nowDate.getMonth() + 1
    const { year, month } = shiftMonth(yearTo.value, monthTo.value, -(pageSize - 1))
    yearFrom.value = year
    monthFrom.value = month
  }

  // === Перезагрузка (например, вручную) ===
  function reload () {
    // просто заново заберём страницу с текущими filters и span
    fetchPage()
  }

  // === Реакция на изменение фильтров: сбрасываем датный span и грузим первую (последнюю) страницу ===
  watch(
    () => unref(filtersRef),
    () => {
      resetToLatest()
      fetchPage()
    },
    { deep: true, immediate: false }
  )

  // === Первоначальная загрузка ===
  fetchPage()

  return {
    // Данные для графика
    data,

    // Флаги наличия "до" и "после"
    hasBefore,
    hasAfter,

    // Текущее "окно" (спан) дат
    yearFrom,
    monthFrom,
    yearTo,
    monthTo,

    // Состояние загрузки / ошибки
    loading,
    error,

    // Методы навигации / перезагрузки
    prevPage,
    nextPage,
    reload,
  }
}
