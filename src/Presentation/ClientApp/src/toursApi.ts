// src/Presentation/src/api/toursApi.ts
import axios from 'axios';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || '/api',
});

export const getTours = async (includeInactive = false) => {
  const response = await api.get<TourDto[]>('/tours', {
    params: { includeInactive }
  });
  return response.data;
};

export const createBooking = async (tourId: string, bookingData: BookingRequest) => {
  const response = await api.post<BookingDto>(`/tours/${tourId}/bookings`, bookingData);
  return response.data;
};

export type TourDto = {
  id: string;
  name: string;
  description: string;
  price: number;
  durationDays: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
};

export type BookingRequest = {
  customerName: string;
  customerEmail: string;
  numberOfPeople: number;
};