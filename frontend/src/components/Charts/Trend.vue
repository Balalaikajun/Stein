<template>
  <div class="trend-chart">
    <div class="chart-header">
      <h3 class="chart-title">{{ title }}</h3>
      <div class="chart-controls">
        <slot name="controls"></slot>
      </div>
    </div>
    <canvas ref="chart"></canvas>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, nextTick } from 'vue'
import {
  Chart,
  LineController,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Tooltip,
  Legend
} from 'chart.js'

Chart.register(
    LineController,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Tooltip,
    Legend
)

const props = defineProps({
  labels: { type: Array, default: () => [] },
  values: { type: Array, default: () => [] },
  performanceData: {
    type: Array,
    default: () => [],
    validator: arr => arr.every(
        item => 'Excellent' in item && 'Good' in item && 'Normal' in item && 'Falling' in item
    )
  },
  title: { type: String, default: 'Тренд успеваемости' }
})

const chart = ref(null)
let chartInstance = null

const wrapLabel = (label, maxChars = 10) => {
  const words = label.split(' ')
  const lines = []
  let line = ''
  words.forEach(word => {
    if ((line + ' ' + word).trim().length <= maxChars) {
      line = (line + ' ' + word).trim()
    } else {
      if (line) lines.push(line)
      line = word
    }
  })
  if (line) lines.push(line)
  return lines
}

const initChart = () => {
  if (chartInstance) chartInstance.destroy()

  const styles = getComputedStyle(document.documentElement)
  const primaryColor = styles.getPropertyValue('--primary-color').trim() || '#5B00E1'
  const bgColor = styles.getPropertyValue('--background-color').trim() || '#ffffff'
  const textColor = styles.getPropertyValue('--text-color').trim() || '#333333'
  const secondaryColor = styles.getPropertyValue('--secondary-color').trim() || '#e0e0e0'
  const ticksColor = styles.getPropertyValue('--secondary-text-color').trim() || '#666666'

  chartInstance = new Chart(chart.value, {
    type: 'line',
    data: {
      labels: props.labels,
      datasets: [
        {
          label: 'Средний балл',
          data: props.values,
          borderColor: primaryColor,
          backgroundColor: 'rgba(91, 0, 225, 0.1)',
          tension: 0.4,
          pointRadius: 5,
          pointHoverRadius: 7,
          borderWidth: 3,
          pointBackgroundColor: primaryColor
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      layout: {
        padding: { bottom: 40 }   // добавляем отступ снизу для место под подписи
      },
      plugins: {
        tooltip: {
          backgroundColor: bgColor,
          titleColor: textColor,
          bodyColor: textColor,
          borderColor: secondaryColor,
          borderWidth: 1,
          padding: { x: 12, y: 8 },
          bodySpacing: 6,
          callbacks: {
            label: context => `Средний балл: ${context.parsed.y}`,
            afterBody: context => {
              const idx = context[0].dataIndex
              const perf = props.performanceData[idx] || { Excellent: 0, Good: 0, Normal: 0, Falling: 0 }
              return [
                `Отличники: ${perf.Excellent}`,
                `Хорошисты: ${perf.Good}`,
                `Троечники: ${perf.Normal}`,
                `Отстающие: ${perf.Falling}`
              ]
            }
          }
        },
        legend: { display: false }
      },
      scales: {
        x: {
          grid: { display: false },
          ticks: {
            color: ticksColor,
            autoSkip: false,
            maxRotation: 45,
            minRotation: 45,
            padding: 10,           // немного отступа вокруг тиков
            callback: function(val) {
              const label = this.getLabelForValue(val) || ''
              return wrapLabel(label, 12)
            }
          }
        },
        y: {
          min: 0,
          max: 100,
          ticks: { color: ticksColor, callback: value => `${value}` },
          grid: { color: 'rgba(0, 0, 0, 0.05)' }
        }
      }
    }
  })
}

watch(
    () => [props.labels, props.values, props.performanceData],
    initChart,
    { deep: true }
)

onMounted(() => {
  nextTick(initChart)
})
</script>

<style scoped>
.trend-chart {
  display: flex;
  flex-direction: column;
  background: var(--background-color);
  border-radius: var(--border-radius);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  height: 400px;
  overflow: hidden;       /* предотвращаем выход canvas за пределы */
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

.trend-chart canvas {
  flex: 1;
  width: 100% !important;
  height: 100% !important;
}

@media (max-width: 768px) {
  .trend-chart {
    height: 300px;
    padding: 1rem;
  }
}
</style>