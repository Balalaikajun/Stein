<script>
import FunButton from '@/components/FunButton.vue'

export default {
  name: 'TestPage',
  components: {
    FunButton
  }
}
</script>

<template>
  <div class="mainBox">
    <div class="box1">
      <h1 class="testPage">Привет мир!</h1>
    </div>
    <div class="box2">
      <FunButton class="testPage"/>
    </div>
  </div>
</template>

<style scoped>
.testPage {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  color: #0086ff;
  text-align: center;
  font-size: 18px;
}

.box1 {
  background: #9fcced;
}

.box2 {
  background: #9fcced;
}

.mainBox {
  width: 30vw;
  margin: 5px auto;
  display: flex;
  flex-direction: row;
  gap: 10vw;
}

</style>