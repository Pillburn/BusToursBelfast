import { Grid} from "@mui/material";
import Activitylist from "./ActivityList";
import ActivityFilters from "./ActivityFilters";

export default function ActivityDashboard() {
  
  return (
    <Grid container spacing={3} sx={{display:'flex'}}>
      <Grid size={8}>
        <Activitylist />
      </Grid>
      <Grid size={4}>
        <ActivityFilters/>
      </Grid>
    </Grid>
  )
}

