// features/tours/TourDetails.tsx
import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Grid,
  Typography,
  Box,
  Button,
  Chip,
  Rating,
  Divider,
  Paper,
  useTheme,
  CircularProgress,
  Alert,
} from '@mui/material';
import { LocalActivity, AccessTime, LocationOn, People } from '@mui/icons-material';
import { Tour } from '../../../lib/types/tour';

export const TourDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const theme = useTheme();
  const [tour, setTour] = useState<Tour | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTour = async () => {
      try {
        setLoading(true);
        // ✅ Fetch tour from your backend
        const response = await fetch(`/api/tour/${id}`);
        if (!response.ok) {
          throw new Error('Tour not found');
        }
        const data = await response.json();
        
        // Map backend data to frontend Tour type
        setTour({
          id: data.id,
          name: data.title || data.name,
          description: data.description,
          price: data.price,
          imageUrl: data.imageUrl || '/images/default-tour.jpg',
          duration: data.durationDays ? `${data.durationDays} days` : 'N/A',
          location: data.location,
          rating: data.rating || 4.5,
          maxCapacity: data.maxCapacity,
          includes: data.includes || [],
        });
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load tour');
      } finally {
        setLoading(false);
      }
    };

    if (id) {
      fetchTour();
    }
  }, [id]);

  const handleBookNow = () => {
    if (tour) {
      // ✅ Navigate to booking with tour data
      navigate('/tours', { state: { selectedTourId: tour.id } });
    }
  };

  if (loading) {
    return (
      <Container sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error || !tour) {
    return (
      <Container sx={{ py: 8 }}>
        <Alert severity="error">{error || 'Tour not found'}</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: { xs: 3, md: 6 } }}>
      <Paper elevation={0} sx={{ overflow: 'hidden', borderRadius: 3 }}>
        {/* Hero Image */}
        <Box sx={{ position: 'relative', height: { xs: 250, md: 400 } }}>
          <Box
            component="img"
            src={tour.imageUrl}
            alt={tour.name}
            sx={{
              width: '100%',
              height: '100%',
              objectFit: 'cover',
            }}
          />
          <Box
            sx={{
              position: 'absolute',
              bottom: 0,
              left: 0,
              right: 0,
              height: '50%',
              background: 'linear-gradient(transparent, rgba(0,0,0,0.7))',
            }}
          />
          <Box
            sx={{
              position: 'absolute',
              bottom: 24,
              left: 24,
              right: 24,
              color: 'white',
            }}
          >
            <Typography variant="h3" sx={{ fontWeight: 700 }}>
              {tour.name}
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mt: 1 }}>
              <Chip
                icon={<LocalActivity />}
                label={tour.duration}
                sx={{ bgcolor: 'rgba(255,255,255,0.2)', color: 'white' }}
              />
              <Chip
                icon={<LocationOn />}
                label={tour.location || 'Various Locations'}
                sx={{ bgcolor: 'rgba(255,255,255,0.2)', color: 'white' }}
              />
              <Rating value={tour.rating} readOnly precision={0.5} />
            </Box>
          </Box>
        </Box>

        {/* Content */}
        <Box sx={{ p: { xs: 2, md: 4 } }}>
          <Grid container spacing={4}>
            {/* Main Content */}
            <Grid size={{ xs: 12, md: 8 }}>
              <Typography variant="h5" sx={{ fontWeight: 600, mb: 2 }}>
                About This Tour
              </Typography>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 3, lineHeight: 1.8 }}>
                {tour.description}
              </Typography>

              <Divider sx={{ my: 3 }} />

              <Typography variant="h6" sx={{ fontWeight: 600, mb: 2 }}>
                What's Included
              </Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mb: 3 }}>
                {tour.includes?.map((item, index) => (
                  <Chip
                    key={index}
                    label={item}
                    sx={{
                      bgcolor: theme.palette.primary.light + '20',
                      color: theme.palette.primary.main,
                    }}
                  />
                ))}
                {tour.maxCapacity && (
                  <Chip
                    icon={<People />}
                    label={`Max ${tour.maxCapacity} people`}
                    sx={{
                      bgcolor: 'rgba(0,0,0,0.05)',
                      color: theme.palette.text.secondary,
                    }}
                  />
                )}
              </Box>
            </Grid>

            {/* Sidebar */}
            <Grid size={{ xs: 12, md: 4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 3,
                  bgcolor: theme.palette.background.default,
                  borderRadius: 2,
                  position: 'sticky',
                  top: 24,
                }}
              >
                <Typography variant="h4" sx={{ fontWeight: 700, color: theme.palette.primary.main }}>
                  £{tour.price}
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                  per person
                </Typography>

                <Button
                  variant="contained"
                  fullWidth
                  size="large"
                  onClick={handleBookNow}
                  sx={{
                    py: 1.5,
                    mb: 2,
                    fontWeight: 600,
                    backgroundColor: theme.palette.primary.main,
                    '&:hover': {
                      backgroundColor: theme.palette.primary.dark,
                    },
                  }}
                >
                  Book Now
                </Button>

                <Divider sx={{ my: 2 }} />

                <Typography variant="body2" color="text.secondary">
                  <AccessTime sx={{ fontSize: 16, verticalAlign: 'middle', mr: 1 }} />
                  Free cancellation within 24 hours
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                  <LocalActivity sx={{ fontSize: 16, verticalAlign: 'middle', mr: 1 }} />
                  Instant confirmation
                </Typography>
              </Paper>
            </Grid>
          </Grid>
        </Box>
      </Paper>
    </Container>
  );
};