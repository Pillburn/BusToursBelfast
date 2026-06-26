// src/lib/hooks/__tests__/useBookNow.test.ts
import { renderHook, act } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useBookNow } from '../useBookNow'

// Mock fetch
global.fetch = vi.fn()

describe('useBookNow', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('initializes with default values', () => {
    const { result } = renderHook(() => useBookNow({
      tourId: '1',
      tourName: 'Test Tour',
      tourPrice: 100,
    }))

    expect(result.current.isOpen).toBe(false)
    expect(result.current.isLoading).toBe(false)
    expect(result.current.bookingStatus).toBe('idle')
  })

  it('opens and closes modal', () => {
    const { result } = renderHook(() => useBookNow({
      tourId: '1',
      tourName: 'Test Tour',
      tourPrice: 100,
    }))

    act(() => {
      result.current.openModal()
    })
    expect(result.current.isOpen).toBe(true)

    act(() => {
      result.current.closeModal()
    })
    expect(result.current.isOpen).toBe(false)
  })

  it('handles booking submission', async () => {
    const mockResponse = {
      success: true,
      bookingId: '123',
      clientSecret: 'secret_123'
    }

    const mockFetch = vi.fn().mockResolvedValueOnce({
      ok: true,
      json: async () => mockResponse
    })

    global.fetch = mockFetch as unknown as typeof fetch; 

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

    expect(global.fetch).toHaveBeenCalledWith('/api/tour/bookings', expect.any(Object))
    expect(result.current.bookingStatus).toBe('success')
  })
})