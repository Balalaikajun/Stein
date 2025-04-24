<script setup>
defineProps({
  label: {
    type: String,
    default: 'Открыть модалку'
  },
  variant: {
    type: String,
    default: 'primary',
    validator: (value) => ['primary', 'secondary', 'accent'].includes(value)
  }
})

defineEmits(['click'])
</script>

<template>
  <button
      @click="$emit('click')"
      class="toggle-button"
      :class="[`button-${variant}`]"
  >
    {{ label }}
  </button>
</template>

<style scoped>
.toggle-button {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: all var(--transition-duration) var(--transition-timing);
}

.button-primary {
  background: var(--primary-color);
  color: var(--background-color);
}

.button-primary:hover {
  background: var(--secondary-color);
}

.button-secondary {
  background: var(--secondary-background-color);
  color: var(--text-color);
  border: 1px solid var(--secondary-text-color);
}

.button-secondary:hover {
  background: var(--hover-color);
}

.button-accent {
  background: var(--accent-color);
  color: var(--text-color);
}

.button-accent:hover {
  opacity: 0.9;
}
</style>