<template>
  <div class="orders-chart">
    <div class="chart-header">
      <h3 class="chart-title">{{ title }}</h3>
      <div class="chart-controls">
        <slot name="controls"></slot>
      </div>
    </div>

    <!-- График -->
    <div class="chart-body">
      <canvas ref="chart"></canvas>
    </div>

    <!-- Блок с общей численностью приказов (теперь под графиком) -->
    <div class="total-orders">
      <span>Всего приказов:</span>
      <span class="total-value">{{ totalOrders }}</span>
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
  orderData: {
    type: Object,
    required: true,
    validator: obj => obj !== null && typeof obj === 'object'
  },
  totalOrders: {
    type: Number,
    required: true
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

const chartLabels = ref(orderedKeys.map(key => keyToLabel[key] || key))
const chartValues = ref([])

function initChart () {
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
          callbacks: {
            label: context => `Количество: ${context.raw}`
          }
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
            callback (value) {
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
    () => props.orderData,
    newObj => {
      const arr = orderedKeys.map(key => {
        const v = newObj[key]
        return Number.isInteger(v) ? v : 0
      })
      chartValues.value = arr

      nextTick(() => {
        initChart()
      })
    },
    { immediate: true }
)

onMounted(() => {
  nextTick(() => {
    if (!chartInstance && chartValues.value.length) {
      initChart()
    }
  })
})
</script>

<style scoped>
.orders-chart {
  display: flex;
  flex-direction: column;
  background: var(--background-color);
  border-radius: var(--border-radius);
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
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
  position: relative;
  flex: 1 1 auto;
  height: 350px; /* фиксируем высоту, чтобы канвас занял всегда определённое место */
}

.chart-body canvas {
  position: absolute;
  top: 0;
  left: 0;
  width: 100% !important;
  height: 100% !important;
  display: block;
}

/* Теперь блок с общим числом стоит ПОД графиком, не покрывая его */
.total-orders {
  margin-top: 0.75rem;
  display: flex;
  justify-content: flex-end;
  font-size: 0.95rem;
  color: var(--text-color);
}

.total-orders .total-value {
  font-weight: bold;
  margin-left: 0.25rem;
}

@media (max-width: 768px) {
  .orders-chart {
    padding: 1rem;
  }

  .chart-body {
    height: 250px;
  }

  .total-orders {
    font-size: 0.85rem;
  }
}
</style>
