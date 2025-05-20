<template>
  <div class="specialty-chart-container">
    <div class="chart-header">
      <h3 class="chart-title">{{ title }}</h3>
      <div class="chart-controls">
        <slot name="controls"></slot>
      </div>
    </div>
    <div class="chart-scroll-wrapper">
      <div class="chart-wrapper">
        <canvas ref="chart"></canvas>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, nextTick } from 'vue'
import {
  Chart,
  BarController,
  CategoryScale,
  LinearScale,
  BarElement,
  Tooltip,
  Legend
} from 'chart.js'

// Регистрируем необходимые контроллеры и плагины
Chart.register(
    BarController,
    CategoryScale,
    LinearScale,
    BarElement,
    Tooltip,
    Legend
)

// Описание props:
// data: массив объектов вида { department, specialization, course, group, count }
// title: строка (название графика)
const props = defineProps({
  data: {
    type: Array,
    default: () => [],
    validator: items =>
        items.every(item =>
            'department' in item &&
            'specialization' in item &&
            'course' in item &&
            'group' in item &&
            'count' in item &&
            Number.isInteger(item.count)
        )
  },
  title: { type: String, default: 'Распределение студентов' }
})

const chart = ref(null)
let chartInstance = null

// Функция для генерации цветов для каждого курса
const generateCourseColors = (courseCount) => {
  const styles = getComputedStyle(document.documentElement)
  const baseColor = styles.getPropertyValue('--primary-color')?.trim() || '#5B00E1'

  // Преобразуем hex-цвет в RGB
  const r = parseInt(baseColor.slice(1, 3), 16)
  const g = parseInt(baseColor.slice(3, 5), 16)
  const b = parseInt(baseColor.slice(5, 7), 16)

  return Array.from({ length: courseCount }, (_, i) => {
    const opacity = 1 - (i * 0.15)
    return `rgba(${r}, ${g}, ${b}, ${opacity})`
  })
}

