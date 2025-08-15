//import { useForm } from "react-hook-form";
//import { zodResolver } from "@hookform/resolvers/zod";
import { Box, Button, Paper, Typography } from "@mui/material";
import { LockOpen } from "@mui/icons-material";
import { Link } from "react-router-dom";
import TextInput from "../../src/app/shared/components/TextInput";

//RETOOL REGISTER FORM TO EITHER CONTACT-US FORM OR BOOKING FORM

export default function RegisterForm() {
  //const {registerUser} = useAccount();
  //const {control, handleSubmit,setError,formState: { isValid, isSubmitting  }} = useForm<RegisterSchema>({
    //mode: 'onTouched',
    //resolver: zodResolver(registerSchema)
  //});

 // const onSubmit = async (data: RegisterSchema) => 
  //{
    //  await registerUser.mutateAsync(data,{
      /*  onError:(error) => {
          if(Array.isArray(error)) {
            error.forEach(err => {
                if(err.includes('Email')) setError('email',{message: err});
                else if (err.includes('Password')) setError('password',{message:err});
            })
          }
        }
          
      }); */
      
  
  return (
    <Paper 
      component={'form'} 
      //onSubmit={handleSubmit(onSubmit)}
      sx={{display: 'flex',
          flexDirection: 'column',
          p: 3,
          gap: 3,
          maxWidth: 'md',
          mx: 'auto',
          borderRadius: 3
      }}
      >
        <Box display={'flex'} alignItems={'center'} justifyContent={'center'}
        gap={3} color={'secondary.main'}> 
          <LockOpen fontSize="large"/>
          <Typography variant="h4">Register</Typography>
        </Box>
        <TextInput label='Booking Name ' name='bookingName' />
        <TextInput label='Booking Type' name='displayName' />
        <TextInput label='Password' autoComplete='current-password' type='password' name='password'/>
        <Button 
          type='submit'
          //disabled={!isValid|| isSubmitting} 
          variant='contained'
          size='large' >
            Register
        </Button>
        <Typography sx={{textAlign: 'center'}}>
          Any questions or remarks about your experience you can 
          <Typography sx={{ml: 2}} component={Link} to={'/contact-us'} color="primary">
            contact us
          </Typography>
        </Typography> 
    </Paper>
  )
}