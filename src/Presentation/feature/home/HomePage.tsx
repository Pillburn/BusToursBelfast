// feature/home/HomePage.tsx
import { Box, Button, Container, Grid, Typography, Paper } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { theme } from '../../src/theme/theme';

const HomePage = () => {
  const navigate = useNavigate();

  return (
    <Box sx={{ 
      minHeight: '100vh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      background: `linear-gradient(135deg, ${theme.palette.primary.dark}, ${theme.palette.primary.main})`,
      position: 'relative',
      overflow: 'hidden',
    }}>
      {/* Decorative elements */}
      <Box sx={{
        position: 'absolute',
        top: -100,
        right: -100,
        width: 400,
        height: 400,
        borderRadius: '50%',
        background: `radial-gradient(circle, rgba(201, 168, 76, 0.15), transparent 70%)`,
      }} />
      <Box sx={{
        position: 'absolute',
        bottom: -150,
        left: -150,
        width: 500,
        height: 500,
        borderRadius: '50%',
        background: `radial-gradient(circle, rgba(122, 171, 138, 0.15), transparent 70%)`,
      }} />
      
      <Container maxWidth="md">
        <Paper elevation={0} sx={{
          p: { xs: 4, sm: 6, md: 8 },
          borderRadius: 4,
          background: 'rgba(255, 255, 255, 0.08)',
          backdropFilter: 'blur(20px)',
          border: '1px solid rgba(255, 255, 255, 0.1)',
          textAlign: 'center',
          position: 'relative',
          zIndex: 1,
        }}>
          <Typography 
            variant="h1" 
            component="h1"
            sx={{
              fontSize: { xs: '2.5rem', sm: '3.5rem', md: '4.5rem' },
              fontWeight: 700,
              color: '#ffffff',
              mb: 2,
              letterSpacing: '-0.02em',
              textShadow: '0 2px 40px rgba(0,0,0,0.2)',
            }}
          >
            Experience Belfast
          </Typography>
          
          <Typography 
            variant="h5" 
            sx={{
              color: 'rgba(255,255,255,0.9)',
              mb: 4,
              fontStyle: 'italic',
              fontSize: { xs: '1rem', sm: '1.25rem', md: '1.5rem' },
            }}
          >
            Discover the Emerald Isle's most breathtaking tours
          </Typography>

          <Grid container spacing={2} justifyContent="center">
            <Grid size={{ xs: 12, sm: 'auto' }}>
              <Button
                variant="contained"
                size="large"
                onClick={() => navigate('/tours')}
                sx={{
                  background: `linear-gradient(135deg, ${theme.palette.secondary.main}, ${theme.palette.secondary.dark})`,
                  color: '#ffffff',
                  px: 4,
                  py: 1.5,
                  fontSize: '1.1rem',
                  '&:hover': {
                    background: `linear-gradient(135deg, ${theme.palette.secondary.dark}, ${theme.palette.secondary.main})`,
                  },
                }}
              >
                View Tours
              </Button>
            </Grid>
            <Grid size={{ xs: 12, sm: 'auto' }}>
              <Button
                variant="outlined"
                size="large"
                onClick={() => navigate('/about')}
                sx={{
                  borderColor: 'rgba(255,255,255,0.5)',
                  color: '#ffffff',
                  px: 4,
                  py: 1.5,
                  fontSize: '1.1rem',
                  '&:hover': {
                    borderColor: '#ffffff',
                    background: 'rgba(255,255,255,0.1)',
                  },
                }}
              >
                Learn More
              </Button>
            </Grid>
          </Grid>
        </Paper>
      </Container>
    </Box>
  );
};

export default HomePage;