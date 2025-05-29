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

Chart.register(
    BarController,
    CategoryScale,
    LinearScale,
    BarElement,
    Tooltip,
    Legend
)

const props = defineProps({
  data: {
    type: Array,
    default: () => [],
    validator: items => items.every(item =>
        'name' in item &&
        'total' in item &&
        'courses' in item &&
        Object.values(item.courses).every(Number.isInteger)
    )
  },
  title: { type: String, default: 'Распределение студентов' }
})

const chart = ref(null)
let chartInstance = null

const generateCourseColors = (courseCount) => {
  const styles = getComputedStyle(document.documentElement)
  const baseColor = styles.getPropertyValue('--primary-color') || '#5B00E1'

  return Array.from({ length: courseCount }, (_, i) => {
    const opacity = 1 - (i * 0.15)
    return `rgba(${parseInt(baseColor.slice(1, 3), 16)}, ${parseInt(baseColor.slice(3, 5), 16)}, ${parseInt(baseColor.slice(5, 7), 16)}, ${opacity})`
  })
}

const initChart = () => {
  if (chartInstance) chartInstance.destroy()
  if (!props.data.length) return

  const styles = getComputedStyle(document.documentElement)
  const textColor = styles.getPropertyValue('--text-color')
  const bgColor = styles.getPropertyValue('--background-color')
  const gridColor = styles.getPropertyValue('--secondary-background-color')

  const courses = Object.keys(props.data[0].courses).sort()
  const courseColors = generateCourseColors(courses.length)

  chartInstance = new Chart(chart.value, {
    type: 'bar',
    data: {
      labels: props.data.map(item => item.name),
      datasets: courses.map((course, index) => ({
        label: `${course} курс`,
        data: props.data.map(item => item.courses[course]),
        backgroundColor: courseColors[index],
        borderColor: bgColor,
        borderWidth: 1,
        hoverBackgroundColor: courseColors[index] + 'CC',
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
          borderColor: gridColor,
          borderWidth: 1,
          padding: 12,
          callbacks: {
            title: (items) => items[0].label,
            beforeBody: (items) => {
              const total = props.data[items[0].dataIndex].total
              return `Всего студентов: ${total}`
            },
            label: (context) => {
              const value = context.raw
              const total = props.data[context.dataIndex].total
              const percentage = ((value / total) * 100).toFixed(1)
              return `${context.dataset.label}: ${value} (${percentage}%)`
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
      if (props.data.length && !props.data[0].courses) {
        console.error('Invalid data structure')
        return
      }
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
