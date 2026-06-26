// src/features/bookings/__tests__/BookTourForm.test.tsx
import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { BookTourForm } from '../../../tour/form/BookTourForm'

describe('BookTourForm', () => {
  const mockProps = {
    open: true,
    tourName: 'Test Tour',
    tourPrice: 100,
    isLoading: false,
    onSubmit: vi.fn(),
    onClose: vi.fn(),
    bookingStatus: 'idle' as const,
    error: null
  }

  it('validates required fields', async () => {
    render(<BookTourForm {...mockProps} />)
    
    // Click confirm without filling fields
    const confirmButton = screen.getByText('Confirm Booking')
    fireEvent.click(confirmButton)
    
    // Should show validation errors
    expect(await screen.findByText('Full name is required')).toBeInTheDocument()
  })

  it('submits form when valid', async () => {
    render(<BookTourForm {...mockProps} />)
    
    // Fill in the form
    fireEvent.change(screen.getByLabelText(/Full Name/i), {
      target: { value: 'John Doe' }
    })
    fireEvent.change(screen.getByLabelText(/Email/i), {
      target: { value: 'john@example.com' }
    })
    fireEvent.change(screen.getByLabelText(/Phone/i), {
      target: { value: '1234567890' }
    })
    
    const confirmButton = screen.getByText('Confirm Booking')
    fireEvent.click(confirmButton)
    
    expect(mockProps.onSubmit).toHaveBeenCalled()
  })
})