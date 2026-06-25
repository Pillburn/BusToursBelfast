// lib/hooks/useBookNow.ts
import { useState, useCallback } from 'react';
import type {BookingFormData } from '../types/booking';
import { loadStripe  } from '@stripe/stripe-js';

//Initialise Stripe outside component
// Use import.meta.env for Vite
const getStripeKey = (): string => {
  const  key = import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY;
  if(!key)
    {
      console.warn('Stripe publishable key is missing. Using test key.')
      return 'pk_test_1234567890';
    }

    return key;
};

const stripePromise = loadStripe(getStripeKey()).catch(error => {
  console.error('Failed to load stripe:', error);
  return null;
})

interface UseBookNowProps {
  tourId: string;
  tourName: string;
  tourPrice: number;
  onSuccess?: (data: unknown) => void;
  onError?: (error: Error) => void;
}

export const useBookNow = ({ 
  tourId, 
  tourName, 
  tourPrice, 
  onSuccess, 
  onError 
}: UseBookNowProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [bookingStatus, setBookingStatus] = useState<'idle' | 'loading' | 'success' | 'error'>('idle');
  const [error, setError] = useState<Error | null>(null);

  const openModal = useCallback(() => {
    console.log('openModal called - setting isOpen to true')
    setIsOpen(true);
    setBookingStatus('idle');
    setError(null);
  }, []);

  const closeModal = useCallback(() => {
    console.log('🔵 closeModal called - setting isOpen to false');
    setIsOpen(false);
    setBookingStatus('idle');
    setError(null);
    setIsLoading(false);
  }, []);

  const handleBookingSubmit = useCallback(async (formData: BookingFormData) => {
    setIsLoading(true);
    setBookingStatus('loading');

    try {
      // Prepare the data for API - don't spread formData directly
      const apiRequest = {
        tourId,
        tourName,
        tourPrice,
        customerName: formData.customerName,
        email: formData.email,
        phoneNumber: formData.phoneNumber,
        numberOfParticipants: {
          adults: formData.numberOfParticipants.adults,
          children: formData.numberOfParticipants.children,
          infants: formData.numberOfParticipants.infants
        },
        preferredDate: formData.preferredDate,
        pickupLocation: formData.pickupLocation,
        specialRequests: formData.specialRequests,
        passportNumber: formData.passportNumber,
        dateOfBirth: formData.dateOfBirth,
        emergencyContact: formData.emergencyContact,
        travelInsuranceDetails: formData.travelInsuranceDetails,
        bookingDate: new Date().toISOString(),
        status: 'pending'
      };
      console.log('📤 Sending booking request:', apiRequest);
      // Your API endpoint - update to match your backend
      const response = await fetch('/api/tour/bookings', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(apiRequest),
      });

      console.log('📥 Response status:', response.status);
      const result = await response.json();
      console.log('📦 Full response from backend:', result);
      console.log('🔑 ClientSecret:', result.clientSecret);
      console.log('📋 BookingId:', result.bookingId);

      if (!response.ok) 
        {
        // If response is not JSON, get text
          const errorMessage = result?.message || result?.title || result?.error || 'Booking failed';
          console.error('❌ Backend error text:', errorMessage);
          throw new Error(errorMessage);
        }
      
      if (!result.clientSecret) 
      {
        console.error('❌ No clientSecret in response!');
        console.error('Response:', JSON.stringify(result, null, 2));
        throw new Error('Payment initialization failed - no client secret');
      }
       // 2. Confirm payment with Stripe
      const stripe = await stripePromise;
      if (!stripe)
      {
        throw new Error('Stripe failed to initialize. Please check your publishable key.');
      }

      // 3. Confirm the payment
      const { error: stripeError } = await stripe.confirmPayment({
        clientSecret: result.clientSecret,
        confirmParams: {
          return_url: `${window.location.origin}/booking-confirmation?bookingId=${result.bookingId}`,
        },
      });

      if (stripeError) {
        throw new Error(stripeError.message);
      }

      // 4. Success (payment confirmed)
      setBookingStatus('success');
      onSuccess?.(result);
      
      setTimeout(() => {
        closeModal();
      }, 3000);
      setBookingStatus('success');
      onSuccess?.(result);
      
      // Auto close after 2 seconds on success
      setTimeout(() => {
        closeModal();
      }, 2000);
      
    } catch (err) {
      const error = err instanceof Error ? err : new Error('Booking failed');
      setError(error);
      setBookingStatus('error');
      onError?.(error);
    } finally {
      setIsLoading(false);
    }
  }, [tourId, tourName, tourPrice, onSuccess, onError, closeModal]);

  return {
    isOpen,
    isLoading,
    bookingStatus,
    error,
    openModal,
    closeModal,
    handleBookingSubmit
  };
};