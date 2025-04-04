<script>
import '@fontsource/roboto/700.css'

export default {
  name: 'Authentication',
  data () {
    return {
      login: '',
      password: '',
      errorsMessage:'',
      isLoading: false
    }
  },
  methods: {
    async handleSubmit (e) {
      e.preventDefault();

      if(!this.isFormValid) return;

      this.isLoading = true;
      this.errorsMessage = '';

      try {

        // Отправка запроса на сервер

        if (false) {
          throw new Error('Authentication failed');
        }

        this.$router.push('/');
      }
      catch(e) {
        this.errorsMessage = e.message||'Ошибочка';
      }
      finally {
        this.isLoading = false;
      }


    }
  },
computed: {
  isFormValid() {
    return this.login.trim().length >= 3 &&
        this.password.length >= 6;
  }
}
}
</script>

<template>
  <header class="header">
    <img src="@/assets/CollageLogo.png" alt="ККАСиЦТ" class="logo"/>
  </header>
  <div class="auth-container">


    <main class="auth-main">
      <form @submit.prevent="handleSubmit" class="auth-form">

        <h2 class="form-title">Вход в систему</h2>

        <div class="form-group">
          <label for="login" class="input-label">Login:</label>
          <input
          id="login"
          v-model.trim="login"
          type="text"
          maxlength="25"
          :disabled="isLoading"
          class="form-input">
        </div>

        <div class="form-group">
          <label for="password" class="input-label">Password:</label>
          <input
              id="password"
              v-model="password"
              type="password"
              minlength="6"
              maxlength="25"
              :disabled="isLoading"
              class="form-input">
        </div>

        <div v-if="errorsMessage" class="error-message">
          {{errorsMessage}}
        </div>

        <button
          type="submit"
          :disabled="isLoading||!isFormValid"
          class="submit-button"
        >
          <span v-if="!isLoading">Войти</span>
          <span v-else>Проверка...</span>
        </button>
      </form>
    </main>
  </div>
</template>

<style scoped>
.auth-container{
  max-width: 400px;
  margin: 5rem auto;
  padding: 2rem;
}

.header {
  text-align: left;
  margin: 2rem ;
}

.logo{
  width: 40vw;
  max-width: 150px;
}

.auth-form{
  background: #F5F5F5;
  padding: 2rem;
  border-radius: 16px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.5);
}

.form-title{
  text-align: center;
  margin-bottom: 1.5rem;
  color: #212121;
  font-size: 1.5rem;
  font-family: "Roboto",serif;
}

.form-group{
  margin-bottom: 1.5rem;
}

.input-label{
  display: block;
  margin-bottom: 0.5rem;
  color: #212121;
  font-size: 1rem;
  font-family: "Roboto", sans-serif;
  font-weight: 300;
}

.form-input{
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #212121;
  border-radius: 4px;
  box-sizing: border-box;
  font-size: 1rem;
  font-family: "Roboto", sans-serif;
  font-weight: 300;
  transition: border-color 0.3s ease;
}

.form-input:focus{
  outline:none;
  border-color: #7C4DFF;
  box-shadow: 0 0 0 5px rgba(124, 77, 255, 0.4);
}

.error-message{
  color: #212121;
  margin-bottom: 1rem;
  font-size: 1rem;
  font-family: "Roboto", sans-serif;
  font-weight: 300;
  text-align: center;
}

.submit-button{
  width: 100%;
  padding: 0.75rem;
  background-color: #5B00E1;
  color: #F5F5F5;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  font-family: "Roboto", sans-serif;
  font-weight: 300;
  cursor: pointer;
}

.submit-button:hover:not(:disabled){
  background-color: #7C4DFF;
}

.submit-button:disabled{
  opacity: 0.7;
  cursor: not-allowed;
}
</style>
