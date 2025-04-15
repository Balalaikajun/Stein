<template>
  <div class="filters-wrapper">
    <div class="filters-container">
      <template v-for="filter in filters" :key="filter.id">
        <BooleanFilter
            v-if="filter.dataType === 'boolean'"
            :filter="filter"
            v-model="selectedFilters[filter.id]"
            @update:modelValue="handleFilterChange"
        />

        <LookupFilter
            v-else-if="filter.dataType === 'lookup'"
            :filter="filter"
            v-model="selectedFilters[filter.id]"
            @update:modelValue="handleFilterChange"
        />
      </template>

      <slot name="additional-filters"></slot>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import BooleanFilter from '@/components/Selectors/Filters/BooleanFilter.vue'
import LookupFilter from '@/components/Selectors/Filters/LookupFilter.vue'

const props = defineProps({
  filters: {
    type: Array,
    default: () => []
  },
  values: {
    type: Object,
    default: () => ({})
  },
  showHeader: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['update-filters'])

const selectedFilters = ref({ ...props.values })

const handleFilterChange = () => {
  emit('update-filters', selectedFilters.value)
}

watch(() => props.values, (newVal) => {
  selectedFilters.value = { ...newVal }
}, { deep: true })
</script>

<style scoped>
.filters-wrapper {
  margin-bottom: 16px;
}

.filters-header {
  margin-bottom: 8px;
}

.toggle-button {
  background: none;
  border: none;
  color: #1976d2;
  cursor: pointer;
  padding: 4px 8px;
  font-size: 14px;
}

.filters-container {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}
</style>