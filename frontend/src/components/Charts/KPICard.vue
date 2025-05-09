<template>
  <div class="kpi-card">
    <h3 class="kpi-title">{{ title }}</h3>
    <div class="kpi-value">{{ value }}</div>
  </div>
</template>

<script setup>
defineProps({
  title: String,
  value: [Number, String]
})
</script>

<style scoped>
.kpi-card {
  background: var(--background-color);
  padding: 1rem;
  border-radius: var(--border-radius);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.kpi-title {
  color: var(--secondary-text-color);
  font-size: 1rem;
  margin-bottom: 0.5rem;
}

.kpi-value {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-color);
}
</style>