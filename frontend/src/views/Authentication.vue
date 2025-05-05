<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import '@fontsource/roboto/700.css'

const login = ref('')
const password = ref('')
const errorsMessage = ref('')
const isLoading = ref(false)

const router = useRouter()

const isFormValid = computed(() => {
  return login.value.trim().length >= 3 && password.value.length >= 3
})

async function handleSubmit(e) {
  e.preventDefault()

  if (!isFormValid.value) return

  isLoading.value = true
  errorsMessage.value = ''

  try {
    const res = await axios.post('https://localhost:7203/api/Auth', {
      login: login.value,
      password: password.value,
    })

    // API возвращает токен в формате plain text, а не объект.
    localStorage.setItem('token', res.data)
    axios.defaults.headers.common['Authorization'] = `Bearer ${res.data}`

    router.push('/departments')
  } catch (err) {
    if (err.response && err.response.status === 400)
      errorsMessage.value = err.response.data
    else
      errorsMessage.value = 'Ошибка аутентификации'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <header class="header">
    <img src="@/assets/CollageLogo.png" alt="ККАСиЦТ" class="logo" />
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
              class="form-input"
          />
        </div>

        <div class="form-group">
          <label for="password" class="input-label">Password:</label>
          <input
              id="password"
              v-model="password"
              type="password"
              minlength="3"
              maxlength="25"
              :disabled="isLoading"
              class="form-input"
          />
        </div>

        <div v-if="errorsMessage" class="error-message">
          {{ errorsMessage }}
        </div>

        <button
            type="submit"
            :disabled="isLoading || !isFormValid"
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
.auth-container {
  max-width: 400px;
  margin: 5rem auto;
  padding: 2rem;
}

.header {
  text-align: left;
  margin: 2rem;
}

.logo {
  width: 40vw;
  max-width: 150px;
}

.auth-form {
  background: var(--secondary-background-color);
  padding: 2rem;
  border-radius: 16px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.5);
}

.form-title {
  text-align: center;
  font-family: Roboto, serif;
}

.form-group {
  margin-bottom: 1.5rem;
}

.input-label {
  display: block;
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid var(--text-color);
  border-radius: 4px;
  box-sizing: border-box;
  transition: border-color 0.3s ease;
}

.form-input:focus {
  outline: none;
  border-color: var(--secondary-color);
  box-shadow: 0 0 0 5px rgba(124, 77, 255, 0.4);
}

.error-message {
  margin-bottom: 1rem;
  font-size: 1rem;
  font-weight: 300;
  text-align: center;
}

.submit-button {
  width: 100%;
  padding: 0.75rem;
  background-color: var(--primary-color);
  color: var(--background-color);
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  font-weight: 300;
  cursor: pointer;
}

.submit-button:hover:not(:disabled) {
  background-color: var(--secondary-color);
}

.submit-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}
</style>
