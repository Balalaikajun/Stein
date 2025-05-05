<template>
  <Teleport to="body">
    <div v-if="isVisible" class="modal-overlay" @click.self="close">
      <div class="modal-container">
        <div class="modal-header">
          <h2>{{ isEditing ? `Редактировать ${config.title}` : `Создать ${config.title}` }}</h2>
          <button class="close-button" @click="close">&times;</button>
        </div>
        <div class="modal-content">
          <form @submit.prevent="handleSubmit">
            <div
                v-for="field in config.fields"
                :key="field.name"
                class="form-group"
            >
              <label :for="field.name">{{ field.label }}</label>

              <input
                  v-if="['text','number','email','password','date'].includes(field.type)"
                  :type="field.type"
                  :id="field.name"
                  v-model="formData[field.name]"
                  :placeholder="field.placeholder"
                  class="form-input"
              />

              <Selector
                  v-else-if="field.type === 'select'"
                  v-model="formData[field.name]"
                  :filter="field.filter"
              />

              <div v-if="errors[field.name]" class="error-message">
                {{ errors[field.name] }}
              </div>
            </div>

            <div class="modal-actions">
              <button type="button" class="btn cancel" @click="close">
                Отмена
              </button>
              <button type="submit" class="btn confirm">
                {{ isEditing ? 'Сохранить' : 'Создать' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { reactive, watch } from 'vue'
import Selector from '@/components/Form/Fields/Selector.vue'

const props = defineProps({
  config: {
    type: Object,
    required: true
  },
  isVisible: {
    type: Boolean,
    required: true
  },
  isEditing: {
    type: Boolean,
    default: false
  },
  initialData: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:isVisible', 'submit'])

const formData = reactive({})
const errors = reactive({})

watch(
    () => props.initialData,
    (data) => {
      props.config.fields.forEach(f => {
        formData[f.name] = data[f.name] ?? ''
        errors[f.name] = ''
      })
    },
    { immediate: true }
)

function validate() {
  let isValid = true
  props.config.fields.forEach(f => {
    errors[f.name] = ''
    const val = formData[f.name]

    // Обновленная проверка
    if (f.required && (val === undefined || val === null)) {
      errors[f.name] = f.errorMessage || 'Обязательное поле'
      isValid = false
    }

    if (f.validate && !f.validate(val)) {
      errors[f.name] = f.errorMessage || 'Неверное значение'
      isValid = false
    }
  })
  return isValid
}

async function handleSubmit (data) {
  if (!validate()) return

  const filteredData = filterNonEmptyFields(formData)

  const payload = props.isEditing
      ? { ...filteredData, id: props.initialData.id }
      : { ...filteredData }

  emit('submit', payload)
  close()
}

function filterNonEmptyFields(obj) {
  const filtered = {}
  for (const key in obj) {
    const value = obj[key]
    if (value !== null && value !== undefined && value !== '') {
      filtered[key] = value
    }
  }
  return filtered
}

function close () {
  emit('update:isVisible', false)
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-container {
  background: var(--background-color);
  border-radius: var(--border-radius);
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  padding: 1.5rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--secondary-background-color);
}

.close-button {
  font-size: 1.75rem;
  background: none;
  border: none;
  cursor: pointer;
  color: var(--secondary-text-color);
  padding: 0 0.5rem;
  transition: color var(--transition-duration) var(--transition-timing);
}

.close-button:hover {
  color: var(--text-color);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid var(--secondary-background-color);
  border-radius: var(--border-radius);
  transition: border-color var(--transition-duration) var(--transition-timing);
  font-size: 1rem;
}

.form-input:focus {
  border-color: var(--primary-color);
  outline: none;
  box-shadow: 0 0 0 2px rgba(91, 0, 225, 0.2);
}

.error-message {
  color: var(--error-color);
  font-size: 0.875rem;
  margin-top: 0.5rem;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--secondary-background-color);
}

.btn {
  padding: 0.75rem 1.5rem;
  border-radius: var(--border-radius);
  font-weight: 500;
  font-size: 1rem;
  transition: all var(--transition-duration) var(--transition-timing);
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  min-width: 100px;
}

.btn:focus {
  outline: 2px solid var(--primary-color);
  outline-offset: 2px;
}

.btn.confirm {
  background: var(--primary-color);
  color: white;
  border: none;
  box-shadow: 0 2px 4px rgba(91, 0, 225, 0.2);
}

.btn.confirm:hover {
  background: #4A00B9;
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(91, 0, 225, 0.25);
}

.btn.confirm:active {
  transform: translateY(0);
  box-shadow: 0 1px 2px rgba(91, 0, 225, 0.2);
}

.btn.cancel {
  background: var(--secondary-background-color);
  color: var(--secondary-text-color);
  border: 1px solid var(--secondary-text-color);
}

.btn.cancel:hover {
  background: var(--secondary-background-color);
  color: var(--text-color);
  border-color: var(--text-color);
}

.btn.cancel:active {
  background: #e0e0e0;
}

.loading-spinner {
  animation: spin 0.8s linear infinite;
  border: 2px solid currentColor;
  border-top-color: transparent;
  border-radius: 50%;
  width: 1.2rem;
  height: 1.2rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>