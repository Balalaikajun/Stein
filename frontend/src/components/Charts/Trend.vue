<template>
  <div class="trend-chart">
    <div class="chart-header">
      <h3 class="chart-title">{{ title }}</h3>
      <div class="chart-controls">
        <div class="pagination-controls">
          <button
              @click="$emit('update:page', page - 1)"
              :disabled="page <= 1"
              class="pagination-button"
          >&lt;
          </button>
          <span class="page-indicator">
            Страница {{ page }} из {{ totalPages }}
          </span>
          <button
              @click="$emit('update:page', page + 1)"
              :disabled="page >= totalPages"
              class="pagination-button"
          >&gt;
          </button>
        </div>
      </div>
    </div>
    <div class="chart-container">
      <canvas ref="chart"></canvas>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted, nextTick } from 'vue'
import {
  Chart, LineController, CategoryScale, LinearScale,
  PointElement, LineElement, Tooltip, Legend
} from 'chart.js'

Chart.register(
    LineController, CategoryScale, LinearScale,
    PointElement, LineElement, Tooltip, Legend
)

const props = defineProps({
  // Данные уже «нарезанные» по странице, передаёт родитель
  labels: { type: Array, default: () => [] },
  values: { type: Array, default: () => [] },
  performanceData: {
    type: Array, default: () => [],
    validator: arr =>
        arr.every(item =>
            ['Excellent', 'Good', 'Normal', 'Falling'].every(k => k in item)
        )
  },
  title: { type: String, default: 'Тренд успеваемости' },
  page: { type: Number, default: 1 },
  totalPages: { type: Number, default: 1 }
})

const chart = ref(null)
let chartInstance = null

function initChart () {
  if (chartInstance) chartInstance.destroy()

  const styles = getComputedStyle(document.documentElement)
  const primaryColor = styles.getPropertyValue('--primary-color')?.trim() || '#5B00E1'
  const bgColor = styles.getPropertyValue('--background-color')?.trim() || '#fff'
  const textColor = styles.getPropertyValue('--text-color')?.trim() || '#333'
  const secColor = styles.getPropertyValue('--secondary-color')?.trim() || '#e0e0e0'
  const ticksColor = styles.getPropertyValue('--secondary-text-color')?.trim() || '#666'

  chartInstance = new Chart(chart.value, {
    type: 'line',
    data: {
      labels: props.labels,
      datasets: [{
        label: 'Средний балл',
        data: props.values,
        borderColor: primaryColor,
        backgroundColor: 'rgba(91, 0, 225, 0.1)',
        tension: 0.4, pointRadius: 5,
        pointHoverRadius: 7, borderWidth: 3,
        pointBackgroundColor: primaryColor
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      layout: { padding: { bottom: 40 } },
      plugins: {
        tooltip: {
          backgroundColor: bgColor,
          titleColor: textColor,
          bodyColor: textColor,
          borderColor: secColor,
          borderWidth: 1,
          padding: { x: 12, y: 8 },
          bodySpacing: 6,
          callbacks: {
            label: ctx => `Средний балл: ${ctx.parsed.y}`,
            afterBody: ctx => {
              const idx = ctx[0].dataIndex
              const perf = props.performanceData[idx] || {}
              return [
                `Отличники: ${perf.Excellent || 0}`,
                `Хорошисты: ${perf.Good || 0}`,
                `Троечники: ${perf.Normal || 0}`,
                `Отстающие: ${perf.Falling || 0}`
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
            padding: 10,
            callback(value) {
              // Получаем текст метки
              const label = this.getLabelForValue(value) || ''
              // Теперь можно резать на слова
              const words = label.split(' ')
              const lines = []
              let line = ''
              const maxChars = 12

              words.forEach(w => {
                if ((line + ' ' + w).trim().length <= maxChars) {
                  line = (line + ' ' + w).trim()
                } else {
                  lines.push(line)
                  line = w
                }
              })
              if (line) lines.push(line)
              return lines
            }
          }
        },
        y: {
          min: 0, max: 100,
          ticks: { color: ticksColor },
          grid: { color: 'rgba(0,0,0,0.05)' }
        }
      }
    }
  })
}

watch(
    () => [props.labels, props.values, props.performanceData],
    () => nextTick(initChart),
    { deep: true }
)

onMounted(() => nextTick(initChart))
</script>

<style scoped>
.trend-chart {
  display: flex;
  flex-direction: column;
  background: var(--background-color);
  border-radius: var(--border-radius);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  min-height: 400px;
  overflow: visible;
  position: relative;
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
  display: flex;
  align-items: center;
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-left: auto;
}

.pagination-button {
  padding: 6px 12px;
  border-radius: 50%;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid var(--secondary-color);
  background: var(--background-color);
  cursor: pointer;
  transition: all 0.2s;
}

.pagination-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-button:hover:not(:disabled) {
  background: var(--primary-color);
  color: white;
  border-color: var(--primary-color);
}

.page-indicator {
  font-size: 0.9rem;
  color: var(--secondary-text-color);
}

.chart-container {
  flex: 1;
  position: relative;
  min-height: 300px;
  overflow-x: auto;
}

.trend-chart canvas {
  width: 100% !important;
  min-width: 600px;
  height: auto !important;
  min-height: 300px;
}

@media (max-width: 768px) {
  .trend-chart {
    min-height: 350px;
    padding: 1rem;
  }

  .chart-container {
    min-height: 250px;
  }

  .trend-chart canvas {
    min-width: 100%;
  }
}
</style>