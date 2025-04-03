<template>
  <div class="container">
    <div>
      <button @click="updateData" :disabled="loading">
        {{ loading ? 'Обновление...' : 'Обновить данные с ЦБ' }}
      </button>
      <div v-if="lastUpdate">
        Последнее обновление: {{ formatDate(lastUpdate) }}
      </div>
    </div>

    <table class="currency-table">
      <thead>
      <tr>
        <th>Дата</th>
        <th>EUR</th>
        <th>USD</th>
        <th>{{ getBYRBYNLabel() }}</th>
        <th>KZT</th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="rate in rates" :key="rate.date">
        <td>{{ formatDate(rate.date) }}</td>
        <td>{{ rate.EUR || '-' }}</td>
        <td>{{ rate.USD || '-' }}</td>
        <td>{{ rate.BYN || rate.BYR || '-' }}</td>
        <td>{{ rate.KZT || '-' }}</td>
      </tr>
      </tbody>
    </table>

    <div class="filter-section">
      <label>
        <input type="checkbox" v-model="showPeriodFilter"/>
        Показать за период
      </label>

      <div v-if="showPeriodFilter">
        <input type="date" v-model="startDate"/>
        <input type="date" v-model="endDate"/>
        <button @click="loadFilteredData">Показать за период</button>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      loading: false,
      lastUpdate: Date.now(),
      rates: [],
      showPeriodFilter: false,
      startDate: null,
      endDate: null
    };
  },
  mounted() {
    this.loadData();
  },
  methods: {
    async loadData(params = {}) {
      try {
        const response = await axios.get('/api/CurrencyRates', {params});
        this.rates = this.transformData(response.data);
      } catch (error) {
        console.error('Ошибка загрузки данных:', error);
      }
    },

    transformData(rawData) {
      return rawData.reduce((acc, item) => {
        const dateKey = item.date;
        if (!acc[dateKey]) {
          acc[dateKey] = {
            date: dateKey,
            EUR: null,
            USD: null,
            BYN: null, BYR: null,
            KZT: null
          };
        }
        acc[dateKey][item.currencyCode] = (item.value / item.nominal).toFixed(4);
        return acc;
      }, {});
    },
    
    getBYRBYNLabel() {
      if (Object.keys(this.rates).length === 0) return 'BYN';

      const firstDateKey = Object.keys(this.rates)[0];
      return this.rates[firstDateKey].BYN !== null ? 'BYN' : 'BYR';
    },

    async updateData() {
      this.loading = true;
      try {
        const response = await axios.post('/api/CurrencyRates/update');
        this.lastUpdate = response.data.lastUpdate;
        await this.loadData();
      } finally {
        this.loading = false;
      }
    },

    async loadFilteredData() {
      await this.loadData({
        startDate: this.startDate,
        endDate: this.endDate
      });
    },

    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString('ru-RU');
    }
  }
};
</script>

<style>
.currency-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

.currency-table th, .currency-table td {
  border: 1px solid white;
  padding: 8px;
  text-align: center;
}

.filter-section {
  margin-top: 20px;
}
</style>