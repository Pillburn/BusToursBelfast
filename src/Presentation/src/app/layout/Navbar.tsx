// src/app/layout/Navbar.tsx
import { AppBar, Toolbar, Typography, Button, Box, useMediaQuery, IconButton, Drawer, List, ListItem, ListItemText } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import MenuIcon from '@mui/icons-material/Menu';
import { useTheme } from '@mui/material/styles';

const Navbar = () => {
  const navigate = useNavigate();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [drawerOpen, setDrawerOpen] = useState(false);

  const navItems = [
    { label: 'Tours', path: '/tours' },
    { label: 'About', path: '/about' },
    { label: 'Contact', path: '/contact' },
  ];

  return (
    <AppBar 
      position="sticky"
      sx={{
        // ✅ Force green colors
        backgroundColor: theme.palette.primary.main,
        backgroundImage: `linear-gradient(90deg, ${theme.palette.primary.main}, ${theme.palette.primary.light})`,
        color: theme.palette.common.white,
        boxShadow: '0 4px 20px rgba(26, 60, 42, 0.15)',
      }}
    >
      <Toolbar sx={{ justifyContent: 'space-between', py: 1 }}>
        <Typography 
          variant="h5" 
          component="div" 
          sx={{ 
            fontWeight: 700,
            cursor: 'pointer',
            color: theme.palette.common.white,
            '&:hover': { opacity: 0.8 },
          }}
          onClick={() => navigate('/')}
        >
          🇮🇪 Béal Feirste Turas
        </Typography>

        {isMobile ? (
          <>
            <IconButton 
              color="inherit" 
              onClick={() => setDrawerOpen(true)}
              sx={{ 
                color: theme.palette.common.white,
                '&:hover': { backgroundColor: 'rgba(255,255,255,0.1)' }
              }}
            >
              <MenuIcon />
            </IconButton>
            <Drawer
              anchor="right"
              open={drawerOpen}
              onClose={() => setDrawerOpen(false)}
              PaperProps={{
                sx: {
                  width: 250,
                  backgroundColor: theme.palette.primary.main,
                  color: theme.palette.common.white,
                }
              }}
            >
              <List sx={{ mt: 4 }}>
                {navItems.map((item) => (
                  <ListItem 
                    key={item.path} 
                    onClick={() => {
                      navigate(item.path);
                      setDrawerOpen(false);
                    }}
                    sx={{ 
                      '&:hover': { 
                        backgroundColor: 'rgba(255,255,255,0.1)',
                        borderRadius: 1,
                      }
                    }}
                  >
                    <ListItemText primary={item.label} />
                  </ListItem>
                ))}
              </List>
            </Drawer>
          </>
        ) : (
          <Box sx={{ display: 'flex', gap: 2 }}>
            {navItems.map((item) => (
              <Button 
                key={item.path} 
                color="inherit" 
                onClick={() => navigate(item.path)}
                sx={{
                  color: theme.palette.common.white,
                  fontWeight: 500,
                  '&:hover': {
                    backgroundColor: 'rgba(255,255,255,0.1)',
                  },
                }}
              >
                {item.label}
              </Button>
            ))}
          </Box>
        )}
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;