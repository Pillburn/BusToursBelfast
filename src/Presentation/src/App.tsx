import { Box, Container, CssBaseline } from "@mui/material";
import Navbar from "../src/app/layout/Navbar";
import { Outlet , useLocation } from "react-router-dom";
import HomePage from "../feature/home/HomePage";
import { useEffect } from "react";


function App() {
  const location = useLocation(); // Gets the current Location
    // App.tsx
  console.log('🔍 Environment check:');
  console.log('VITE_API_URL:', import.meta.env.VITE_API_URL);
  console.log('VITE_STRIPE_PUBLISHABLE_KEY:', import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY ? 'Set' : 'Missing');
  useEffect(() => {
    const testAPI = async () => {
      try {
        console.log('🔍 Testing API connection...');
        
        // ✅ Use relative URL - goes through Vite proxy
        const response = await fetch('/api/health');
        
        if (response.ok) {
          const data = await response.json();
          console.log('✅ API test successful:', data);
        } else {
          console.error('❌ API test failed:', response.status);
        }
      } catch (error) {
        console.error('❌ API test failed:', error);
      }
    };
    
    testAPI();
  }, []);
  return (
    
    <Box sx={{bgcolor: '#eeeeee', minHeight: '100vh'}}>
      <CssBaseline/>
    {location.pathname === '/' ? ( 
      <HomePage/> 
    ) : ( //if the current location pathname is '/' then show the homepage otherwise, show the site with the Navbar on
      <>
        <Navbar/>
        <Container maxWidth='xl' sx={{mt: 3}}>
          <Outlet/>
        </Container>
      </>
    )}
  </Box>
  );
}
export default App
