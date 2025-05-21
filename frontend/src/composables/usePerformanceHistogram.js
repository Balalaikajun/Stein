// src/composables/usePerformanceHistogram.js
import { ref, watch } from 'vue'
import axios from 'axios'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

/**
 * Сдвигает год/месяц на заданное число месяцев.
 * @param {number} year — четырёхзначный год (например, 2025)
 * @param {number} month — месяц от 1 до 12
 * @param {number} offset — смещение в месяцах (может быть отрицательным)
 * @returns {{ year: number, month: number }} — новый год и месяц
 */
function shiftMonth(year, month, offset) {
  // Переводим в абсолютный счётчик месяцев (0-based для удобства)
  const totalMonths = year * 12 + (month - 1) + offset
  const newYear = Math.floor(totalMonths / 12)
  const newMonth = (totalMonths % 12 + 12) % 12 + 1 // %12 и возвращаем в 1..12
  return { year: newYear, month: newMonth }
}

export function usePerformanceHistogram(
  filtersRef,
  options = {
    pageSize: 6,            // Сколько месяцев показывать в одном «листе»
    initialYearTo: null,     // По умолчанию — текущая дата (year, month)
    initialMonthTo: null
  }
) {
  // === Состояние пагинации ===
  const pageSize = options.pageSize
  const now = new Date()
  const yearTo = ref(options.initialYearTo ?? now.getFullYear())
  const monthTo = ref(options.initialMonthTo ?? now.getMonth() + 1)

  // Откуда начнём текущую «сторинку»:
  const from = shiftMonth(yearTo.value, monthTo.value, -(pageSize - 1))
  const yearFrom = ref(from.year)
  const monthFrom = ref(from.month)

  // === Состояние ответа API ===
  const data = ref([])          // массив элементов { Year, Month, Count, ExcellentCount, ... }
  const hasBefore = ref(false)
  const hasAfter = ref(false)
  const loading = ref(false)
  const error = ref(null)

  // === Функция для вызова API ===
  async function fetchPage() {
    loading.value = true
    error.value = null


    // Составляем тело запроса с учётом фильтров (departments, specializations, groups...)
    const body = {
      yearFrom: yearFrom.value,
      monthFrom: monthFrom.value,
      yearTo: yearTo.value,
      monthTo: monthTo.value,
      isFullTime: filtersRef.value.isFullTime ?? null,
      departments: Array.isArray(filtersRef.value.departmentIds)
        ? filtersRef.value.departmentIds
        : null,
      specializations: Array.isArray(filtersRef.value.SpecializationIds)
        ? filtersRef.value.SpecializationIds
        : null,
      groups: Array.isArray(filtersRef.value.GroupKeys)
        ? filtersRef.value.GroupKeys
        : null
    }

    try {
      const resp = await axios.post(
        `${BACKEND_API_HOST}/api/Metrics/histogram/performance`,
        body,
        { headers: { 'Content-Type': 'application/json' } }
      )
      const dto = resp.data

      // === маппим DTO → формат, ожидаемый в PerformanceHistogram.vue ===
      // DTO PerformanceElementDto: { year, month, count, data: { Excellent, Good, Normal, Falling } }
      data.value = (dto.elements ?? []).map(el => ({
        Year:        el.year,
        Month:       el.month,
        Count:       el.count,
        ExcellentCount: el.data?.Excellent   ?? 0,
        GoodCount:      el.data?.Good        ?? 0,
        NormalCount:    el.data?.Normal      ?? 0,
        FallingCount:   el.data?.Falling     ?? 0
      }))

      hasBefore.value = !!dto.hasBefore
      hasAfter.value  = !!dto.hasAfter
    } catch (err) {
      console.error('Ошибка загрузки успеваемости:', err)
      error.value = err
      data.value = []
      hasBefore.value = false
      hasAfter.value  = false
    } finally {
      loading.value = false
    }
  }

  // === Функции для перехода на «предыдущую страницу» (раньше даты) и «следующую» (позже) ===

  function prevPage() {
    if (!hasBefore.value) return
    // Новый край «To» = месяц до текущего from
    const newTo = shiftMonth(yearFrom.value, monthFrom.value, -1)
    yearTo.value   = newTo.year
    monthTo.value  = newTo.month

    // Новый «From» = newTo − (pageSize − 1) месяцев
    const newFrom = shiftMonth(newTo.year, newTo.month, -(pageSize - 1))
    yearFrom.value  = newFrom.year
    monthFrom.value = newFrom.month

    fetchPage()
  }

  function nextPage() {
    if (!hasAfter.value) return
    // Новый край «From» = месяц после текущего to
    const newFrom = shiftMonth(yearTo.value, monthTo.value, +1)
    yearFrom.value  = newFrom.year
    monthFrom.value = newFrom.month

    // Новый «To» = newFrom + (pageSize − 1) месяцев
    const newTo = shiftMonth(newFrom.year, newFrom.month, +(pageSize - 1))
    yearTo.value   = newTo.year
    monthTo.value  = newTo.month

    fetchPage()
  }

  // === Сброс страницы (к начальному окну) при изменении внешних фильтров ===

  function resetToLatest() {
    const nowDate = new Date()
    yearTo.value  = nowDate.getFullYear()
    monthTo.value = nowDate.getMonth() + 1
    const f = shiftMonth(yearTo.value, monthTo.value, -(pageSize - 1))
    yearFrom.value  = f.year
    monthFrom.value = f.month
  }

  watch(
    () => filtersRef.value,
    () => {
      resetToLatest()
      fetchPage()
    },
    { deep: true }
  )

  // === Первоначальная загрузка ===
  fetchPage()

  return {
    // Данные для графика
    data,
    // Флаги наличия «до» и «после»
    hasBefore,
    hasAfter,
    // Текущее «окно» (спан) дат
    yearFrom,
    monthFrom,
    yearTo,
    monthTo,
    // Загрузка / ошибка
    loading,
    error,
    // Методы навигации
    prevPage,
    nextPage
  }
}
