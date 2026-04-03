import Box from "@mui/material/Box";
import { TourCard } from "./TourCard";
import { useTours } from "../../../lib/hooks/useTour";
import { Typography } from "@mui/material";



export default function TourList() {
  const {tours, isLoading} = useTours();
  if (isLoading) return <Typography>Loading...</Typography>
  if(!tours) return <Typography>No Tour Found</Typography>
  return (
    <Box sx={{display: 'flex', flexDirection: 'column', gap:3}}>
        {tours.map(Tour => (
            <TourCard 
            tour={Tour}
            />
        ))}
    </Box>
    
  )
}