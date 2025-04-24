<script setup>
import { ref } from 'vue'
import FormModal from './FormModal.vue'
import OpenFormButton from './OpenFormButton.vue'

const isModalVisible = ref(false)
const editingData = ref(null)

const userConfig = {
  title: 'пользователя',
  fields: [
    {
      name: 'name',
      label: 'Имя',
      type: 'date',
      required: true,
      validate: (value) => value.length >= 3,
      errorMessage: 'Минимум 3 символа'
    },
    {
      name: 'email',
      label: 'Email',
      type: 'select',
      required: true,
      errorMessage: 'Неверный формат email',
      filter: {
        id: 'SpecializationIds',
        title: 'Специализации',
        dataType: 'lookup',
        apiEndpoint: '/api/Specialization',
        dependsOn: ['DepartmentIds'],
        dependentParams: {
          DepartmentIds: 'departmentIds'
        },
        params: {
          take: 15,
        },
        paramKeys: {
          skip: 'skip',
          take: 'take',
          search: 'searchText',
          sortKey: 'sortBy',
          sortOrder: 'descending'
        },
        mapOption: opt => ({ label: opt.title, value: opt.id })
      }
    }
  ],
  submitHandler: async (data) => {
    // API запрос для сохранения данных
    console.log('Saving data:', data)
  }
}

const openModal = (editData = null) => {
  editingData.value = editData
  isModalVisible.value = true
}
</script>

<template>
  <OpenFormButton
      label="Создать пользователя"
      @click="openModal()"
  />

  <FormModal
      v-model:is-visible="isModalVisible"
      :config="userConfig"
      :is-editing="!!editingData"
      :initial-data="editingData"
  />
</template>