import { Grid, Container, Typography } from '@mui/material';
import type { Tour } from '../../../src/types/tour';
import { TourCard } from './TourCard';

const sampleTours: Tour[] = [
  {
    id: '1',
    title: 'Belfast City Explorer',
    description: 'Discover the rich history and culture of Belfast...',
    price: 25,
    imageUrl: '/images/belfast-tour.jpg',
    duration: '3 hours',
    rating: 4.5,
    includes: ['Guide', 'Entry fees', 'Transport']
  },
  {
    id: '2',
    title: 'Causeway Tour',
    description: 'Experience the giants causeway one of Irelands most famous landmarks steeped in story and history',
    price: 20,
    imageUrl: '/images/belfast-tour.jpg',
    duration: 'All Day',
    rating: 5,
    includes: ['Guide', 'Entry fees', 'Transport']
  }
];

interface ToursProps {
  tours?: Tour[];
  onBookTour?: (tourId: string) => void;
}

export const Tours = ({ tours = sampleTours, onBookTour }: ToursProps) => {
  const handleBookNow = (tourId: string) => {
    console.log('Booking Tour:', tourId);
    onBookTour?.(tourId);
  };

  return (
    <Container 
      maxWidth="lg" 
      disableGutters // removes default horizontal padding
      sx={{ 
        px: { xs: 2, sm: 3, md: 4 }, // responsive padding
        py: { xs: 2, sm: 3, md: 4 }  // vertical padding
      }}
    >
      <Typography 
        variant="h4" 
        component="h1"
        sx={{ 
          mb: { xs: 2, sm: 3, md: 4 }, // responsive bottom margin
          fontSize: { xs: '1.75rem', sm: '2rem', md: '2.25rem' }, // responsive font size
          fontWeight: 'bold',
          textAlign: { xs: 'center', sm: 'left' } // center on mobile
        }}
      >
        Available Tours
      </Typography>

      <Grid 
        container 
        spacing={{ xs: 2, sm: 2.5, md: 3 }} // smaller gap on mobile
      >
        {tours.map((tour) => (
          <Grid 
            size={{ xs: 12, sm: 6, md: 4 }} 
            key={tour.id}
          >
            <TourCard tour={tour} onBookNow={handleBookNow} />
          </Grid>
        ))}
      </Grid>
    </Container>
  );
};

export default Tours;