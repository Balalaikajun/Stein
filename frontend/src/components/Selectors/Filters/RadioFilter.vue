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
        <div class="filter-modal-content">
          <ul class="options-list">
            <li
                v-for="opt in filteredOptions"
                :key="opt.value"
                class="options-list__item"
            >
              <label class="radio-option">
                <input
                    type="radio"
                    :value="opt.value"
                    :checked="localValue === opt.value"
                    :name="filter.id"
                    @click="handleSelect(opt.value)"
                />
                {{ opt.label }}
              </label>
            </li>
            <li v-if="!filteredOptions.length" class="empty-state">
              Ничего не найдено
            </li>
          </ul>
        </div>
      </template>
    </FilterModal>
  </div>
</template>

<script setup>
import { computed, nextTick, ref, watch } from 'vue'
import FilterButton from '@/components/Selectors/FilterButton.vue'
import FilterModal from '@/components/Selectors/FilterModal.vue'

const props = defineProps({
  filter: {
    type: Object,
    required: true,
  },
  modelValue: { type: [String, Number, Boolean], default: null }
})

const emit = defineEmits(['update:modelValue'])

// Reactive state
const isOpen = ref(false)
const search = ref('')
const localValue = ref(props.modelValue)
const btnRef = ref(null)
const anchorRect = ref(null)

// Computed
const hasValue = computed(() => localValue.value !== null)
const filteredOptions = computed(() => {
  const searchTerm = search.value.toLowerCase()
  return props.filter.staticOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchTerm)
  )
})

// Handlers
function toggle() {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    nextTick(() => {
      anchorRect.value = btnRef.value.$el.getBoundingClientRect()
      search.value = ''
    })
  }
}

function handleSelect(value) {
  localValue.value = localValue.value === value ? null : value
}

function apply() {
  emit('update:modelValue', localValue.value)
  close()
}

function close() {
  isOpen.value = false
  search.value = ''
  localValue.value = props.modelValue
}

// Sync value
watch(
    () => props.modelValue,
    (v) => { localValue.value = v }
)
</script>

<style scoped>
.filter-modal-content {
  max-height: 400px;
  overflow-y: auto;
  padding: 8px;
}

.filter-input {
  width: 100%;
  padding: 8px 12px;
  margin-bottom: 12px;
  border: 1px solid var(--secondary-background-color);
  border-radius: var(--border-radius);
  transition: border-color 0.2s;
}

.filter-input:focus {
  outline: none;
  border-color: var(--primary-color);
}

.options-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.options-list__item {
  padding: 4px 0;
}

.radio-option {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px;
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: background-color 0.2s;
}

.radio-option:hover {
  background-color: var(--hover-color);
}

input[type="radio"] {
  width: 16px;
  height: 16px;
  accent-color: var(--primary-color);
}

.empty-state {
  padding: 12px;
  text-align: center;
  color: var(--secondary-text-color);
}
</style>