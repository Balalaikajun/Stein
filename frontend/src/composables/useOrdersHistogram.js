import { computed, isRef, ref, unref, watch } from 'vue'
import axios from '@/api/api.js'

/**
 * Хук для загрузки гистограммы приказов
 *
 * @param {Object|Ref<Object>} initialRequestBody - тело запроса
 */
export function useOrdersHistogram (initialRequestBody = {}) {
  const requestBodyRef = isRef(initialRequestBody) ? initialRequestBody : ref(initialRequestBody)

  const data = ref([])
  const isLoading = ref(false)
  const error = ref(null)

  const mergedBody = computed(() => unref(requestBodyRef))

  const fetchOrders = async (additionalBody = {}) => {
    isLoading.value = true
    error.value = null

    try {
      const finalBody = { ...mergedBody.value, ...additionalBody }
      const url = `/api/Metrics/histogram/order`

      const response = await axios.post(url, finalBody, {
        headers: { 'Content-Type': 'application/json' }
      })

      data.value = response.data
    } catch (err) {
      console.error('Ошибка загрузки гистограммы приказов:', err)
      error.value = err
      data.value = []
    } finally {
      isLoading.value = false
    }
  }

  watch(mergedBody, () => {
    fetchOrders()
  }, { deep: true, immediate: true })

  return {
    data,
    isLoading,
    error,
    fetchOrders
  }
}
