<template>
  <button
      class="filter-btn"
      :class="{
      'filter-btn--open': isOpen,
      'filter-btn--active': hasValue
    }"
      type="button"
      @click="$emit('toggle', $event)"
      :aria-expanded="isOpen.toString()"
      aria-haspopup="dialog"
  >
    <span class="filter-btn__title">{{ title }}</span>
    <span v-if="badgeCount" class="filter-btn__badge">{{ badgeCount }}</span>
    <svg class="filter-btn__icon" viewBox="0 0 24 24" aria-hidden="true">
      <path d="M7 10l5 5 5-5H7z"/>
    </svg>
  </button>
</template>

<script setup>
defineProps({
  title: { type: String, required: true },
  isOpen: { type: Boolean, default: false },
  badgeCount: { type: Number, default: 0 },
  hasValue: { type: Boolean, default: false }
})
</script>

<style scoped>
.filter-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: var(--background-color);
  border: 1px solid var(--secondary-text-color);
  border-radius: var(--border-radius);
  font-size: 0.875rem;
  transition: all var(--transition-duration) var(--transition-timing);
  cursor: pointer;
}

.filter-btn:hover {
  border-color: var(--primary-color);
  background: var(--hover-color);
}

.filter-btn--open,
.filter-btn--active {
  background: var(--active-bg-color);
  border-color: var(--primary-color);
}

.filter-btn__badge {
  background: var(--primary-color);
  color: var(--background-color);
  border-radius: 0.625rem;
  padding: 0.125rem 0.5rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.filter-btn__icon {
  width: 1rem;
  height: 1rem;
  transition: transform var(--transition-duration) var(--transition-timing);
}

.filter-btn--open .filter-btn__icon {
  transform: rotate(180deg);
}
</style>