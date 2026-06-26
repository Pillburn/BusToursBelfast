// src/features/tours/dashboard/__tests__/TourCard.test.tsx
import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { TourCard } from '../TourCard'

const mockTour = {
  id: '1',
  title: 'Belfast City Explorer',
  description: 'Discover the rich history of Belfast',
  price: 25,
  imageUrl: '/images/tour.jpg',
  duration: '3 hours',
  rating: 4.5,
  includes: ['Guide', 'Entry fees']
}

describe('TourCard', () => {
  it('renders tour information correctly', () => {
    render(<TourCard tour={mockTour} onBookNow={() => {}} />)
    
    expect(screen.getByText('Belfast City Explorer')).toBeInTheDocument()
    expect(screen.getByText('£25')).toBeInTheDocument()
    expect(screen.getByText('3 hours')).toBeInTheDocument()
    expect(screen.getByText('⭐ 4.5')).toBeInTheDocument()
  })

  it('calls onBookNow when button is clicked', () => {
    const handleBookNow = vi.fn()
    render(<TourCard tour={mockTour} onBookNow={handleBookNow} />)
    
    const button = screen.getByText('Book Now')
    fireEvent.click(button)
    
    expect(handleBookNow).toHaveBeenCalledWith('1')
  })

  it('displays includes chips', () => {
    render(<TourCard tour={mockTour} onBookNow={() => {}} />)
    
    expect(screen.getByText('Guide')).toBeInTheDocument()
    expect(screen.getByText('Entry fees')).toBeInTheDocument()
  })
})