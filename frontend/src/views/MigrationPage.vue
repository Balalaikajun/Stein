<script setup>
import { ref, computed } from 'vue'
import axios from '@/api/api.js'
import '@fontsource/roboto/700.css'
import menuItems from '@/router/menuData.js'
import Sidebar from '@/components/Sidebar/Sidebar.vue'
import { Info } from 'lucide-vue-next'
import exampleFilePath from '@/assets/ПримерВыгрузки.xlsx?url'

// Пусть в папке /src/assets лежит файл example.xlsx
const exampleFile = exampleFilePath

const errorsMessage = ref('')
const successMessage = ref('')
const isLoading = ref(false)
const file = ref(null)

const allowedTypes = [
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
]

// Валидность формы: выбран файл
const isFormValid = computed(() => file.value !== null)

// Управление модалкой
const isModalOpen = ref(false)

function openModal () {
  isModalOpen.value = true
}

function closeModal () {
  isModalOpen.value = false
}

// Обработка выбора файла (основного инпута)
function onFileChange (e) {
  const selected = e.target.files[0]
  if (!selected) {
    file.value = null
    return
  }

  // Проверяем MIME-тип (только .xlsx)
  if (!allowedTypes.includes(selected.type)) {
    errorsMessage.value = 'Неверный формат файла. Допускается только XLSX.'
    file.value = null
    return
  }

  // Проверяем размер (не больше 100 МБ)
  const maxSizeBytes = 100 * 1024 * 1024
  if (selected.size > maxSizeBytes) {
    errorsMessage.value = 'Размер файла слишком большой. Не более 100 МБ.'
    file.value = null
    return
  }

  // Сохраняем файл и сбрасываем сообщения
  file.value = selected
  errorsMessage.value = ''
  successMessage.value = ''
}

// Отправка формы на бэк
async function handleSubmit () {
  errorsMessage.value = ''
  successMessage.value = ''

  if (!file.value) {
    errorsMessage.value = 'Пожалуйста, выберите файл для загрузки.'
    return
  }

  isLoading.value = true
  try {
    const formData = new FormData()
    formData.append('file', file.value)

    const response = await axios.post(
        `/api/Migration`,
        formData,
        { headers: { 'Content-Type': 'multipart/form-data' } }
    )

    // Обычный формат: { success: true/false, message: '...' }
    if (response.status === 200 || response.data.success) {
      successMessage.value = response.data.message || 'Файл успешно загружен.'
    } else {
      errorsMessage.value = response.data.message || 'Неизвестная ошибка при загрузке.'
    }
  } catch (error) {
    console.log(error)
    if (error.response?.data?.message) {
      errorsMessage.value = error.response.data.message
    } else {
      errorsMessage.value = 'Ошибка при подключении к серверу. Повторите попытку позже.'
    }
  } finally {
    isLoading.value = false
  }
}

// Скачиваем пример с бекенда
async function downloadSample () {
  errorsMessage.value = ''
  successMessage.value = ''
  isLoading.value = true

  try {
    const response = await axios.get(`/api/Migration/sample`, {
      responseType: 'blob'
    })

    const blob = new Blob([response.data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'sample.xlsx')
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)

    successMessage.value = 'Пример файла успешно загружен.'
  } catch (err) {
    errorsMessage.value = 'Не удалось загрузить пример файла.'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="layout">
    <!-- Sidebar слева -->
    <Sidebar :items="menuItems"/>

    <!-- Основной контент справа -->
    <div class="page-content">
      <div class="auth-container">
        <!-- Форма загрузки основного файла -->
        <form @submit.prevent="handleSubmit" class="auth-form" style="margin-top: 1.5rem">
          <h2 class="form-title" style="margin-bottom: 1.5rem">
            Загрузка выгрузки
          </h2>
          <div class="form-group">
            <!-- Label отдельно -->
            <label class="input-label" for="fileInput">
              Выберите файл выгрузки (XLSX)
            </label>
            <!-- Иконка вне label -->
            <Info class="info-icon" @click="openModal"/>

            <input
                id="fileInput"
                type="file"
                class="form-input"
                accept=".xlsx,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                @change="onFileChange"
                :disabled="isLoading"
            />
          </div>

          <!-- Ошибки и успех -->
          <div v-if="errorsMessage" class="error-message">
            {{ errorsMessage }}
          </div>
          <div v-if="successMessage" class="success-message">
            {{ successMessage }}
          </div>

          <button
              type="submit"
              :disabled="isLoading || !isFormValid"
              class="submit-button"
              style="margin-top: 0.5rem"
          >
            <span v-if="!isLoading">Загрузить файл</span>
            <span v-else>Загрузка...</span>
          </button>
        </form>

        <!-- Модалка с локальным exampleFile -->
        <div v-if="isModalOpen" class="modal-overlay" @click.self="closeModal">
          <div class="modal-content">
            <h3>Пример файла для загрузки</h3>
            <p>Скачайте шаблон, чтобы правильно заполнить Excel-документ:</p>
            <a :href="exampleFile" download class="download-link">
              📄 Скачать шаблон
            </a>
            <button class="close-button" @click="closeModal">Закрыть</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Flex-контейнер для Sidebar + контента */
.layout {
  display: flex;
  min-height: 100vh;
}

/* Контент справа */
.page-content {
  flex: 1;
  background: var(--background-color, #f9f9f9);
  padding: 2rem;
  box-sizing: border-box;
}

/* Блок с формой и кнопкой */
.auth-container {
  max-width: 400px;
  margin: 0 auto;
  padding: 0;
}

/* Форма */
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
  position: relative;
  margin-bottom: 1.5rem;
}

.input-label {
  display: block;
  margin-bottom: 0.5rem;
}

/* Иконка информации */
.info-icon {
  position: absolute;
  top: 0;
  right: 0;
  width: 18px;
  height: 18px;
  cursor: pointer;
  color: var(--primary-color);
}

/* Поле выбора файла */
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

/* Ошибки и успех */
.error-message {
  margin-bottom: 1rem;
  font-size: 1rem;
  font-weight: 300;
  text-align: center;
  color: red;
}

.success-message {
  margin-bottom: 1rem;
  font-size: 1rem;
  font-weight: 300;
  text-align: center;
  color: green;
}

/* Кнопка отправки */
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

/* Стили модалки */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal-content {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  width: 90%;
  max-width: 400px;
  text-align: center;
}

.download-link {
  display: inline-block;
  margin: 1rem 0 2rem; /* вместо margin: 1rem 0; добавили отступ снизу */
  color: var(--primary-color);
  font-weight: bold;
  text-decoration: none;
}

.download-link:hover {
  text-decoration: underline;
}

.close-button {
  background: var(--secondary-color);
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  cursor: pointer;
  margin-left: 1rem;
}
</style>
