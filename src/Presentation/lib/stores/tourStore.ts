// stores/tourStore.ts
import { makeAutoObservable } from 'mobx';

export interface Tour {
  id: string;
  name: string;
  price: number;
  duration: string;
}

class TourStore {
  tours: Tour[] = [];
  loading = false;

  constructor() {
    makeAutoObservable(this);
  }

  // Action
  setTours = (tours: Tour[]) => {
    this.tours = tours;
  };

  // Action
  setLoading = (loading: boolean) => {
    this.loading = loading;
  };

  // Computed property
  get totalTours() {
    return this.tours.length;
  }

  // Async action
  fetchTours = async () => {
    this.setLoading(true);
    try {
      // Simulate API call
      const response = await fetch('/api/tours');
      const tours = await response.json();
      this.setTours(tours);
    } catch (error) {
      console.error('Failed to fetch tours:', error);
    } finally {
      this.setLoading(false);
    }
  };
}

export const tourStore = new TourStore();