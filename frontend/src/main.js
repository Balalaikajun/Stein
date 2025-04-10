import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/700.css';
import './assets/styles/main.css'
import axios from 'axios'

const app = createApp(App)

app.use(router)
app.mount('#app')
