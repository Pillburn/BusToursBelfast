import { Grid} from "@mui/material";
import  { TourList } from "./TourList.tsx";


export default function TourDashboard() {
  
  return (
    <Grid container spacing={3} sx={{display:'flex'}}>
      <Grid size={8}>
        <TourList />
      </Grid>
      <Grid size={4}>
        <ActivityFilters/>
      </Grid>
    </Grid>
  )
}