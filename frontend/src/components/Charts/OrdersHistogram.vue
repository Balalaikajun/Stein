<template>
  <div class="orders-chart">
    <div class="chart-header">
      <h3 class="chart-title">{{ title }}</h3>
      <!-- Слот справа -->
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
  labels: {
    type: Array,
    default: () => [],
    validator: value => value.every(item => typeof item === 'string')
  },
  values: {
    type: Array,
    default: () => [],
    validator: value => value.every(item => Number.isInteger(item)),
  },
  title: { type: String, default: 'Статистика приказов' }
})

const chart = ref(null)
let chartInstance = null

const initChart = () => {
  if (chartInstance) chartInstance.destroy()

  const styles = getComputedStyle(document.documentElement)
  const primaryColor = styles.getPropertyValue('--primary-color').trim() || '#5B00E1'
  const secondaryColor = styles.getPropertyValue('--secondary-color').trim() || '#7C4DFF'
  const bgColor = styles.getPropertyValue('--background-color').trim() || '#ffffff'
  const textColor = styles.getPropertyValue('--text-color').trim() || '#212121'
  const ticksColor = styles.getPropertyValue('--secondary-text-color').trim() || '#757575'

  chartInstance = new Chart(chart.value, {
    type: 'bar',
    data: {
      labels: props.labels,
      datasets: [{
        label: 'Количество приказов',
        data: props.values,
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
        legend: {
          display: false
        },
        tooltip: {
          backgroundColor: bgColor,
          titleColor: textColor,
          bodyColor: textColor,
          borderColor: secondaryColor,
          borderWidth: 1,
          padding: { x: 12, y: 8 },
          callbacks: {
            label: (context) => `Количество: ${context.raw}`
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
            // функция-обёртка для переноса по словам
            callback: function(value) {
              const label = this.getLabelForValue(value) || '';
              const maxCharsPerLine = 10;  // настраивайте по необходимости
              const words = label.split(' ');
              const lines = [];
              let line = '';

              words.forEach(word => {
                // если добавление слова не превышает лимит — продолжаем в той же строке
                if ((line + ' ' + word).trim().length <= maxCharsPerLine) {
                  line = (line + ' ' + word).trim();
                } else {
                  // иначе сохраняем предыдущую строку и начинаем новую
                  if (line) lines.push(line);
                  line = word;
                }
              });
              // не забываем последнюю
              if (line) lines.push(line);

              return lines;
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
          grid: {
            color: 'rgba(0, 0, 0, 0.05)'
          }
        }
      }
    }
  })
}

watch(
    () => [props.labels, props.values],
    () => {
      if (props.labels.length !== props.values.length) {
        console.error('Labels and values arrays must have the same length')
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
.orders-chart {
  display: flex;
  flex-direction: column;
  height: 400px;           /* или 300px для мобилки */
  background: var(--background-color);
  border-radius: var(--border-radius);
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.chart-header {
  display: flex;                  /* включаем flex */
  justify-content: space-between; /* распределяем элементы по краям */
  align-items: center;            /* центрируем по вертикали */
  margin-bottom: 1rem;
}

.chart-body {
  flex: 1 1 auto;          /* тело — занимает всё оставшееся место */
  position: relative;
}

.chart-body canvas {
  position: absolute;
  top: 0; left: 0;
  width: 100% !important;
  height: 100% !important;
  display: block;
}
.chart-title {
  margin: 0;
  font-size: 1.25rem;
}
@media (max-width: 768px) {
  .orders-chart {
    height: 300px;
    padding: 1rem;
  }
}
</style>