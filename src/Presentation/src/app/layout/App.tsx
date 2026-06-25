// App.tsx
import { ThemeProvider, useTheme } from '@mui/material/styles';
import { Box, CssBaseline, Container, GlobalStyles } from "@mui/material";
import { Outlet, useLocation } from "react-router-dom";
import Navbar from './Navbar';

function App() {
  const location = useLocation();
  const isHomePage = location.pathname === '/';
  const theme = useTheme();
  console.log('🎨 Current theme primary color:', theme.palette.primary.main);
  return (
    <ThemeProvider theme={theme}>
      <GlobalStyles
        styles={{
          '.MuiAppBar-root': {
            backgroundColor: '#1a3c2a !important',
            backgroundImage: 'linear-gradient(90deg, #1a3c2a, #2d5a3d) !important',
            color: '#ffffff !important',
          },
          '.MuiButton-containedPrimary': {
            backgroundColor: '#1a3c2a !important',
          },
        }}
      />
      <CssBaseline />
      <Box sx={{ 
        minHeight: '100vh',
        // ✅ Let the HomePage control its own background
        bgcolor: isHomePage ? 'transparent' : 'background.default',
      }}>
        {!isHomePage && <Navbar />}
        <Box sx={{ 
          // ✅ Full width container for HomePage
          width: '100%',
        }}>
          {isHomePage ? <Outlet /> : (
            <Container maxWidth='xl' sx={{ mt: 3 }}>
              <Outlet />
            </Container>
          )}
        </Box>
      </Box>
    </ThemeProvider>
  );
}

export default App;