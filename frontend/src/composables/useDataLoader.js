import { ref } from 'vue'
import axios from '@/api/api.js'

export default function useDataLoader (apiConfig) {
  const items = ref([])
  const loading = ref(false)
  const hasMore = ref(false)
  const lastSeenId = ref(null)
  const lastSeenValue = ref(null)

  const loadData = async (params) => {
    try {
      loading.value = true
      const { data } = await axios.post(
        `${apiConfig.endpoint}`,
        params
      )

      items.value = [...items.value, ...data.items]
      hasMore.value = data.hasMore
      lastSeenId.value = data.lastSeenId
      lastSeenValue.value = data.lastSeenValue
    } catch (error) {
      console.error('Ошибка загрузки данных:', error)
    } finally {
      loading.value = false
    }
  }

  return {
    items,
    loading,
    hasMore,
    lastSeenId,
    lastSeenValue,
    loadData
  }
}