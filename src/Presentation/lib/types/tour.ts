// types/tour.ts
export interface Tour {
  id: string;
  name: string;
  price: number;
  description: string;
  image?: string;
  duration?: string;
  maxCapacity?: number;
  availableDates?: string[];
}

export interface TourFilters {
  minPrice?: number;
  maxPrice?: number;
  duration?: string;
  destination?: string;
}