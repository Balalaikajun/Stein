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
  Chart, BarController, LineController, CategoryScale, LinearScale,
  BarElement, LineElement, PointElement, Tooltip, Legend
} from 'chart.js'

Chart.register(
    BarController, LineController, CategoryScale, LinearScale,
    BarElement, LineElement, PointElement, Tooltip, Legend
)

const props = defineProps({
  data: {
    type: Array,
    default: () => [],
    validator: items => items.every(item =>
        'Year' in item &&
        'Month' in item &&
        'Count' in item &&
        'ExcellentCount' in item &&
        'GoodCount' in item &&
        'NormalCount' in item &&
        'FallingCount' in item
    )
  },
  title: { type: String, default: 'Динамика успеваемости' }
})
const chart = ref(null)
let chartInstance = null

function initChart () {
  if (chartInstance) chartInstance.destroy()
  if (!props.data.length) return

  const s = getComputedStyle(document.documentElement)
  const textColor = s.getPropertyValue('--text-color').trim()
  const bg = s.getPropertyValue('--background-color').trim()
  const grid = s.getPropertyValue('--secondary-background-color').trim()

  const colors = {
    excellent: s.getPropertyValue('--primary-color').trim(),
    good: s.getPropertyValue('--secondary-color').trim(),
    normal: s.getPropertyValue('--accent-color').trim(),
    falling: s.getPropertyValue('--error-color').trim(),
    qualityLine: s.getPropertyValue('--secondary-color').trim(),
    generalLine: s.getPropertyValue('--primary-color').trim(),
  }

  chartInstance = new Chart(chart.value, {
    type: 'bar',
    data: {
      labels: props.data.map(i => `${i.Month}/${i.Year}`),
      datasets: [

        // бары
        {
          label: 'Отличники',
          data: props.data.map(i => i.ExcellentCount),
          backgroundColor: colors.excellent,
          stack: '0',
          order: 1
        },
        {
          label: 'Хорошисты',
          data: props.data.map(i => i.GoodCount),
          backgroundColor: colors.good,
          stack: '0',
          order: 1
        },
        {
          label: 'Удовлетворительно',
          data: props.data.map(i => i.NormalCount),
          backgroundColor: colors.normal,
          stack: '0',
          order: 1
        },
        {
          label: 'Неуспевающие',
          data: props.data.map(i => i.FallingCount),
          backgroundColor: colors.falling,
          stack: '0',
          order: 1
        },
        // линии
        {
          label: 'Качественная успеваемость',
          type: 'line',
          data: props.data.map(i => ((i.ExcellentCount + i.GoodCount) / i.Count * 100).toFixed(1)),
          borderColor: colors.qualityLine,
          tension: 0.4,
          yAxisID: 'y1',
          pointRadius: 4,
          order: 0,
          fill: false
        },
        {
          label: 'Общая успеваемость',
          type: 'line',
          data: props.data.map(i => ((i.Count - i.FallingCount) / i.Count * 100).toFixed(1)),
          borderColor: colors.generalLine,
          tension: 0.4,
          yAxisID: 'y1',
          pointRadius: 4,
          order: 0,
          fill: false
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,

      interaction: {
        mode: 'index',
        intersect: false
      },

      plugins: {
        legend: {
          position: 'right',
          labels: { color: textColor, padding: 16, usePointStyle: true }
        },
        tooltip: {
          backgroundColor: bg,
          titleColor: textColor,
          bodyColor: textColor,
          borderColor: grid,
          borderWidth: 1,
          padding: 10,
          callbacks: {
            title: items => items[0].label,
            beforeBody: items => `Всего студентов: ${props.data[items[0].dataIndex].Count}`,
            label: context => {
              const v = context.raw
              const item = props.data[context.dataIndex]
              if (context.dataset.type === 'line') {
                return `${context.dataset.label}: ${v}%`
              }
              const pct = ((v / item.Count) * 100).toFixed(1)
              return `${context.dataset.label}: ${v} (${pct}%)`
            }
          }
        }
      },

      scales: {
        x: { stacked: true, ticks: { color: textColor, maxRotation: 45, minRotation: 45 }, grid: { display: false } },
        y: {
          stacked: true, beginAtZero: true, title: { display: true, text: 'Кол-во студентов', color: textColor },
          ticks: { color: textColor, stepSize: 1 }, grid: { color: grid }
        },
        y1: {
          position: 'right', beginAtZero: true, max: 100,
          title: { display: true, text: 'Процент успеваемости', color: textColor },
          ticks: { color: textColor, callback: v => `${v}%` },
          grid: { drawOnChartArea: false }
        }
      }
    }
  })
}

watch(() => props.data, () => nextTick(initChart), { deep: true })
onMounted(() => nextTick(initChart))
</script>

<style scoped>
.specialty-chart-container {
  display: flex;
  flex-direction: column;
  background: var(--background-color);
  border-radius: var(--border-radius);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  padding: 1.5rem;
  height: 100%;

}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.chart-title {
  font-size: 1.25rem;
  color: var(--text-color);
}

.chart-controls ::v-deep > * {
  margin-left: 0.5rem;
}

.chart-scroll-wrapper {
  overflow-x: auto;
  flex: 1;
  padding-bottom: 0.5rem;
}

.chart-scroll-wrapper::-webkit-scrollbar {
  height: 8px;
}

.chart-scroll-wrapper::-webkit-scrollbar-thumb {
  background: var(--secondary-color);
  border-radius: var(--border-radius);
}

.chart-wrapper {
  min-width: 700px;
  height: 100%;
}

@media (max-width: 768px) {
  .specialty-chart-container {
    padding: 1rem;
  }

  .chart-wrapper {
    min-width: 500px;
  }
}
</style>
