import { Box, Container, CssBaseline } from "@mui/material";
import Navbar from "../src/app/layout/Navbar";
import { Outlet , useLocation } from "react-router-dom";
import HomePage from "../feature/home/HomePage";


function App() {
  const location = useLocation(); // Gets the current Location

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
