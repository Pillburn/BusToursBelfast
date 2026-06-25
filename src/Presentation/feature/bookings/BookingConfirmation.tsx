// features/bookings/BookingConfirmation.tsx
import { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  Paper,
  Button,
  Alert,
  CircularProgress,
  Divider,
  useTheme,
} from '@mui/material';
import { CheckCircle, Print } from '@mui/icons-material';
import {BookingConfirmation as BookingConfirmationType } from "../../lib/types/booking"

export const BookingConfirmation = () => {
  const [searchParams] = useSearchParams();
  const bookingId = searchParams.get('bookingId');
  const [booking, setBooking] = useState<BookingConfirmationType | null>(null); 
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const theme = useTheme();

  useEffect(() => {
    if (!bookingId) {
      setError('No booking found');
      setLoading(false);
      return;
    }

    const fetchBooking = async () => {
      try {
        setLoading(true);
        const response = await fetch(`/api/bookings/${bookingId}`);
        if (!response.ok) {
          throw new Error('Booking not found');
        }
        const data = await response.json();
        setBooking(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load booking');
      } finally {
        setLoading(false);
      }
    };

    fetchBooking();
  }, [bookingId]);

  if (loading) {
    return (
      <Container sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error || !booking) {
    return (
      <Container sx={{ py: 8 }}>
        <Alert severity="error">{error || 'Booking not found'}</Alert>
        <Button onClick={() => navigate('/')} sx={{ mt: 2 }}>
          Return Home
        </Button>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: { xs: 3, md: 6 } }}>
      <Paper
        elevation={0}
        sx={{
          p: { xs: 3, md: 5 },
          borderRadius: 3,
          textAlign: 'center',
          border: `1px solid ${theme.palette.divider}`,
        }}
      >
        <Box sx={{ mb: 4 }}>
          <Box
            sx={{
              width: 80,
              height: 80,
              borderRadius: '50%',
              bgcolor: 'success.main',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              mx: 'auto',
              mb: 2,
            }}
          >
            <CheckCircle sx={{ fontSize: 48, color: 'white' }} />
          </Box>
          <Typography variant="h4" sx={{ fontWeight: 700, color: 'success.main' }}>
            Booking Confirmed! 🎉
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mt: 1 }}>
            Your tour has been booked successfully
          </Typography>
        </Box>

        <Divider sx={{ my: 3 }} />

        <Box sx={{ textAlign: 'left', maxWidth: 400, mx: 'auto' }}>
          <Typography variant="subtitle2" color="text.secondary">
            Booking Reference
          </Typography>
          <Typography variant="h5" sx={{ fontWeight: 600, mb: 2 }}>
            {booking.bookingId || 'N/A'}
          </Typography>

          <Typography variant="subtitle2" color="text.secondary">
            Tour
          </Typography>
          <Typography variant="body1" sx={{ fontWeight: 500, mb: 2 }}>
            {booking.tourName}
          </Typography>

          <Typography variant="subtitle2" color="text.secondary">
            Date
          </Typography>
          <Typography variant="body1" sx={{ fontWeight: 500, mb: 2 }}>
            {booking.preferredDate}
          </Typography>

          <Typography variant="subtitle2" color="text.secondary">
            Participants
          </Typography>
          <Typography variant="body1" sx={{ fontWeight: 500, mb: 2 }}>
            {booking.totalParticipants} people
          </Typography>

          <Typography variant="subtitle2" color="text.secondary">
            Total Price
          </Typography>
          <Typography variant="h6" sx={{ fontWeight: 700, color: theme.palette.primary.main }}>
            £{booking.totalPrice}
          </Typography>
        </Box>

        <Divider sx={{ my: 3 }} />

        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, gap: 2, justifyContent: 'center' }}>
          <Button
            variant="contained"
            onClick={() => window.print()}
            startIcon={<Print />}
            sx={{ backgroundColor: theme.palette.primary.main }}
          >
            Print Confirmation
          </Button>
          <Button
            variant="outlined"
            onClick={() => navigate('/my-booking')}
          >
            View My Booking
          </Button>
          <Button
            variant="text"
            onClick={() => navigate('/tours')}
          >
            Browse More Tours
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};