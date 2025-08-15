import { Grid} from "@mui/material";
import TourOptions from "./TourOptions";

export default function TourDashboard() {
  
  return (
    <Grid container spacing={3} sx={{display:'flex'}}>
      <Grid size={8}>
        <TourOptions />
      </Grid>
    </Grid>
  )
}