const initChart = () => {
  if (chartInstance) {
    chartInstance.destroy()
    chartInstance = null
  }
  if (!props.data.length) return

  // 1. Собираем все уникальные названия специальностей (labels по X-оси)
  const specializations = Array.from(
      new Set(props.data.map(item => item.specialization))
  )

  // 2. Собираем все уникальные значения курсов (dataset-ы)
  const courses = Array.from(
      new Set(props.data.map(item => item.course))
  ).sort((a, b) => {
    // Если курсы числовые, сортируем как числа, иначе как строки
    const na = Number(a), nb = Number(b)
    if (!isNaN(na) && !isNaN(nb)) return na - nb
    return a.localeCompare(b)
  })

  // 3. Предвычисляем:
  //   a) Суммарное количество студентов по каждой специальности
  //   b) Суммарное количество студентов по каждому отделению
  //   c) Словарь: для каждой «специальность|курс» массив групп с их числами
  const specTotals = {}     // specTotals[специальность] = число
  const deptTotals = {}     // deptTotals[отделение]    = число
  const specDeptMap = {}    // specDeptMap[специальность] = отделение
  const groupsMap = {}      // groupsMap["специальность|курс"] = [{ name: 'ПМ-101', count: 10 }, …]

  // Инициализируем:
  props.data.forEach(item => {
    // 3.a) Специальность → суммарная численность
    if (!specTotals[item.specialization]) {
      specTotals[item.specialization] = 0
    }
    specTotals[item.specialization] += item.count

    // 3.b) Отделение → суммарная численность
    if (!deptTotals[item.department]) {
      deptTotals[item.department] = 0
    }
    deptTotals[item.department] += item.count

    // 3.c) Запоминаем, что данная специальность относится к этому же отделению
    specDeptMap[item.specialization] = item.department

    // 3.d) Группы: ключ — "специальность|курс"
    const key = `${item.specialization}|${item.course}`
    if (!groupsMap[key]) {
      groupsMap[key] = []
    }
    groupsMap[key].push({ name: item.group, count: item.count })
  })

  // 4. Формируем «матрицу» значений: для каждого курса — массив чисел по специальностям
  const dataMatrix = courses.map(course => {
    return specializations.map(spec => {
      // Найти все записи, у которых совпадает specialization=spec и course=course
      // и сложить их count
      return props.data
          .filter(item => item.specialization === spec && item.course === course)
          .reduce((sum, item) => sum + item.count, 0)
    })
  })

  // Подготовка цветов и прочих стилей
  const styles = getComputedStyle(document.documentElement)
  const textColor = styles.getPropertyValue('--text-color')?.trim() || '#000'
  const bgColor = styles.getPropertyValue('--background-color')?.trim() || '#fff'
  const gridColor = styles.getPropertyValue('--secondary-background-color')?.trim() || '#ddd'

  const courseColors = generateCourseColors(courses.length)

  // Создаём Chart.js график
  chartInstance = new Chart(chart.value, {
    type: 'bar',
    data: {
      labels: specializations,
      datasets: courses.map((course, idx) => ({
        label: `${course} курс`,
        data: dataMatrix[idx],
        backgroundColor: courseColors[idx],
        borderColor: bgColor,
        borderWidth: 1,
        hoverBackgroundColor: courseColors[idx] + 'CC',
        categoryPercentage: 0.8,
        barPercentage: 0.9
      }))
    },
    options: {
      indexAxis: 'x',
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        tooltip: {
          backgroundColor: bgColor,
          titleColor: textColor,
          bodyColor: textColor,
          footerColor: textColor,
          borderColor: gridColor,
          borderWidth: 1,
          padding: 12,
          callbacks: {
            // Заголовок подсказки: название специальности
            title: (items) => {
              return items[0].label
            },
            // Перед самим списком (title и label): показываем отделение и численность отделения, а также специальность и её численность
            beforeBody: (items) => {
              const dataIndex = items[0].dataIndex  // индекс по специальности
              const specName = specializations[dataIndex]
              const deptName = specDeptMap[specName]
              const deptTotal = deptTotals[deptName]
              const specTotal = specTotals[specName]
              return [
                `${deptName} (Всего ${deptTotal})`,
                `${specName} (Всего ${specTotal})`,
              ]
            },
            // Стандартная строка «курс: число (процент)»
            label: (context) => {
              const value = context.raw
              const dataIndex = context.dataIndex
              const specName = specializations[dataIndex]
              const specTotal = specTotals[specName]
              const percentage = specTotal > 0
                  ? ((value / specTotal) * 100).toFixed(1)
                  : '0.0'
              return `${context.dataset.label}: ${value} (${percentage}%)`
            },
            // В футере выводим список групп и их численность внутри данного курса + специальности
            footer: (items) => {
              if (!items.length) return ''
              const dataIndex = items[0].dataIndex   // индекс по специальности
              const datasetIndex = items[0].datasetIndex // индекс по курсу
              const specName = specializations[dataIndex]
              const courseName = courses[datasetIndex]
              const key = `${specName}|${courseName}`
              const groupsList = groupsMap[key] || []
              if (!groupsList.length) {
                return 'Нет групп'
              }
              // Вернём массив строк, каждая — «- <группа>: <численность>»
              return [
                'Группы данного курса:',
                ...groupsList.map(g => `· ${g.name}: ${g.count}`)
              ]
            }
          }
        },
        legend: {
          position: 'right',
          labels: {
            color: textColor,
            usePointStyle: true,
            pointStyle: 'rect',
            padding: 16
          }
        }
      },
      scales: {
        x: {
          stacked: true,
          grid: { display: false },
          ticks: {
            color: textColor,
            autoSkip: false,
            maxRotation: 45,
            minRotation: 45
          }
        },
        y: {
          stacked: true,
          beginAtZero: true,
          ticks: {
            color: textColor,
            stepSize: 1,
            precision: 0
          },
          grid: {
            color: gridColor,
            lineWidth: 1
          },
          title: {
            display: true,
            text: 'Количество студентов',
            color: textColor
          }
        }
      }
    }
  })
}

watch(
    () => props.data,
    () => {
      // При изменении входных данных пересоздаём график
      if (!props.data.length) return
      initChart()
    },
    { deep: true }
)

onMounted(() => {
  nextTick(initChart)
})
</script>

<style scoped>
.specialty-chart-container {
  display: flex;
  flex-direction: column;
  background: var(--background-color);
  border-radius: var(--border-radius);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  height: 500px;
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.chart-title {
  margin: 0;
  font-size: 1.25rem;
}

.chart-controls {
  flex: 0 0 auto;
}

.chart-scroll-wrapper {
  overflow-x: auto;
  height: 100%;
  padding-bottom: 1rem;
}

.chart-wrapper {
  min-width: 800px;
  height: 100%;
}

@media (max-width: 768px) {
  .specialty-chart-container {
    height: 400px;
    padding: 1rem;
  }
  .chart-wrapper {
    min-width: 600px;
  }
}
</style>
