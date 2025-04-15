<template>
  <div class="filter-item">
    <FilterButton
        ref="btnRef"
        :title="filter.title"
        @toggle="toggle"
        :isOpen="isOpen"
    />
    <FilterModal
        v-if="isOpen"
        :visible="isOpen"
        :anchorRect="anchorRect"
        @apply="apply"
        @cancel="close"
    >
      <template #body>
        <label><input type="radio" value="" v-model="local" /> Все</label>
        <label><input type="radio" value="true"  v-model="local" /> Да</label>
        <label><input type="radio" value="false" v-model="local" /> Нет</label>
      </template>
    </FilterModal>
  </div>
</template>

<script setup>
import { ref, nextTick } from 'vue'
import FilterButton from '../FilterButton.vue'
import FilterModal  from '../FilterModal.vue'

const props = defineProps({
  filter: Object,
  modelValue: {
    type: [Boolean, null],
    default: null
  }
})
const emit = defineEmits(['update:modelValue'])

const isOpen = ref(false)
const local = ref(props.modelValue === true ? 'true' : props.modelValue === false ? 'false' : '')
const anchorRect = ref(null)
const btnRef = ref(null)

function toggle(event) {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    nextTick(() => {
      const rect = btnRef.value?.$el?.getBoundingClientRect?.()
      if (rect) anchorRect.value = rect
    })
  }
}

function apply() {
  emit('update:modelValue',
      local.value === 'true' ? true :
          local.value === 'false' ? false : null
  )
  close()
}
function close() {
  local.value = props.modelValue != null ? String(props.modelValue) : ''
  isOpen.value = false
}
</script>
