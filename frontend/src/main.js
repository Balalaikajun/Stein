import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import '@fontsource/roboto/400.css'
import '@fontsource/roboto/700.css'
import './assets/styles/main.css'
import axios from '@/api/api.js'

const app = createApp(App)
const token = localStorage.getItem('token')
if (token) {
  axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
}
axios.defaults.url = import.meta.env.VITE_BACKEND_API_HOST
app.use(router)
app.mount('#app')
