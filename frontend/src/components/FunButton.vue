<script>
import { computed } from 'vue'

export default {
  name: 'FunButton',
  data () {
    return {
      count: 0
    }
  }
}
</script>

<template>
  <div>
    <button @click="count++" class="fun-button">{{ count }}</button>
  </div>
</template>

<style scoped>
.fun-button {
  font-family: inherit;
  font-size: inherit;
  color: inherit;
}
</style>