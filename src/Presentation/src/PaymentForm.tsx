// client/src/components/PaymentForm.tsx
import { Elements, CardElement, useStripe, useElements } from '@stripe/react-stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import React from 'react';

// Initialize Stripe with your publishable key
const stripePromise = loadStripe('your_publishable_key');

// Define TypeScript interfaces
interface PaymentFormProps {
  amount: number;
  onSuccess: (paymentIntent: unknown) => void; // You might want to create a more specific type for paymentIntent
}

interface PaymentIntentResponse {
  clientSecret: string;
}

const PaymentForm: React.FC<PaymentFormProps> = ({ amount, onSuccess }) => {
  const stripe = useStripe();
  const elements = useElements();

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    
    if (!stripe || !elements) {
      console.error('Stripe or Elements not loaded');
      return;
    }

    try {
      // 1. Get client secret from your backend
      const response = await fetch('/api/payments/intent', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ amount })
      });
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const { clientSecret }: PaymentIntentResponse = await response.json();

      // 2. Confirm payment with Stripe
      const cardElement = elements.getElement(CardElement);
      
      if (!cardElement) {
        throw new Error('Card element not found');
      }

      const result = await stripe.confirmCardPayment(clientSecret, {
        payment_method: {
          card: cardElement,
        }
      });

      if (result.error) {
        console.error(result.error.message);
      } else if (result.paymentIntent && result.paymentIntent.status === 'succeeded') {
        onSuccess(result.paymentIntent);
      }
    } catch (error) {
      console.error('Payment error:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <CardElement />
      <button type="submit" disabled={!stripe}>
        Pay ${amount}
      </button>
    </form>
  );
};

// Define props for the wrapper component
interface StripePaymentProps {
  amount: number;
  onSuccess?: (paymentIntent: unknown) => void; // Optional if you want to handle success differently
}

// Wrap your payment form with Elements provider
export const StripePayment: React.FC<StripePaymentProps> = ({ amount, onSuccess }) => (
  <Elements stripe={stripePromise}>
    <PaymentForm amount={amount} onSuccess={onSuccess || (() => {})} />
  </Elements>
);

export default StripePayment;