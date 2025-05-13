<template>
  <div class="pie-chart-container">
    <div class="chart-wrapper">
      <canvas ref="chart"></canvas>
    </div>
    <div v-if="showLegend" class="legend-container">
      <div
          v-for="segment in pie.segments"
          :key="segment.label"
          class="legend-item"
      >
        <span
            class="legend-color"
            :style="{ backgroundColor: segment.color || defaultColor }"
        ></span>
        <span class="legend-text">{{ segment.label }}</span>
        <span class="legend-percent">{{ percentMap[segment.label] }}%</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { Chart, PieController, ArcElement, Tooltip, Legend } from 'chart.js'

Chart.register(PieController, ArcElement, Tooltip, Legend)

const props = defineProps({
  pie: {
    type: Object,
    required: true,
    // ожидаем { title: string, segments: Array<{ label, value, color? }> }
  },
  showLegend: {
    type: Boolean,
    default: true
  },
  animate: {
    type: Boolean,
    default: true
  },
  defaultColor: {
    type: String,
    default: '#CCCCCC'
  }
})

const chart = ref(null)
let chartInstance = null

// Вынимаем из pie.segments три параллельных массива:
const labels = computed(() => props.pie.segments.map(s => s.label))
const values = computed(() => props.pie.segments.map(s => s.value))
const colors = computed(() =>
    props.pie.segments.map(s => s.color || props.defaultColor)
)

// Считаем проценты и мапим по label для легенды
const total = computed(() => values.value.reduce((sum, v) => sum + v, 0))
const percentMap = computed(() => {
  const m = {}
  values.value.forEach((v, i) => {
    const p = total.value > 0 ? ((v / total.value) * 100).toFixed(1) : '0.0'
    m[labels.value[i]] = p
  })
  return m
})

const initChart = () => {
  if (chartInstance) {
    chartInstance.destroy()
  }

  const ctx = chart.value.getContext('2d')
  chartInstance = new Chart(ctx, {
    type: 'pie',
    data: {
      labels: labels.value,
      datasets: [
        {
          data: values.value,
          backgroundColor: colors.value,
          borderColor: 'white',
          borderWidth: 2,
          hoverOffset: 8
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: !!props.pie.title,
          text: props.pie.title,
          font: {
            size: 16,
            weight: '600'
          }
        },
        tooltip: {
          callbacks: {
            label: ctx => {
              const lbl = ctx.label || ''
              const val = ctx.raw || 0
              const pct = percentMap.value[lbl]
              return `${lbl}: ${val} (${pct}%)`
            }
          }
        },
        legend: { display: false }
      },
      animation: {
        duration: props.animate ? 1000 : 0
      }
    }
  })
}

// Следим за изменениями сегментов
watch(
    () => props.pie.segments,
    () => {
      initChart()
    },
    { deep: true }
)

onMounted(initChart)
</script>

<style scoped>
.pie-chart-container {
  background: var(--background-color);
  border-radius: var(--border-radius);
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  position: relative;
}

.chart-wrapper {
  position: relative;
  height: 300px;
  margin-bottom: 1rem;
}

.legend-container {
  display: grid;
  gap: 0.75rem;
  padding: 0 1rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.9rem;
}

.legend-color {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  display: inline-block;
}

.legend-text {
  flex-grow: 1;
  color: var(--text-color);
}

.legend-percent {
  color: var(--secondary-text-color);
  font-weight: 500;
}

@media (max-width: 768px) {
  .chart-wrapper {
    height: 250px;
  }

  .legend-container {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
