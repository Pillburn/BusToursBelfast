// types/booking.ts
export interface ParticipantCount {
  adults: number;
  children: number;
  infants: number;
}

export interface BookingFormData {
  customerName: string;
  email: string;
  phoneNumber: string;
  numberOfParticipants: ParticipantCount;
  preferredDate: string;
  pickupLocation: string;
  specialRequests: string;
  passportNumber: string;
  dateOfBirth: string | null;
  emergencyContact: string;
  travelInsuranceDetails: string;
}

export interface BookingApiRequest extends BookingFormData {
  tourId: string;
  tourName: string;
  tourPrice: number;
  bookingDate: string;
  status: string;
}

export interface BookingResponse {
  bookingId: string;
  referenceNumber: string;
  status: string;
  message: string;
  totalPrice: number;
  totalParticipants: number;
  tourName: string;
  preferredDate: string;
  email: string;
  bookingDate: string;
}

// types/booking.ts
export interface BookingConfirmation {
  id: string;
  bookingId: string;
  referenceNumber: string;
  tourName: string;
  preferredDate: string;
  totalParticipants: number;
  totalPrice: number;
  status: string;
  customerName: string;
  email: string;
  phoneNumber: string;
  createdAt: string;
}