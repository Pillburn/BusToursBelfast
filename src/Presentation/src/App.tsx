import { Box, Container, CssBaseline } from "@mui/material";
import Navbar from "../src/app/layout/Navbar";
import { Outlet } from "react-router";
import HomePage from "../feature/home/HomePage";


function App() {
  return (
  <Box sx={{bgcolor: '#eeeeee'}}>
    <CssBaseline/>
    {location.pathname === '/' ? <HomePage/> : ( //if the current location pathname is '/' then show the homepage otherwise, show the site with the Navbar on
      <>
        <Navbar/>
        <Container maxWidth='xl' sx={{mt: 3}}>
          <Outlet/>
        </Container>
      </>
    )}

  </Box>
      
  )
}
export default App
