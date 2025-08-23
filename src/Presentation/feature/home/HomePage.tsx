import { Group } from "@mui/icons-material";
import { Box, Button, Paper, Typography } from "@mui/material";
import { Link } from "react-router-dom";

export default function HomePage() {
  return (
    <Paper
      sx={{
        color: 'white',
        display:'flex',
        flexDirection:'column',
        gap:6,
        alignItems:'center',
        alignContent:'center',
        justifyContent:'center',
        height: '100vh',
        width: '100vw',
        backgroundImage: 'linear-gradient(120deg,rgb(114, 139, 114) 12%, #53a653 30%, #2e662e 60%)',
        margin: 0,
        padding: 2,
        boxSizing: 'border-box'
      }}
    >
      <Box sx={{
        display:'flex',alignItems:'center', alignContent: 'center',
        color:'white',gap:3, flexWrap: 'wrap', justifyContent: 'center'}}
      >
        <Group sx={{height: 110, width: 110}}/>
        <Typography variant="h1" sx={{ fontSize: { xs: '2.5rem', md: '3.5rem' }, textAlign: 'center' }}>
          Bus Tours Belfast
        </Typography>
      </Box>
      <Typography variant="h2" sx={{ fontSize: { xs: '1.8rem', md: '2.5rem' }, textAlign: 'center', fontWeight: 300 }}>
        See the City, See the Country
      </Typography>
      <Button
          component={Link}
          to='/activities'
          size="large"
          variant="contained"
          sx={{
            height: 80, 
            borderRadius: 4, 
            fontSize: '1.5rem',
            backgroundColor: '#2e662e',
            '&:hover': {
              backgroundColor: '#3a7a3a',
              transform: 'translateY(-2px)',
              boxShadow: '0 4px 8px rgba(0, 0, 0, 0.2)'
            }
          }}
          >
          Bus Tours
      </Button>
    </Paper>
  )
}