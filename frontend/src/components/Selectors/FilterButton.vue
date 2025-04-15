<template>
  <button
      class="filter-btn"
      :class="{ 'filter-btn--open': isOpen, 'filter-btn--active': hasValue }"
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
const props = defineProps({
  title:      { type: String, required: true },
  isOpen:     { type: Boolean, default: false },
  badgeCount: { type: Number, default: 0 },
  hasValue:   { type: Boolean, default: false }
})
</script>

<style scoped>
.filter-btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 12px;
  background: #fff;
  border: 1px solid #ccc;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  position: relative;
  transition: border-color .2s, background .2s;
}
.filter-btn:hover { border-color: #007bff; }
.filter-btn--open,
.filter-btn--active { background: #f0f8ff; border-color: #007bff; }
.filter-btn__title { white-space: nowrap; }
.filter-btn__badge {
  background: #007bff;
  color: #fff;
  border-radius: 10px;
  padding: 0 6px;
  font-size: 12px;
}
.filter-btn__icon {
  width: 16px; height: 16px;
  fill: currentColor;
  transform: rotate(0deg);
  transition: transform .2s;
}
.filter-btn--open .filter-btn__icon {
  transform: rotate(180deg);
}
</style>
