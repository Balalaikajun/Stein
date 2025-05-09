<template>
  <div class="pie-chart-container">
    <div class="chart-wrapper">
      <canvas ref="chart"></canvas>
    </div>
    <div v-if="showLegend" class="legend-container">
      <div
          v-for="(label, index) in labels"
          :key="label"
          class="legend-item"
      >
        <span
            class="legend-color"
            :style="{ backgroundColor: colors[index] }"
        ></span>
        <span class="legend-text">{{ label }}</span>
        <span class="legend-percent">{{ percentages[index] }}%</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { Chart, PieController, ArcElement, Tooltip, Legend } from 'chart.js'

Chart.register(PieController, ArcElement, Tooltip, Legend)

const props = defineProps({
  labels: {
    type: Array,
    required: true
  },
  values: {
    type: Array,
    required: true
  },
  colors: {
    type: Array,
    default: () => ['#5B00E1', '#7C4DFF', '#FFD740', '#FF5252']
  },
  title: {
    type: String,
    default: ''
  },
  showLegend: {
    type: Boolean,
    default: true
  },
  animate: {
    type: Boolean,
    default: true
  }
})

const chart = ref(null)
let chartInstance = null

const total = computed(() => props.values.reduce((a, b) => a + b, 0))
const percentages = computed(() =>
    props.values.map(value => ((value / total.value) * 100).toFixed(1))
)

const initChart = () => {
  if (chartInstance) {
    chartInstance.destroy()
  }

  const ctx = chart.value.getContext('2d')

  chartInstance = new Chart(ctx, {
    type: 'pie',
    data: {
      labels: props.labels,
      datasets: [{
        data: props.values,
        backgroundColor: props.colors,
        borderColor: 'white',
        borderWidth: 2,
        hoverOffset: 8
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: !!props.title,
          text: props.title,
          font: {
            size: 16,
            weight: '600'
          }
        },
        tooltip: {
          callbacks: {
            label: (context) => {
              const label = context.label || ''
              const value = context.raw || 0
              const percent = percentages.value[context.dataIndex]
              return `${label}: ${value} (${percent}%)`
            }
          }
        },
        legend: {
          display: false
        }
      },
      animation: {
        duration: props.animate ? 1000 : 0
      }
    }
  })
}

watch(() => [props.values, props.labels], () => {
  initChart()
})

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