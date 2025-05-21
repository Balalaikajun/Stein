import { ref, watch } from 'vue'
import axios from 'axios'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

export function useContingentHistogram(requestBodyRef) {
  const data = ref([])
  const loading = ref(false)
  const error = ref(null)

  const fetchContingent = async () => {
    loading.value = true
    error.value = null

    try {
      const response = await axios.post(
        `${BACKEND_API_HOST}/api/Metrics/histogram/contingent`,
        requestBodyRef.value,
        { headers: { 'Content-Type': 'application/json' } }
      )

      data.value = response.data
    } catch (err) {
      console.error('Ошибка загрузки приказов:', err)
      error.value = err
    } finally {
      loading.value = false
    }
  }

  // Подгружаем при изменении параметров
  watch(requestBodyRef, fetchContingent, { immediate: true })

  return {
    data,
    loading,
    error,
    fetchContingent
  }
}
