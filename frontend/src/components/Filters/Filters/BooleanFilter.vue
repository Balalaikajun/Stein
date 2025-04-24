<template>
  <div class="filter-item">
    <FilterButton
        ref="btnRef"
        :title="filter.title"
        @toggle="toggle"
        :isOpen="isOpen"
        :hasValue="modelValue !== null"
    />
    <FilterModal
        v-if="isOpen"
        :visible="isOpen"
        :anchorRect="anchorRect"
        @apply="apply"
        @cancel="close"
    >
      <template #body>
        <div class="radio-group">
          <label class="radio-option">
            <input type="radio" value="" v-model="local" />
            <span class="radio-label">Все</span>
          </label>
          <label class="radio-option">
            <input type="radio" value="true" v-model="local" />
            <span class="radio-label">Да</span>
          </label>
          <label class="radio-option">
            <input type="radio" value="false" v-model="local" />
            <span class="radio-label">Нет</span>
          </label>
        </div>
      </template>
    </FilterModal>
  </div>
</template>

<script setup>
import { ref, nextTick } from 'vue'
import FilterButton from '../FilterButton.vue'
import FilterModal from '../FilterModal.vue'

const props = defineProps({
  filter: Object,
  modelValue: [Boolean, null]
})

const emit = defineEmits(['update:modelValue'])

const isOpen = ref(false)
const local = ref('')
const anchorRect = ref(null)
const btnRef = ref(null)

// Логика обработки значений
function toggle() {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    nextTick(() => {
      const rect = btnRef.value?.$el?.getBoundingClientRect?.()
      if (rect) anchorRect.value = rect
    })
  }
}

function apply() {
  const value = local.value === 'true' ? true : local.value === 'false' ? false : null
  emit('update:modelValue', value)
  close()
}

function close() {
  isOpen.value = false
}
</script>