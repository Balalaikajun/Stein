<script setup>
import GenericTableView from '@/components/GenericTableView.vue'
import teachers from '@/configs/teachers.js'
</script>

<template>
  <GenericTableView
      :config="teachers"
      page-title="Преподаватели"
  />
</template>