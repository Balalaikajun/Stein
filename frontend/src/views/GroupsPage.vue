<script setup>
import GenericTableView from '@/components/GenericTableView.vue'
import groups from '@/configs/groups.js'
</script>

<template>
  <GenericTableView
      :config="groups"
      page-title="Группы"
  />
</template>