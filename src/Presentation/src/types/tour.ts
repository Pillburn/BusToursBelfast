// types/tour.ts
export interface Tour {
  id: string;
  title: string;
  description: string;
  price: number;
  imageUrl: string;
  duration: string;
  rating: number;
  includes?: string[];
}

export interface TourCardProps {
  tour: Tour;
  onBookNow?: (tourId: string) => void;
} 