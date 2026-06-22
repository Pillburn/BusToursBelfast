import Box from "@mui/material/Box";
import { TourCard } from "./TourCard";
import { useTours } from "../../../lib/hooks/useTour";
import { Typography } from "@mui/material";
import { useBookNow } from "../../../lib/hooks/useBookNow";
import { useState } from "react";
import { Tour } from "../../../src/types/tour";
import BookTourForm from "../../tour/form/BookTourForm";



export default function TourList() {
  const {tours, isLoading} = useTours();
  const [selectedTour, setSelectedTour] = useState<Tour | null>(null);
  
  const {
    isOpen,
    isLoading: isBookingLoading,
    bookingStatus,
    error,
    openModal,
    closeModal,
    handleBookingSubmit
  } = useBookNow({
    tourId: selectedTour?.id || '',
    tourName: selectedTour?.title || '',
    tourPrice: selectedTour?.price || 0,
    onSuccess: (data: unknown) => {
      console.log('Booking successful:', data);
    },
    onError: (error: unknown) => {
      console.error('Booking failed:', error);
    }
  });
  
  if (isLoading) return <Typography>Loading...</Typography>
  if(!tours) return <Typography>No Tour Found</Typography>
  
  const handleBookNow = (tourId: string) => {
    console.log('🟢 handleBookNow called with:', tourId);
    console.log('📦 Available tours:', tours);
    const tour = tours?.find(t => t.id === tourId);
    console.log('🎯 Found tour:', tour);
    if (tour) {
      setSelectedTour(tour);
      console.log('🚀 Opening modal...');
      openModal();
      console.log('📌 After openModal, isOpen should be true');
    }
  };

  return (
    <>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
        {tours.map(tour => (
          <TourCard 
            key={tour.id}
            tour={tour}
            onBookNow={handleBookNow} 
          />
        ))}
      </Box>

        {console.log('Rendering BookTourForm - isOpen: ' ,  isOpen, 'selectedTour:',selectedTour?.title)}
  
      <BookTourForm
        open={isOpen}
        tourName={selectedTour?.title || ''}
        tourPrice={selectedTour?.price || 0}
        isLoading={isBookingLoading}
        onSubmit={handleBookingSubmit}
        onClose={closeModal}
        bookingStatus={bookingStatus}
        error={error}
      />
    </>
    
  )
}