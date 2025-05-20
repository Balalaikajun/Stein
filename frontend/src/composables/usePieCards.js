import { ref, isRef, unref, computed, watch } from 'vue';
import axios from 'axios';
import { BACKEND_API_HOST } from '@/configs/apiConfig.js';

export function usePieCards(pieTypes, initialRequestBody = {}) {
  const typesRef = isRef(pieTypes) ? pieTypes : ref(pieTypes);
  const requestBodyRef = isRef(initialRequestBody)
    ? initialRequestBody
    : ref(initialRequestBody);

  const pies = ref({});
  const isLoading = ref(false);
  const error = ref(null);

  // Реактивное объединение тел запросов
  const mergedBody = computed(() => unref(requestBodyRef));

  async function fetchAllPies(additionalBody = {}) {
    isLoading.value = true;
    error.value = null;

    try {
      const results = {};
      const finalBody = {
        ...unref(mergedBody),
        ...additionalBody
      };

      await Promise.all(
        unref(typesRef).map(async (type) => {
          const url = `${BACKEND_API_HOST}/api/Metrics/pie?type=${encodeURIComponent(type)}`;
          const response = await axios.post(url, finalBody);
          results[type.toLowerCase()] = response.data;
        })
      );

      pies.value = results;
    } catch (err) {
      error.value = err;
      pies.value = {};
    } finally {
      isLoading.value = false;
    }
  }

  // Автоматический запрос при изменении mergedBody
  watch(
    mergedBody,
    () => fetchAllPies(),
    { deep: true, immediate: true }
  );

  return {
    pies,
    pieIsLoading:isLoading,
    pieError:error,
    fetchAllPies
  };
}