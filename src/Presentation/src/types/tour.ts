// types/tour.ts
export interface Tour {
  id: string;
  title: string;
  description: string;
  price: number;
  imageUrl?: string;
  duration?: string;
  rating?: number;
  includes?: string[];
  location?: string;
  maxCapacity?: string;
}

export interface TourCardProps {
  tour: Tour;
  onBookNow?: (tourId: string) => void;
} 

export interface TourDetails {
  CustomerName: string;
  email: string;
  phoneNumber: string;
  numberOfParticipants: {
    adults: number;
    children: number;
    infants: number;
  };
  preferredDate: string;
  pickupLocation: string;
  specialRequests: string;
  passportNumber?: string;
  dateOfBirth?: string;
  emergencyContact?: string;
  travelInsuranceDetails?: string;
}