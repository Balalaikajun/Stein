<template>
  <span :class="['status-badge', status ? 'active' : 'inactive']">
    {{ status ? 'Активен' : 'Неактивен' }}
  </span>
</template>

<script setup>
defineProps({
  status: {
    type: Boolean,
    required: true
  }
})
</script>

<style scoped>
.status-badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 12px;
  display: inline-block;
}

.status-badge.active {
  background: #e8f5e9;
  color: #2e7d32;
}

.status-badge.inactive {
  background: #ffebee;
  color: #c62828;
}
</style>