<script setup>
import GenericTableView from '@/components/GenericTableView.vue'
import departmentConfig from '@/configs/department.js'
</script>

<template>
  <GenericTableView
      :config="departmentConfig"
      page-title="Отделения"
  />
</template>