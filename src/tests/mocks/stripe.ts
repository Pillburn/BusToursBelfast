// src/test/mocks/stripe.ts
import { vi } from 'vitest'

export const mockStripe = {
  confirmPayment: vi.fn().mockResolvedValue({ error: null }),
  confirmSetup: vi.fn().mockResolvedValue({ error: null }),
  retrievePaymentIntent: vi.fn().mockResolvedValue({ paymentIntent: { status: 'succeeded' } }),
}

export const mockStripePromise = vi.fn().mockResolvedValue(mockStripe)

// Mock the loadStripe function
vi.mock('@stripe/stripe-js', () => ({
  loadStripe: vi.fn().mockImplementation(() => mockStripePromise),
}))