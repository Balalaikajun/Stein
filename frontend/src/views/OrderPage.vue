<script setup>
import GenericTableView from '@/components/GenericTableView.vue'
import orders from '@/configs/orders.js'
</script>

<template>
  <GenericTableView
      :config="orders"
      page-title="Приказы"
  />
</template>