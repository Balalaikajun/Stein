<script setup>
import GenericTableView from '@/components/GenericTableView.vue'
import students from '@/configs/students.js'
</script>

<template>
  <GenericTableView
      :config="students"
      page-title="Студенты"
  />
</template>