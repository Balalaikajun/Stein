<template>
  <div class="filters-wrapper">
    <div class="filters-container">
      <template v-if="filters.length">
        <template v-for="filter in filters" :key="filter.id">
          <BooleanFilter
              v-if="filter.dataType === 'boolean'"
              :filter="filter"
              v-model="selectedFilters[filter.id]"
              @update:modelValue="value => updateFilterValue(filter.id, value)"
          />
          <LookupFilter
              v-else-if="filter.dataType === 'lookup'"
              :filter="filter"
              :active-filters="selectedFilters"
              v-model="selectedFilters[filter.id]"
              @update:modelValue="value => updateFilterValue(filter.id, value)"

          />
          <DateRangeFilter
              v-else-if="filter.dataType === 'dateRange'"
              :filter="filter"
              :active-filters="selectedFilters"
              v-model="selectedFilters[filter.id]"
              @update:modelValue="value => updateFilterValue(filter.id, value)"

          />
          <RadioFilter
              v-else-if="filter.dataType === 'radio'"
              :filter="filter"
              v-model="selectedFilters[filter.id]"
              @update:modelValue="value => updateFilterValue(filter.id, value)"

          />
        </template>
      </template>
      <div v-else class="no-filters">
        Нет доступных фильтров
      </div>
      <slot name="additional-filters"></slot>
    </div>
  </div>
</template>

<script setup>
import { ref, toRaw, watch } from 'vue'
import BooleanFilter from './Filters/BooleanFilter.vue'
import LookupFilter from './Filters/LookupFilter.vue'
import DateRangeFilter from '@/components/Filters/Filters/DateRangeFilter.vue'
import { debounce } from 'lodash-es'
import RadioFilter from '@/components/Filters/Filters/RadioFilter.vue'

const props = defineProps({
  filters: {
    type: Array,
    default: () => []
  },
  values: {
    type: Object,
    default: () => ({})
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

function updateFilterValue(filterId, value) {
  selectedFilters.value[filterId] = value;

  // Сбрасываем все фильтры, зависящие от filterId
  props.filters
      .filter(f => f.dependsOn?.includes(filterId))
      .forEach(dep => {
        selectedFilters.value[dep.id] = [];
      });
  // Эмитим единожды
  emit('update-filters', { ...selectedFilters.value });
}

const debouncedEmit = debounce(filters => {
  emit('update-filters', filters);
}, 300);
</script>

<style scoped>


.filters-wrapper {
}

.filters-container {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  padding: 1rem;
  background: var(--secondary-background-color);
  border-radius: var(--border-radius);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  min-height: 60px;
}

.no-filters {
  padding: 0.5rem;
  color: var(--secondary-text-color);
  font-style: italic;
}
</style>