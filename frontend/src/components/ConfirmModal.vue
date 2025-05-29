<!-- ConfirmationModal.vue -->
<template>
  <teleport to="body">
    <div
        v-if="visible"
        class="modal-overlay"
        @click.self="emitCancel"
    >
      <div class="confirmation-modal">
        <p class="modal-message">{{ message }}</p>
        <div class="modal-actions">
          <button
              class="confirm-button"
              @click="emitConfirm"
          >
            {{ confirmText }}
          </button>
          <button
              class="cancel-button"
              @click="emitCancel"
          >
            {{ cancelText }}
          </button>
        </div>
      </div>
    </div>
  </teleport>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'

const props = defineProps({
  visible: Boolean,
  message: {
    type: String,
    default: 'Вы уверены, что хотите выполнить это действие?'
  },
  confirmText: {
    type: String,
    default: 'Подтвердить'
  },
  cancelText: {
    type: String,
    default: 'Отмена'
  }
})

const emit = defineEmits(['confirm', 'cancel'])

function handleKeydown(e) {
  if (e.key === 'Escape') {
    emitCancel()
  }
}

function emitConfirm() {
  emit('confirm')
}

function emitCancel() {
  emit('cancel')
}

onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
})

onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleKeydown)
})
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.confirmation-modal {
  background: var(--secondary-background-color);
  padding: 2rem;
  border-radius: 16px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.5);
  min-width: 400px;
  max-width: 90%;
}

.modal-message {
  text-align: center;
  margin-bottom: 2rem;
  font-family: Roboto, serif;
  color: var(--text-color);
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
}

.confirm-button,
.cancel-button {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: opacity 0.3s ease;
}

.confirm-button {
  background-color: var(--primary-color);
  color: var(--background-color);
}

.cancel-button {
  background-color: var(--secondary-background-color);
  color: var(--text-color);
  border: 1px solid var(--text-color);
}

.confirm-button:hover,
.cancel-button:hover {
  opacity: 0.9;
}
</style>