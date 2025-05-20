import axios from 'axios'
import { ref } from 'vue'
import { BACKEND_API_HOST } from '@/configs/apiConfig.js'

export function useAllKpis() {
  // Объявляем ref (импортирован из 'vue')
  const kpis = ref({
    Students: null,
    Orders: null,
    Foreigners: null
  });
  const isLoading = ref(false);
  const error     = ref(null);

  // Параллельный запрос для всех трёх типов
  async function fetchAllKpis() {
    isLoading.value = true;
    error.value     = null;

    const types = ['Students', 'Orders', 'Foreigners'];
    try {
      const requests = types.map(type =>
        axios.post(
          `${BACKEND_API_HOST}/api/Metrics/kpi?type=${encodeURIComponent(type)}`,
          {}
        )
      );

      const responses = await Promise.all(requests);
      types.forEach((type, idx) => {
        kpis.value[type] = responses[idx].data;
      });
    } catch (err) {
      console.error('Ошибка при загрузке всех KPI:', err);
      error.value = err;
      kpis.value.Students = null;
      kpis.value.Orders   = null;
      kpis.value.Foreigners = null;
    } finally {
      isLoading.value = false;
    }
  }

  // Автоматически запускаем при инициализации
  fetchAllKpis();

  return { kpis, kpiIsLoading: isLoading, kpiError: error, fetchAllKpis }
}