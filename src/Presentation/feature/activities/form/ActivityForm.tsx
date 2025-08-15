import { Box, Button, Paper, Typography } from "@mui/material";
import { useActivities } from "../../../lib/hooks/useActivities";
import { useNavigate, useParams } from "react-router";
import { useForm } from "react-hook-form"
import { useEffect } from "react";
import { ActivitySchema, activitySchema } from "../../../lib/schemas/activityschema";
import {zodResolver} from "@hookform/resolvers/zod"
import TextInput from "../../../app/shared/components/TextInput";
import SelectInput from "../../../app/shared/components/SelectInput";
import { categoryOptions } from "./categoryOptions";
import DateInput from "../../../app/shared/components/DateInput";
import LocationInput from "../../../app/shared/components/LocationInput";

export default function ActivityForm() {
  const { reset, handleSubmit,control} = useForm<ActivitySchema>({
    mode: "onTouched",
    resolver: zodResolver(activitySchema)
  });

  const navigate = useNavigate();
  const {id} = useParams();
  const {updateActivity, createActivity,activity, isLoadingActivity} = useActivities(id);

  useEffect(() => {
    if(activity) reset ({
      ...activity,
      location: {
        city: activity.city,
        venue: activity.venue,
        latitude: activity.latitude,
        longitude: activity.longitude
    }});
  }, [activity,reset]);

  const onSubmit = async (data: ActivitySchema) => {
    const {location, ...rest} = data;
    const flattenedData = {...rest, ...location}
    try {
      if(activity){
        updateActivity.mutate({...activity,...flattenedData},{
          onSuccess: () => navigate(`/activities/${activity.id}`)
        })
      }else{
        createActivity.mutate(flattenedData, { 
          onSuccess: (id) => navigate(`/activities/${id}`)
        })
      }
      
    } catch (error) {
      console.log(error);
    }
  }

  if(isLoadingActivity) return <Typography>Loading activity...</Typography>

  return (
    <Paper sx={{borderRadius: 3, padding: 3}}>
        <Typography variant="h5" color="primary">
            {activity ? 'Edit Activity' : 'Create Activity'}
        </Typography>
        <Box component='form' onSubmit={handleSubmit(onSubmit)} display= 'flex' flexDirection='column' gap={3}>
            <TextInput label="Title" name="title" control={control}/>
            <TextInput label ="Description" name="description" control={control}
            multiline rows={3}/>
            <Box display={'flex'} gap={3}>
               <SelectInput 
                items={categoryOptions}
                label='Category'
                control={control}
                name="category"
                />
              <DateInput label = "Date" name="date" control={control}/>
            </Box>
           
            <LocationInput control={control} label="Enter the location" name="location"/>
            <Box display="flex" gap={3} justifyContent='end'>
                <Button onClick={()=> {}} color='inherit'>Cancel</Button>
                <Button onClick={()=> createActivity}
                  type="submit"
                  color="success"
                  variant="contained"
                  disabled={updateActivity.isPending || createActivity.isPending}
                  >Submit</Button>
            </Box>
        </Box>
    </Paper>
  )
}
