// types/tour.ts
export interface Tour {
  id: string;
  name: string;
  price: number;
  description: string;
  imageUrl?: string;
  location?: string;
  duration?: string;
  maxCapacity?: number;
  availableDates?: string[];
  includes?: string[];
  rating?: number;
}

export interface TourFilters {
  minPrice?: number;
  maxPrice?: number;
  duration?: string;
  destination?: string;
}