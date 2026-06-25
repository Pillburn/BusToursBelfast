import { Grid, Container, Typography, Button, Box } from '@mui/material';
import type { Tour } from '../../../src/types/tour';
import { TourCard } from './TourCard';
import { useBookNow } from '../../../lib/hooks/useBookNow'
import { BookTourForm} from '../../tour/form/BookTourForm'
import { useState } from 'react';
import theme from '../../../src/theme/theme';

const sampleTours: Tour[] = [
  {
    id: '95c3eff0-4cb3-49aa-9f14-b432f203eeaa',
    title: 'Belfast City Explorer',
    description: 'Discover the rich history and culture of Belfast...',
    price: 25,
    imageUrl: '/images/belfast-tour.jpg',
    duration: '3 hours',
    rating: 4.5,
    includes: ['Guide', 'Entry fees', 'Transport']
  },
  {
    id: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
    title: 'Causeway Tour',
    description: 'Experience the giants causeway steeped in story and history',
    price: 20,
    imageUrl: '/images/giants-causeway.jpg',
    duration: 'All Day',
    rating: 5,
    includes: ['Guide', 'Entry fees', 'Transport']
  }
];

export const Tours = ({ tours = sampleTours }: {tours?: Tour[] }) => {
  const [selectedTour, setSelectedTour] = useState<Tour | null>(null);

  const {
    isOpen,
    isLoading:isBookingLoading,
    bookingStatus,
    error,
    openModal,
    closeModal,
    handleBookingSubmit
  } = useBookNow({
    tourId:selectedTour?.id ||'',
    tourName: selectedTour?.title || '',
    tourPrice: selectedTour?.price || 0,
    onSuccess: (data: unknown) => {
      console.log('Booking Successful:', data);
    },
    onError: (error: unknown) => {
      console.error('Booking failed:', error)
    }
  });

  const handleBookNow = (tourId: string) => {
    console.log('Tours: Booking Tour:', tourId);
    const tour = tours.find(t => t.id === tourId);
    console.log('🟡 Tours: Found tour:', tour);
    if (tour){
      console.log('🟣 Tours: Setting selectedTour and opening modal');
      setSelectedTour(tour);
      openModal()
      console.log('📌 Tours: openModal called, isOpen should be true');
    }else{
      console.log('Tours: Tour not found')
    }
  };

  return (
    <>
    <Container 
        maxWidth="xl"  // ← Max width for large screens
        sx={{ 
          px: { xs: 2, sm: 3, md: 4 },  // ← Responsive padding
          py: { xs: 3, sm: 4, md: 6 },   // ← Vertical padding
          mx: 'auto',  // ← Auto margin for centering
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',  // ← Center content
        }}
      >
        {/* Header with underline */}
        <Box sx={{ 
          width: '100%', 
          maxWidth: '1200px',  // ← Limit header width
          mb: { xs: 3, sm: 4, md: 5 },
        }}>
          <Typography 
            variant="h3" 
            component="h1"
            sx={{ 
              fontWeight: 700,
              color: theme.palette.primary.main,
              textAlign: { xs: 'center', sm: 'left' },  // ← Center on mobile
              fontSize: { xs: '1.75rem', sm: '2.5rem', md: '3rem' },
            }}
          >
            Our Tours
          </Typography>
          <Box sx={{ 
            width: { xs: '80px', sm: '100px' },
            height: '4px',
            background: `linear-gradient(90deg, ${theme.palette.secondary.main}, ${theme.palette.primary.main})`,
            marginTop: '8px',
            marginLeft: { xs: 'auto', sm: 0 },  // ← Center on mobile
            marginRight: { xs: 'auto', sm: 0 },
            borderRadius: '2px',
          }} />
        </Box>

        {/* Tour Grid - Centered */}
        <Grid 
          container 
          spacing={{ xs: 2, sm: 3, md: 4 }}
          justifyContent="center"  // ← Center the grid items
          sx={{
            maxWidth: '1200px',  // ← Limit grid width
            width: '100%',
          }}
        >
          {tours.map((tour) => (
            <Grid 
              size={{ xs: 12, sm: 6, md: 4 }}  // ← Responsive sizing
              key={tour.id}
              sx={{
                display: 'flex',
                justifyContent: 'center',  // ← Center each card
              }}
            >
              <Box sx={{ width: '100%', maxWidth: '380px' }}> 
                <TourCard tour={tour} onBookNow={handleBookNow} />
              </Box>
            </Grid>
          ))}
        </Grid>

        {/* Empty state if no tours */}
        {tours.length === 0 && (
          <Box sx={{ 
            textAlign: 'center', 
            py: 8,
            width: '100%',
          }}>
            <Typography variant="h5" color="text.secondary">
              No tours available at the moment
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
              Please check back later
            </Typography>
          </Box>
        )}
      </Container>
        // Tours.tsx - Add this temporary button
    <Button 
  variant="contained" 
  color="secondary"
  onClick={async () => {
    console.log('🧪 Test: Sending test booking...');
    
    try {
      // First, try to get a tour
      console.log('🔍 Getting first tour...');
      const tourResponse = await fetch('/api/tour/first');
      console.log('📊 Tour response status:', tourResponse.status);
      
      if (!tourResponse.ok) {
        const errorText = await tourResponse.text();
        console.error('❌ Tour response error:', tourResponse.status, errorText);
        return;
      }
      
      const tour = await tourResponse.json();
      console.log('✅ Found tour:', tour);
      
      // Then try to book it
      console.log('📝 Creating booking...');
      const bookingResponse = await fetch('/api/tour/bookings', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          tourId: tour.id,
          tourName: tour.name,
          tourPrice: tour.price,
          fullName: 'Test User',
          email: 'test@email.com',
          phoneNumber: '1234567890',
          numberOfParticipants: { adults: 1, children: 0, infants: 0 },
          preferredDate: '2026-07-15',
          pickupLocation: 'Test Hotel',
          specialRequests: 'None',
          passportNumber: '',
          dateOfBirth: null,
          emergencyContact: '',
          travelInsuranceDetails: '',
        }),
      });
      
      console.log('📊 Booking response status:', bookingResponse.status);
      
      if (!bookingResponse.ok) {
        const errorText = await bookingResponse.text();
        console.error('❌ Booking error:', bookingResponse.status, errorText);
        return;
      }
      
      const result = await bookingResponse.json();
      console.log('✅ Booking successful:', result);
      
    } catch (error) {
      console.error('❌ Test error:', error);
    }
  }}
>
  🧪 Test API
</Button>
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
  );
};

export default Tours;