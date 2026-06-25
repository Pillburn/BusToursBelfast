// features/bookings/MyBooking.tsx
import { useState } from 'react';
import {
  Container,
  Box,
  Typography,
  TextField,
  Button,
  Paper,
  Alert,
  Divider,
  useTheme,
} from '@mui/material';
import { Search } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

export const MyBooking = () => {
  const [referenceNumber, setReferenceNumber] = useState('');
  const [email, setEmail] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const theme = useTheme();

  const handleLookup = async () => {
    if (!referenceNumber || !email) {
      setError('Please enter both reference number and email');
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const response = await fetch(`/api/bookings/lookup?reference=${referenceNumber}&email=${email}`);
      if (!response.ok) {
        throw new Error('Booking not found. Please check your reference and email.');
      }
      const booking = await response.json();
      navigate(`/booking-confirmation?bookingId=${booking.id}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to find booking');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ py: { xs: 3, md: 6 } }}>
      <Paper
        elevation={0}
        sx={{
          p: { xs: 3, md: 5 },
          borderRadius: 3,
          border: `1px solid ${theme.palette.divider}`,
        }}
      >
        <Typography variant="h4" sx={{ fontWeight: 700, mb: 1 }}>
          Find Your Booking
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
          Enter your booking reference and email to view your booking details
        </Typography>

        <Box component="form" onSubmit={(e) => { e.preventDefault(); handleLookup(); }}>
          <TextField
            fullWidth
            label="Booking Reference"
            placeholder="e.g., BKG-20240624-1234"
            value={referenceNumber}
            onChange={(e) => setReferenceNumber(e.target.value)}
            sx={{ mb: 2 }}
            disabled={loading}
          />
          <TextField
            fullWidth
            label="Email Address"
            type="email"
            placeholder="Enter the email used for booking"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            sx={{ mb: 3 }}
            disabled={loading}
          />

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          <Button
            fullWidth
            variant="contained"
            size="large"
            onClick={handleLookup}
            disabled={loading}
            startIcon={<Search />}
            sx={{
              py: 1.5,
              backgroundColor: theme.palette.primary.main,
              '&:hover': {
                backgroundColor: theme.palette.primary.dark,
              },
            }}
          >
            {loading ? 'Searching...' : 'Find My Booking'}
          </Button>
        </Box>

        <Divider sx={{ my: 3 }} />

        <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center' }}>
          A confirmation email was sent to your provided email address
          <br />
          with your booking reference number.
        </Typography>
      </Paper>
    </Container>
  );
};