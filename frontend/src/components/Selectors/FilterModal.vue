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
            <slot name="body"/>
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
  title:       { type: String, default: '' },
  visible:     { type: Boolean, default: false },
  anchorRect:  { type: Object, default: null }  // { top, left, width, bottom }
})
const emit = defineEmits(['apply','cancel'])
const modalRef = ref(null)

// Позиция рядом с кнопкой
const positionStyle = computed(() => {
  if (!props.anchorRect) return {}
  return {
    position: 'absolute',
    top:  `${props.anchorRect.bottom + 4}px`,
    left: `${props.anchorRect.left}px`,
    minWidth: `${props.anchorRect.width}px`
  }
})

function apply()  { emit('apply') }
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
.fade-enter-active, .fade-leave-active { transition: opacity .2s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

.modal-overlay {
  position: fixed; inset: 0;
  background: rgba(0,0,0,0.2);
  z-index: 1000;
}

.modal-window {
  background: #fff;
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  display: flex;
  flex-direction: column;
  max-height: 300px;
  overflow: hidden;
  z-index: 1001;
}

.modal-header {
  padding: 8px 12px;
  border-bottom: 1px solid #eee;
}

.modal-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

.modal-body {
  padding: 12px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 8px; /* расстояние между элементами списка */
}

.modal-footer {
  padding: 8px 12px;
  border-top: 1px solid #eee;
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.btn {
  padding: 6px 14px;
  border-radius: 4px;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: background .2s;
}

.btn--apply { background: #007bff; color: #fff; }
.btn--apply:hover { background: #0056b3; }
.btn--cancel { background: #f5f5f5; }
.btn--cancel:hover { background: #e0e0e0; }
</style>
