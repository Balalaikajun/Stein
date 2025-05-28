import { ref, isRef, unref, computed, watch } from 'vue';
import axios from 'axios';
import { BACKEND_API_HOST } from '@/configs/apiConfig.js';

export function usePieCards(pieTypes, piesValuesMapping, initialRequestBody = {}) {
  const typesRef = isRef(pieTypes) ? pieTypes : ref(pieTypes);
  const requestBodyRef = isRef(initialRequestBody) ? initialRequestBody : ref(initialRequestBody);

  const pies = ref({});
  const isLoading = ref(false);
  const error = ref(null);

  const mergedBody = computed(() => unref(requestBodyRef));

  async function fetchAllPies(additionalBody = {}) {
    isLoading.value = true;
    error.value = null;

    try {
      const results = {};
      const finalBody = { ...unref(mergedBody), ...additionalBody };

      await Promise.all(
        unref(typesRef).map(async (type) => {
          const url = `${BACKEND_API_HOST}/api/Metrics/pie?type=${encodeURIComponent(type)}`;
          const response = await axios.post(url, finalBody);

          // Ожидаем формат:
          // {
          //   "segments": [
          //     { "label": "Male",   "value": 114 },
          //     { "label": "Female", "value": 111 }
          //   ]
          // }
          const raw = response.data;
          const keyLower = type.toLowerCase();
          const mapping = piesValuesMapping[type] || {};

          // Если пришёл массив raw.segments с полями label/value:
          if (Array.isArray(raw.segments)) {
            const segments = raw.segments.map(item => ({
              label: mapping[item.label] || item.label,
              value: Number(item.value)
            }));

            const total = segments.reduce((sum, seg) => sum + seg.value, 0);
            results[keyLower] = { segments, total };
          } else {
            // В остальных случаях просто возвращаем пустой пай
            results[keyLower] = { segments: [], total: 0 };
          }
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

  watch(
    mergedBody,
    () => fetchAllPies(),
    { deep: true, immediate: true }
  );

  return {
    pies,
    pieIsLoading: isLoading,
    pieError: error,
    fetchAllPies
  };
}
