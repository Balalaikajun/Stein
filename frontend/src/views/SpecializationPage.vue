<script setup>
import GenericTableView from '@/components/GenericTableView.vue'
import specializationConfig from '@/configs/specialization.js'
</script>

<template>
  <GenericTableView
      :config="specializationConfig"
      page-title="Специальности"
  />
</template>