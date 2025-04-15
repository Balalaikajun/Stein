<template>
  <div class="filter-item">
    <FilterButton :title="filter.title" @toggle="isOpen = !isOpen" :isOpen="isOpen"/>
    <FilterModal v-if="isOpen" @apply="apply" @cancel="close">
      <template #body>
        <input
            v-if="filter.searchable"
            v-model="search"
            @input="load()"
            placeholder="Поиск..."
            class="filter-input"
        />
        <ul class="options-list">
          <li
              v-for="opt in options"
              :key="JSON.stringify(opt.value)"
              @click="select(opt.value)"
              :class="{ selected: isEqual(opt.value) }"
          >{{ opt.label }}</li>
        </ul>
      </template>
    </FilterModal>
  </div>
</template>
<script setup>
import { ref, watch, onMounted } from 'vue'
import axios from 'axios'
import isEqualFn from 'lodash.isequal'
import FilterButton from '@/components/Selectors/FilterButton.vue'
import FilterModal from '@/components/Selectors/FilterModal.vue'

const props = defineProps({
  filter:     Object,            // { api, mapOption, searchable? }
  modelValue: [Object, Number]
})
const emit = defineEmits(['update:modelValue'])
const isOpen  = ref(false)
const search  = ref('')
const options = ref([])
const local   = ref(props.modelValue)

function load() {
  axios.get(filter.api, { params: { q: search.value } }).then(r=>{
    options.value = r.data.map(filter.mapOption)
  })
}

function select(v) {
  local.value = isEqualFn(local.value, v) ? null : v
}

function isEqual(v) {
  return isEqualFn(local.value, v)
}

function apply() {
  emit('update:modelValue', local.value)
  isOpen.value = false
}

function close() {
  local.value = props.modelValue
  search.value = ''
  isOpen.value = false
}

onMounted(load)
watch(search, load)
</script>
