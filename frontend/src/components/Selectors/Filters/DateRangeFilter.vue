<template>
  <div class="filter-item">
    <FilterButton
        ref="btnRef"
        :title="filter.title"
        @toggle="toggle"
        :isOpen="isOpen"
        :hasValue="hasValue"
    />

    <FilterModal
        v-if="isOpen"
        :visible="isOpen"
        :anchorRect="anchorRect"
        @apply="apply"
        @cancel="close"
    >
      <template #body>
        <div class="date-range-picker">
          <div class="date-inputs">
            <div class="input-group">
              <label>От</label>
              <input
                  type="date"
                  v-model="localFrom"
                  :max="localTo"
                  class="date-input"
              />
            </div>
            <div class="input-group">
              <label>До</label>
              <input
                  type="date"
                  v-model="localTo"
                  :min="localFrom"
                  class="date-input"
              />
            </div>
          </div>
          <div v-if="hasValue" class="reset-btn" @click="resetDates">
            Сбросить
          </div>
        </div>
      </template>
    </FilterModal>
  </div>
</template>

<script setup>
import { computed, ref, watch } from 'vue'
import FilterButton from '@/components/Selectors/FilterButton.vue'
import FilterModal from '@/components/Selectors/FilterModal.vue'

const props = defineProps({
  filter: {
    type: Object,
    required: true,
    validator: (f) => f.paramKeys && f.paramKeys.from && f.paramKeys.to
  },
  modelValue: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue'])

// Реактивные переменные
const isOpen = ref(false)
const localFrom = ref(null)
const localTo = ref(null)
const btnRef = ref(null)
const anchorRect = ref(null)

// Вычисляемые свойства
const hasValue = computed(() => !!localFrom.value || !!localTo.value)

// Методы
const toggle = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    anchorRect.value = btnRef.value?.$el.getBoundingClientRect()
  }
}

const apply = () => {
  const updates = {}
  if (localFrom.value) updates[props.filter.paramKeys.from] = localFrom.value
  if (localTo.value) updates[props.filter.paramKeys.to] = localTo.value

  // Отправляем обновления напрямую в корень модели
  emit('update:modelValue', {
    ...props.modelValue,
    ...updates
  })
  close()
}

const close = () => {
  isOpen.value = false
}

const resetDates = () => {
  localFrom.value = null
  localTo.value = null
}

// Наблюдатели
watch(() => props.modelValue, (newVal) => {
  localFrom.value = newVal[props.filter.paramKeys.from] || null
  localTo.value = newVal[props.filter.paramKeys.to] || null
}, { deep: true })

watch(localFrom, (newFrom) => {
  if (newFrom && localTo.value && newFrom > localTo.value) {
    localTo.value = newFrom
  }
})

watch(localTo, (newTo) => {
  if (newTo && localFrom.value && newTo < localFrom.value) {
    localFrom.value = newTo
  }
})
</script>

<style scoped>
.date-range-picker {
  padding: 16px;
  min-width: 300px;
}

.date-inputs {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
}

.input-group {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.date-input {
  padding: 8px 12px;
  border: 1px solid var(--secondary-background-color);
  border-radius: var(--border-radius);
  width: 100%;
  font-family: inherit;
  font-size: 0.875rem;
}

.date-input:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(91, 0, 225, 0.1);
}

.reset-btn {
  color: var(--primary-color);
  text-align: center;
  cursor: pointer;
  font-size: 0.875rem;
  padding: 8px;
  border-radius: var(--border-radius);
  transition: background-color 0.2s;
  user-select: none;
}

.reset-btn:hover {
  background-color: var(--hover-color);
}

.reset-btn:active {
  opacity: 0.8;
}
</style>
