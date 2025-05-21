<template>
  <div class="orders-chart">
    <div class="chart-header">
      <h3 class="chart-title">{{ title }}</h3>
      <div class="chart-controls">
        <slot name="controls"></slot>
      </div>
    </div>
    <div class="chart-body">
      <canvas ref="chart"></canvas>
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
  Title,
  Tooltip,
  Legend
} from 'chart.js'

Chart.register(
    BarController,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
)

const props = defineProps({
  orderDto: {
    type: Object,
    required: true,
    validator: obj => obj !== null && typeof obj === 'object' && 'data' in obj
  },
  title: {
    type: String,
    default: 'Статистика приказов'
  }
})

const chart = ref(null)
let chartInstance = null

const orderedKeys = [
  'Enrollment',
  'TransferFromOtherInstitution',
  'TransferToOtherInstitution',
  'ReinstatementFromExpelled',
  'ReinstatementFromAcademy',
  'AcademicLeave',
  'TransferBetweenGroups',
  'Expulsion',
  'Graduation'
]

const keyToLabel = {
  Enrollment: 'Зачисление',
  TransferFromOtherInstitution: 'Перевод из вуза',
  TransferToOtherInstitution: 'Перевод в вуз',
  ReinstatementFromExpelled: 'Восст. из отчисления',
  ReinstatementFromAcademy: 'Восст. из академа',
  AcademicLeave: 'Академотпуск',
  TransferBetweenGroups: 'Перевод между группами',
  Expulsion: 'Отчисление',
  Graduation: 'Выпуск'
}

const chartLabels = ref([])
const chartValues = ref([])

const initChart = () => {
  if (!chart.value || !chart.value.getContext) return
  if (chartInstance) {
    chartInstance.destroy()
    chartInstance = null
  }

  const styles = getComputedStyle(document.documentElement)
  const primaryColor = styles.getPropertyValue('--primary-color').trim() || '#5B00E1'
  const secondaryColor = styles.getPropertyValue('--secondary-color').trim() || '#7C4DFF'
  const bgColor = styles.getPropertyValue('--background-color').trim() || '#ffffff'
  const textColor = styles.getPropertyValue('--text-color').trim() || '#212121'
  const ticksColor = styles.getPropertyValue('--secondary-text-color').trim() || '#757575'

  chartInstance = new Chart(chart.value, {
    type: 'bar',
    data: {
      labels: chartLabels.value,
      datasets: [{
        label: 'Количество приказов',
        data: chartValues.value,
        backgroundColor: primaryColor,
        borderColor: secondaryColor,
        borderWidth: 1,
        hoverBackgroundColor: secondaryColor,
        barPercentage: 0.8,
        categoryPercentage: 0.9
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false },
        tooltip: {
          backgroundColor: bgColor,
          titleColor: textColor,
          bodyColor: textColor,
          borderColor: secondaryColor,
          borderWidth: 1,
          padding: { x: 12, y: 8 },
          callbacks: { label: context => `Количество: ${context.raw}` }
        }
      },
      scales: {
        x: {
          grid: { display: false },
          ticks: {
            color: ticksColor,
            autoSkip: false,
            maxRotation: 45,
            minRotation: 45,
            callback: function(value) {
              const label = this.getLabelForValue(value) || ''
              const maxCharsPerLine = 10
              const words = label.split(' ')
              const lines = []
              let line = ''
              words.forEach(word => {
                if ((line + ' ' + word).trim().length <= maxCharsPerLine) {
                  line = (line + ' ' + word).trim()
                } else {
                  if (line) lines.push(line)
                  line = word
                }
              })
              if (line) lines.push(line)
              return lines
            }
          }
        },
        y: {
          beginAtZero: true,
          ticks: {
            color: ticksColor,
            stepSize: 1,
            precision: 0
          },
          grid: { color: 'rgba(0, 0, 0, 0.05)' }
        }
      }
    }
  })
}

watch(
    () => props.orderDto,
    dto => {
      const dataObj = dto?.data ?? {}

      chartLabels.value = orderedKeys.map(key => keyToLabel[key] || key)
      chartValues.value = orderedKeys.map(key => {
        const v = dataObj[key]
        return Number.isInteger(v) ? v : 0
      })

      if (chartLabels.value.length === chartValues.value.length) {
        nextTick(() => {
          initChart()
        })
      }
    },
    { immediate: true }
)

onMounted(() => {
  nextTick(() => {
    if (!chartInstance && chartLabels.value.length && chartValues.value.length) {
      initChart()
    }
  })
})
</script>

<style scoped>
.orders-chart {
  display: flex;
  flex-direction: column;
  height: 400px;
  background: var(--background-color);
  border-radius: var(--border-radius);
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
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
  /* сюда попадёт слот "controls" */
}

.chart-body {
  flex: 1 1 auto;
  position: relative;
}

.chart-body canvas {
  position: absolute;
  top: 0;
  left: 0;
  width: 100% !important;
  height: 100% !important;
  display: block;
}

@media (max-width: 768px) {
  .orders-chart {
    height: 300px;
    padding: 1rem;
  }
}
</style>
