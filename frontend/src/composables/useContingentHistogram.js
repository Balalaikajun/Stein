import { ref, isRef, unref, computed, watch } from 'vue'
import axios from 'axios'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

/**
 * Хук для загрузки гистограммы "Контингент"
 *
 * @param {Object|Ref<Object>} initialRequestBody - тело запроса
 */
export function useContingentHistogram(initialRequestBody = {}) {
  const requestBodyRef = isRef(initialRequestBody) ? initialRequestBody : ref(initialRequestBody)

  const data = ref([])
  const isLoading = ref(false)
  const error = ref(null)

  const mergedBody = computed(() => unref(requestBodyRef))

  const fetchContingent = async (additionalBody = {}) => {
    isLoading.value = true
    error.value = null

    try {
      const finalBody = { ...mergedBody.value, ...additionalBody }
      const url = `${BACKEND_API_HOST}/api/Metrics/histogram/contingent`

      const response = await axios.post(url, finalBody, {
        headers: { 'Content-Type': 'application/json' }
      })

      data.value = response.data
    } catch (err) {
      console.error('Ошибка загрузки гистограммы Контингент:', err)
      error.value = err
      data.value = []
    } finally {
      isLoading.value = false
    }
  }

  watch(mergedBody, () => {
    fetchContingent()
  }, { deep: true, immediate: true })

  return {
    data,
    isLoading,
    error,
    fetchContingent
  }
}
