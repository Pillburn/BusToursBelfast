// src/lib/hooks/__tests__/useBookNow.withStripe.test.ts
import { renderHook, act } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { useBookNow } from '../useBookNow'
import { mockStripe } from '../../../../tests/mocks/stripe'

describe('useBookNow with Stripe', () => {
  it('handles payment confirmation', async () => {
    const mockResponse = {
      success: true,
      bookingId: '123',
      clientSecret: 'secret_123'
    }

    const mockFetch = vi.fn().mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(mockResponse)
    })

    global.fetch = mockFetch as unknown as typeof fetch

    const { result } = renderHook(() => useBookNow({
      tourId: '1',
      tourName: 'Test Tour',
      tourPrice: 100,
    }))

    

    const formData = {
      customerName: 'John Doe',
      email: 'john@example.com',
      phoneNumber: '1234567890',
      numberOfParticipants: { adults: 1, children: 0, infants: 0 },
      preferredDate: '2026-07-15',
      pickupLocation: 'Test Hotel',
      specialRequests: '',
      passportNumber: '',
      dateOfBirth: null,
      emergencyContact: '',
      travelInsuranceDetails: ''
    }

    await act(async () => {
      await result.current.handleBookingSubmit(formData)
    })

    expect(mockStripe.confirmPayment).toHaveBeenCalledWith(
      expect.objectContaining({
        clientSecret: 'secret_123'
      })
    )
  })
})