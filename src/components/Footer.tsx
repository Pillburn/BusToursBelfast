// components/common/Footer.tsx
import { Box, Container, Typography, Link, Grid, IconButton, useTheme, TextField, Button } from '@mui/material';
import { Facebook, Twitter, Instagram, Email, Phone, LocationOn} from '@mui/icons-material'

export const Footer = () => {
  const theme = useTheme();

  return (
    <Box
      component="footer"
      sx={{
        bgcolor: theme.palette.primary.main,
        color: theme.palette.common.white,
        py: 6,
        mt: 'auto',
      }}
    >
      <Container maxWidth="lg">
        <Grid container spacing={4}>
          <Grid size={{ xs: 12, md: 4 }}>
            <Typography variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
              🇮🇪 Emerald Tours
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.8, mb: 2 }}>
              Discover the best tours across Ireland. From the Giant's Causeway to the Cliffs of Moher, we've got you covered.
            </Typography>
            <Box sx={{ display: 'flex', gap: 1 }}>
              <IconButton sx={{ color: 'white', bgcolor: 'rgba(255,255,255,0.1)', '&:hover': { bgcolor: 'rgba(255,255,255,0.2)' } }}>
                <Facebook />
              </IconButton>
              <IconButton sx={{ color: 'white', bgcolor: 'rgba(255,255,255,0.1)', '&:hover': { bgcolor: 'rgba(255,255,255,0.2)' } }}>
                <Twitter />
              </IconButton>
              <IconButton sx={{ color: 'white', bgcolor: 'rgba(255,255,255,0.1)', '&:hover': { bgcolor: 'rgba(255,255,255,0.2)' } }}>
                <Instagram />
              </IconButton>
            </Box>
          </Grid>

          <Grid size={{ xs: 6, md: 2 }}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 2, fontSize: '1rem' }}>
              Quick Links
            </Typography>
            <Box component="ul" sx={{ listStyle: 'none', p: 0 }}>
              <li><Link href="/tours" color="inherit" sx={{ opacity: 0.8, '&:hover': { opacity: 1 } }}>Tours</Link></li>
              <li><Link href="/my-booking" color="inherit" sx={{ opacity: 0.8, '&:hover': { opacity: 1 } }}>My Booking</Link></li>
              <li><Link href="/about" color="inherit" sx={{ opacity: 0.8, '&:hover': { opacity: 1 } }}>About</Link></li>
              <li><Link href="/contact" color="inherit" sx={{ opacity: 0.8, '&:hover': { opacity: 1 } }}>Contact</Link></li>
            </Box>
          </Grid>

          <Grid size={{ xs: 6, md: 3 }}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 2, fontSize: '1rem' }}>
              Contact
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
              <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 1, opacity: 0.8 }}>
                <Email sx={{ fontSize: 18 }} /> info@emeraldtours.ie
              </Typography>
              <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 1, opacity: 0.8 }}>
                <Phone sx={{ fontSize: 18 }} /> +353 1 234 5678
              </Typography>
              <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 1, opacity: 0.8 }}>
                <LocationOn sx={{ fontSize: 18 }} /> Dublin, Ireland
              </Typography>
            </Box>
          </Grid>

          <Grid size={{ xs: 12, md: 3 }}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 2, fontSize: '1rem' }}>
              Newsletter
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.8, mb: 2 }}>
              Subscribe for exclusive offers and new tour updates.
            </Typography>
            <Box sx={{ display: 'flex', gap: 1 }}>
              <TextField
                placeholder="Email address"
                size="small"
                sx={{
                  bgcolor: 'rgba(255,255,255,0.1)',
                  borderRadius: 1,
                  '& .MuiOutlinedInput-root': {
                    color: 'white',
                    '& fieldset': { borderColor: 'rgba(255,255,255,0.2)' },
                  },
                  '& .MuiInputLabel-root': { color: 'rgba(255,255,255,0.7)' },
                }}
              />
              <Button
                variant="contained"
                sx={{
                  bgcolor: theme.palette.secondary.main,
                  color: 'white',
                  '&:hover': { bgcolor: theme.palette.secondary.dark },
                }}
              >
                Subscribe
              </Button>
            </Box>
          </Grid>
        </Grid>

        <Box sx={{ borderTop: '1px solid rgba(255,255,255,0.1)', mt: 4, pt: 3, textAlign: 'center' }}>
          <Typography variant="body2" sx={{ opacity: 0.6 }}>
            © 2025 Emerald Tours. All rights reserved. | Privacy Policy | Terms & Conditions
          </Typography>
        </Box>
      </Container>
    </Box>
  );
};