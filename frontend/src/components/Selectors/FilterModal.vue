<template>
  <Teleport to="body">
    <transition name="fade">
      <div v-if="visible" class="modal-overlay" @click="cancel">
        <div
            class="modal-window"
            :style="positionStyle"
            role="dialog"
            aria-modal="true"
            @click.stop
            ref="modalRef"
        >
          <header class="modal-header">
            <slot name="header">
              <h3 class="modal-title">{{ title }}</h3>
            </slot>
          </header>
          <div class="modal-body">
            <slot name="body"></slot>
          </div>
          <footer class="modal-footer">
            <slot name="footer">
              <button class="btn btn--apply" @click="apply">Применить</button>
              <button class="btn btn--cancel" @click="cancel">Отмена</button>
            </slot>
          </footer>
        </div>
      </div>
    </transition>
  </Teleport>
</template>

<script setup>
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { onClickOutside } from '@vueuse/core'

const props = defineProps({
  title: { type: String, default: '' },
  visible: { type: Boolean, default: false },
  anchorRect: { type: Object, default: null }
})

const emit = defineEmits(['apply', 'cancel'])
const modalRef = ref(null)

const positionStyle = computed(() => {
  if (!props.anchorRect) return {}
  return {
    position: 'fixed',
    top: `${props.anchorRect.bottom + window.scrollY + 4}px`,
    left: `${props.anchorRect.left + window.scrollX}px`,
    minWidth: `${props.anchorRect.width}px`,
    zIndex: 1001
  }
})

// Обработчики событий
function apply() { emit('apply') }
function cancel() { emit('cancel') }

onMounted(() => {
  onClickOutside(modalRef, cancel)
  window.addEventListener('keydown', onEsc)
})

function onEsc(e) {
  if (e.key === 'Escape') cancel()
}

onBeforeUnmount(() => {
  window.removeEventListener('keydown', onEsc)
})
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s var(--transition-timing);
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.2);
  z-index: 1000;
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding-top: 1rem;
}

.modal-window {
  background: var(--background-color);
  border-radius: var(--border-radius);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  max-height: 80vh;
  overflow: hidden;
}

.modal-header {
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--secondary-background-color);
}

.modal-title {
  margin: 0;
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-color);
}

.modal-body {
  padding: 1rem;
  overflow-y: auto;
}

.modal-footer {
  padding: 0.75rem 1rem;
  border-top: 1px solid var(--secondary-background-color);
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.btn {
  padding: 0.5rem 1rem;
  border-radius: var(--border-radius);
  font-size: 0.875rem;
  cursor: pointer;
  border: none;
  transition: background-color var(--transition-duration) var(--transition-timing);
}

.btn--apply {
  background: var(--primary-color);
  color: var(--background-color);
}

.btn--apply:hover {
  background: var(--secondary-color);
}

.btn--cancel {
  background: var(--secondary-background-color);
  color: var(--text-color);
}

.btn--cancel:hover {
  background: var(--hover-color);
}

:deep(.radio-group) {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

:deep(.radio-option) {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem;
  border-radius: calc(var(--border-radius) / 2);
  transition: background-color var(--transition-duration) var(--transition-timing);
}

:deep(.radio-option:hover) {
  background-color: var(--hover-color);
}

:deep(input[type="radio"]) {
  width: 1rem;
  height: 1rem;
  accent-color: var(--primary-color);
}
</style>