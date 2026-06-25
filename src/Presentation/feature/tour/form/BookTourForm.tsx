import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  TextField,
  Button,
  Box,
  Grid,
  Typography,
  IconButton,
  InputAdornment,
  Alert,
  CircularProgress,
  Paper,
  Stepper,
  Step,
  StepLabel,
  Divider,
  Chip,
  useTheme,
  alpha
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import AddIcon from '@mui/icons-material/Add';
import RemoveIcon from '@mui/icons-material/Remove';
// ... and many more
import {
  LocalActivity,
  Person,
  Email,
  Phone,
  LocationOn,
  FlightTakeoff,
  CalendarToday,
  People,
  Info,
  Verified,
  Payment,
  CheckCircle
 } from '@mui/icons-material';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
// ✅ Correct import
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { DatePicker } from '@mui/x-date-pickers';
//import './BookTourForm.css'; // Optional: for styling

// Type definitions

interface BookingFormData{
  customerName: string;
  email: string;
  phoneNumber: string;
  numberOfParticipants: {
    adults: number;
    children: number;
    infants: number;
  };
  preferredDate: string;
  pickupLocation: string;
  specialRequests: string;
  passportNumber: string;
  dateOfBirth: string | null;
  emergencyContact: string;
  travelInsuranceDetails: string;
}

interface BookTourFormProps {
  open: boolean;
  tourName: string;
  tourPrice: number;
  isLoading: boolean;
  onSubmit: (data: BookingFormData) => void | Promise<void>; // ← Should match
  onClose: () => void;
  bookingStatus: 'idle' | 'loading' | 'success' | 'error';
  error?: Error | null;
}

export const BookTourForm: React.FC<BookTourFormProps> = ({ 
  open,
  tourName, 
  tourPrice,
  isLoading, 
  onSubmit,
  onClose, 
  bookingStatus,
  error 
}) => {
  const theme = useTheme();
  const [activeStep, setActiveStep] = useState(0);

  console.log('📦 BookTourForm received open:', open, 'tourName:', tourName);
  // State for form fields

  const [formData, setFormData] = useState<BookingFormData>({
    customerName: '',
    email: '',
    phoneNumber: '',
    numberOfParticipants: {
      adults: 1,
      children: 0,
      infants: 0
    },
    preferredDate: '',
    pickupLocation: '',
    specialRequests: '',
    passportNumber: '',
    dateOfBirth: '',
    emergencyContact: '',
    travelInsuranceDetails: ''
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const steps = ['Personal Info','Tour Detail', 'Additional Info', 'Review'];

  // Handle input changes for simple fields
  const handleInputChange = (field: keyof BookingFormData) => 
    (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>)=> {
      const value = event.target.value;
      setFormData(prev => ({ ...prev, [field]: value }));
      if (errors[field]) 
      {
        setErrors(prev => ({ ...prev, [field]: '' }));
      }
  };

  const EXCHANGE_RATE = 1.18; // 1 GBP = 1.18 EUR (update this periodically)

  const handleDateChange = (field: 'preferredDate' | 'dateOfBirth') => (date: Date | null) => {
    const value = date ? date.toISOString().split('T')[0] : '';
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  };
  // Handle participant count changes
  const handleParticipantChange = (type: 'adults' | 'children' | 'infants', delta: number) => {
    setFormData(prev => ({
      ...prev,
      numberOfParticipants: {
        ...prev.numberOfParticipants,
        [type]: Math.max(0, prev.numberOfParticipants[type] + delta)
      }
    }));
  };

  const validateStep = (step: number): boolean => {
    const newErrors: Record<string, string> = {};
    
    if (step === 0) {
      if (!formData.customerName.trim()) newErrors.customerName = 'Customer name is required';
      if (!formData.email.trim()) newErrors.email = 'Email is required';
      else if (!/\S+@\S+\.\S+/.test(formData.email)) newErrors.email = 'Email is invalid';
      if (!formData.phoneNumber.trim()) newErrors.phoneNumber = 'Phone number is required';
    }
    
    if (step === 1) {
      if (!formData.preferredDate) newErrors.preferredDate = 'Preferred date is required';
      if (!formData.pickupLocation.trim()) newErrors.pickupLocation = 'Pickup location is required';
      
      const total = formData.numberOfParticipants.adults + 
                    formData.numberOfParticipants.children + 
                    formData.numberOfParticipants.infants;
      if (total === 0) newErrors.numberOfParticipants = 'At least one participant is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleNext = () => {
    if (validateStep(activeStep)) {
      setActiveStep((prev) => prev + 1);
    }
  };

  const handleBack = () => {
    setActiveStep((prev) => prev - 1);
  };
  // Handle form submission
  const handleSubmit = async(e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault();
    console.log('📝 BookTourForm: Form submitted!');
    console.log('📝 BookTourForm: Form data:', formData);
    if (validateStep(activeStep)) {
      console.log('✅ BookTourForm: Validation passed, calling onSubmit');
      await onSubmit(formData);
    }else{
      console.log('❌ BookTourForm: Validation failed');
    }
  };

  //Reset the form
  /*const handleClose = () => {
    setFormData({
      customerName: '',
      email: '',
      phoneNumber: '',
      numberOfParticipants: { adults: 1, children: 0, infants: 0 },
      preferredDate: '',
      pickupLocation: '',
      specialRequests: '',
      passportNumber: '',
      dateOfBirth: null,
      emergencyContact: '',
      travelInsuranceDetails: ''
      });
      setErrors({});
      onClose();
    };*/
  // Calculate total participants
  const totalParticipants = formData.numberOfParticipants.adults + 
                           formData.numberOfParticipants.children + 
                           formData.numberOfParticipants.infants;

  const totalPrice = totalParticipants * tourPrice;
  const totalInEur = (totalPrice * EXCHANGE_RATE).toFixed(2);
  console.log('📦 BookTourForm rendering - open:', open, 'tourName:', tourName);
  const getStepContent = (step: number) => {
    switch (step) {
      case 0:
        return (
          <Box>
            <Typography variant="subtitle1" sx={{ mb: 3, color: 'theme.palette.text.secondary' }}>
              Tell us who you are so we can prepare your booking
            </Typography>
            <Grid container spacing={3}>
              <Grid size = {{xs:12, sm:6, md:4}}>
                <TextField
                  fullWidth
                  label="Full Name"
                  value={formData.customerName}
                  onChange={handleInputChange('customerName')}
                  error={!!errors.customerName}
                  helperText={errors.customerName}
                  placeholder="As shown on ID/passport"
                  disabled={isLoading}
                  required
                  slotProps={{
                    input:
                    {
                      startAdornment: <InputAdornment position="start"><Person color="primary" /></InputAdornment>,
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    }
                  }}
                />
              </Grid>
              <Grid size= {{ xs:12, sm:6, md:4 }}>
                <TextField
                  fullWidth
                  label="Email Address"
                  type="email"
                  value={formData.email}
                  onChange={handleInputChange('email')}
                  error={!!errors.email}
                  helperText={errors.email}
                  disabled={isLoading}
                  required
                  slotProps={{
                    input:
                    {startAdornment: <InputAdornment position="start"><Email color="primary" /></InputAdornment>,}
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    }
                  }}
                />
              </Grid>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <TextField
                  fullWidth
                  label="Phone Number"
                  value={formData.phoneNumber}
                  onChange={handleInputChange('phoneNumber')}
                  error={!!errors.phoneNumber}
                  helperText={errors.phoneNumber}
                  placeholder="+1 234 567 8900"
                  disabled={isLoading}
                  required
                  slotProps={{
                    input:
                    {
                      startAdornment: <InputAdornment position="start"><Phone color="primary" /></InputAdornment>,
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    }
                  }}
                />
              </Grid>
            </Grid>
          </Box>
        );
      case 1:
        return (
          <Box>
            <Typography variant="subtitle1" sx={{ mb: 3, color: 'theme.palette.text.secondary' }}>
              Let's plan your perfect tour experience
            </Typography>
            <Grid container spacing={3}>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DatePicker
                    label="Preferred Tour Date"
                    value={formData.preferredDate ? new Date(formData.preferredDate) : null}
                    onChange={handleDateChange('preferredDate')}
                    disabled={isLoading}
                    sx={{ 
                      width: '100%',
                      '& .MuiOutlinedInput-root': {
                        borderRadius: 2,
                      }
                    }}
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        error: !!errors.preferredDate,
                        helperText: errors.preferredDate,
                        required: true,
                        InputProps: {
                          startAdornment: <InputAdornment position="start"><CalendarToday color="primary" /></InputAdornment>,
                        }
                      }
                    }}
                  />
                </LocalizationProvider>
              </Grid>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <TextField
                  fullWidth
                  label="Pickup Location"
                  value={formData.pickupLocation}
                  onChange={handleInputChange('pickupLocation')}
                  error={!!errors.pickupLocation}
                  helperText={errors.pickupLocation}
                  placeholder="Hotel name and address"
                  disabled={isLoading}
                  required
                  slotProps={{
                    input:
                    {
                      startAdornment: <InputAdornment position="start"><LocationOn color="primary" /></InputAdornment>,
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    }
                  }}
                />
              </Grid>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <Paper 
                  elevation={0} 
                  sx={{ 
                    p: 3, 
                    bgcolor: alpha(theme.palette.primary.main, 0.04),
                    borderRadius: 3,
                    border: `1px solid ${alpha(theme.palette.primary.main, 0.1)}`
                  }}
                >
                  <Typography variant="subtitle2" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <People color="primary" /> Number of Participants
                  </Typography>
                  <Grid container spacing={2}>
                    {(['adults', 'children', 'infants'] as const).map((type) => (
                      <Grid size= {{xs:12, sm:6, md: 4}} key={type}>
                        <Box sx={{ textAlign: 'center' }}>
                          <Typography variant="body2" color="theme.palette.text.secondary" gutterBottom>
                            {type.charAt(0).toUpperCase() + type.slice(1)} 
                            {type === 'adults' ? ' (12+)' : type === 'children' ? ' (3-11)' : ' (0-2)'}
                          </Typography>
                          <Box sx={{ 
                            display: 'flex', 
                            alignItems: 'center', 
                            justifyContent: 'center', 
                            gap: 1,
                            bgcolor: 'white',
                            borderRadius: 2,
                            py: 0.5,
                            px: 1,
                            border: `1px solid ${alpha(theme.palette.primary.main, 0.1)}`
                          }}>
                            <IconButton 
                              size="small" 
                              onClick={() => handleParticipantChange(type, -1)}
                              disabled={isLoading || formData.numberOfParticipants[type] === 0}
                              sx={{ 
                                bgcolor: alpha(theme.palette.primary.main, 0.1),
                                '&:hover': { bgcolor: alpha(theme.palette.primary.main, 0.2) }
                              }}
                            >
                              <RemoveIcon fontSize="small" />
                            </IconButton>
                            <Typography variant="h6" sx={{ minWidth: 30 }}>
                              {formData.numberOfParticipants[type]}
                            </Typography>
                            <IconButton 
                              size="small" 
                              onClick={() => handleParticipantChange(type, 1)}
                              disabled={isLoading}
                              sx={{ 
                                bgcolor: alpha(theme.palette.primary.main, 0.1),
                                '&:hover': { bgcolor: alpha(theme.palette.primary.main, 0.2) }
                              }}
                            >
                              <AddIcon fontSize="small" />
                            </IconButton>
                          </Box>
                        </Box>
                      </Grid>
                    ))}
                  </Grid>
                  {errors.numberOfParticipants && (
                    <Typography color="error" variant="caption" sx={{ display: 'block', mt: 1 }}>
                      {errors.numberOfParticipants}
                    </Typography>
                  )}
                  <Box sx={{ mt: 2, p: 1.5, bgcolor: alpha(theme.palette.info.main, 0.1), borderRadius: 2 }}>
                    <Typography variant="body2" color="info.main">
                      Total: {totalParticipants} participant(s) · Total: £{totalPrice.toFixed(2)}
                    </Typography>
                  </Box>
                </Paper>
              </Grid>
            </Grid>
          </Box>
        );
      case 2:
        return (
          <Box>
            <Typography variant="subtitle1" sx={{ mb: 3, color: 'theme.palette.text.secondary' }}>
              Help us provide the best experience
            </Typography>
            <Grid container spacing={3}>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <TextField
                  fullWidth
                  label="Passport Number"
                  value={formData.passportNumber}
                  onChange={handleInputChange('passportNumber')}
                  placeholder="Required for international tours"
                  disabled={isLoading}
                  slotProps={{
                    input:
                    {
                    startAdornment: <InputAdornment position="start"><Verified color="primary" /></InputAdornment>,
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    }
                  }}
                />
              </Grid>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DatePicker
                    label="Date of Birth"
                    value={formData.dateOfBirth ? new Date(formData.dateOfBirth) : null}
                    onChange={handleDateChange('dateOfBirth')}
                    disabled={isLoading}
                    sx={{ 
                      width: '100%',
                      '& .MuiOutlinedInput-root': {
                        borderRadius: 2,
                      }
                    }}
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        InputProps: {
                          startAdornment: <InputAdornment position="start"><CalendarToday color="primary" /></InputAdornment>,
                        }
                      }
                    }}
                  />
                </LocalizationProvider>
              </Grid>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <TextField
                  fullWidth
                  label="Emergency Contact"
                  value={formData.emergencyContact}
                  onChange={handleInputChange('emergencyContact')}
                  placeholder="Name and phone number"
                  disabled={isLoading}
                  slotProps={{
                    input:
                    {
                      startAdornment: <InputAdornment position="start"><Info color="primary" /></InputAdornment>,
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    }
                  }}
                />
              </Grid>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <TextField
                  fullWidth
                  label="Travel Insurance Details"
                  value={formData.travelInsuranceDetails}
                  onChange={handleInputChange('travelInsuranceDetails')}
                  placeholder="Insurance provider and policy number"
                  disabled={isLoading}
                  slotProps={{
                    input:
                    {
                      startAdornment: <InputAdornment position="start"><Payment color="primary" /></InputAdornment>,
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    }
                  }}
                />
              </Grid>
              <Grid size= {{xs:12, sm:6, md: 4}}>
                <TextField
                  fullWidth
                  label="Special Requests"
                  multiline
                  rows={3}
                  value={formData.specialRequests}
                  onChange={handleInputChange('specialRequests')}
                  placeholder="Dietary restrictions, mobility needs, allergies, etc."
                  disabled={isLoading}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    }
                  }}
                />
              </Grid>
            </Grid>
          </Box>
        );
      case 3:
        return (
          <Box>
            <Typography variant="subtitle1" sx={{ mb: 3, color: 'theme.palette.text.secondary' }}>
              Review your booking details
            </Typography>
            <Paper 
              elevation={0} 
              sx={{ 
                p: 3, 
                bgcolor: alpha(theme.palette.primary.main, 0.03),
                borderRadius: 3,
                border: `1px solid ${alpha(theme.palette.primary.main, 0.08)}`
              }}
            >
              <Grid container spacing={2}>
                <Grid size= {{xs:12, sm:6, md: 4}}>
                  <Typography variant="subtitle2" color="primary" gutterBottom>
                    <Person sx={{ mr: 1, verticalAlign: 'middle' }} fontSize="small" />
                    Personal Information
                  </Typography>
                  <Box sx={{ pl: 2, mb: 2 }}>
                    <Typography variant="body2"><strong>Name:</strong> {formData.customerName}</Typography>
                    <Typography variant="body2"><strong>Email:</strong> {formData.email}</Typography>
                    <Typography variant="body2"><strong>Phone:</strong> {formData.phoneNumber}</Typography>
                  </Box>
                </Grid>
                <Grid size= {{xs:12, sm:6, md: 4}}>
                  <Typography variant="subtitle2" color="primary" gutterBottom>
                    <FlightTakeoff sx={{ mr: 1, verticalAlign: 'middle' }} fontSize="small" />
                    Tour Details
                  </Typography>
                  <Box sx={{ pl: 2, mb: 2 }}>
                    <Typography variant="body2"><strong>Tour:</strong> {tourName}</Typography>
                    <Typography variant="body2"><strong>Date:</strong> {formData.preferredDate}</Typography>
                    <Typography variant="body2"><strong>Pickup:</strong> {formData.pickupLocation}</Typography>
                    <Typography variant="body2"><strong>Participants:</strong> {totalParticipants} people</Typography>
                    <Typography variant="body2"><strong>Total Price:</strong> £{totalPrice.toFixed(2)}</Typography>
                  </Box>
                </Grid>
                {formData.specialRequests && (
                  <Grid size= {{xs:12, sm:6, md: 4}}>
                    <Typography variant="subtitle2" color="primary" gutterBottom>
                      <Info sx={{ mr: 1, verticalAlign: 'middle' }} fontSize="small" />
                      Special Requests
                    </Typography>
                    <Typography variant="body2" sx={{ pl: 2 }}>{formData.specialRequests}</Typography>
                  </Grid>
                )}
              </Grid>
            </Paper>
          </Box>
        );
      default:
        return null;
    }
  };

  return (
    <Dialog 
      open={open} 
      onClose={onClose}
      maxWidth="md"
      fullWidth
      PaperProps={{
        sx: { 
          borderRadius: 4,
          maxHeight: '90vh',
          overflow: 'hidden',
          bgcolor: 'theme.palette.background.paper'
        }
      }}
    >
      <DialogTitle sx={{ 
        p: 0,
        position: 'relative',
        backgroundColor: theme.palette.primary.main,  // ← Should be green
        backgroundImage: `linear-gradient(135deg, ${theme.palette.primary.main}, ${theme.palette.primary.dark})`,
        color: theme.palette.common.white,
        pt: 3,
        px: 3,
        pb: 2
      }}>
        <Box display="flex" justifyContent="space-between" alignItems="flex-start">
          <Box>
            <Typography  sx={{ fontWeight: 700, mb: 0.5 }}>
              Book Your Tour
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.9 }}>
              {tourName} · £{tourPrice}/person
              <span style={{ marginLeft: '12px', opacity: 0.7 }}>
                (≈ €{totalInEur})
              </span>  
            </Typography>
          </Box>
          <IconButton 
            onClick={onClose} 
            disabled={isLoading}
            sx={{ 
              color: 'white',
              bgcolor: 'rgba(255,255,255,0.2)',
              '&:hover': { bgcolor: 'rgba(255,255,255,0.3)' }
            }}
          >
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>

      <DialogContent sx={{ p: 0, overflow: 'hidden' }}>
        <Box sx={{ px: 3, pt: 3, pb: 2 }}>
          <Stepper activeStep={activeStep} alternativeLabel>
            {steps.map((label) => (
              <Step key={label}>
                <StepLabel 
                  sx={{
                    '& .MuiStepLabel-label': { 
                      fontSize: '0.75rem',
                      fontWeight: 500
                    }
                  }}
                >
                  {label}
                </StepLabel>
              </Step>
            ))}
          </Stepper>
        </Box>

        <Divider />

        <Box sx={{ p: 3, overflowY: 'auto', maxHeight: 'calc(70vh - 200px)' }}>
          {bookingStatus === 'loading' && (
            <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', py: 6 }}>
              <CircularProgress size={60} />
              <Typography variant="body1" sx={{ mt: 2, color: 'theme.palette.text.secondary' }}>
                Processing your booking...
              </Typography>
            </Box>
          )}
          
          {bookingStatus === 'success' && (
            <Box sx={{ textAlign: 'center', py: 6 }}>
              <Box sx={{ 
                width: 80, 
                height: 80, 
                borderRadius: '50%', 
                bgcolor: alpha(theme.palette.success.main, 0.15),
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                mx: 'auto',
                mb: 3
              }}>
                <CheckCircle sx={{ fontSize: 48, color: 'success.main' }} />
              </Box>
              <Typography variant="h5" gutterBottom fontWeight={600}>
                Booking Confirmed! 🎉
              </Typography>
              <Typography variant="body1" color="theme.palette.text.secondary">
                Your tour has been booked successfully. Check your email for confirmation.
              </Typography>
            </Box>
          )}
          
          {bookingStatus === 'error' && (
            <Box sx={{ py: 3 }}>
              <Alert severity="error" sx={{ borderRadius: 2 }}>
                <Typography variant="body1" fontWeight={500}>Booking Failed</Typography>
                <Typography variant="body2">{error?.message || 'Please try again.'}</Typography>
              </Alert>
            </Box>
          )}
          
          {bookingStatus === 'idle' && (
            <form onSubmit={handleSubmit}>
              {getStepContent(activeStep)}
              <Box sx={{ 
                p: 3, 
                borderTop: `1px solid ${theme.palette.divider}`,
                bgcolor: 'background.paper',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
              }}>
                <Box>
                  {activeStep === 0 && (
                    <Chip 
                      icon={<LocalActivity />}
                      label={`${tourName}`}
                      size="small"
                      sx={{ mr: 1 }}
                    />
                  )}
                  {activeStep === 1 && (
                    <Chip 
                      icon={<People />}
                      label={`${totalParticipants} participants`}
                      size="small"
                      sx={{ mr: 1 }}
                    />
                  )}
                  {activeStep === 3 && (
                    <Chip 
                      icon={<Payment />}
                      label={`£${totalPrice.toFixed(2)}`}
                      size="small"
                      color="primary"
                      sx={{ mr: 1 }}
                    />
                  )}
                </Box>
                <Box sx={{ display: 'flex', gap: 1 }}>
                  {activeStep > 0 && (
                    <Button onClick={handleBack} disabled={isLoading}>
                      Back
                    </Button>
                  )}
                  {activeStep < steps.length - 1 ? (
                    <Button 
                      variant="contained" 
                      onClick={handleNext}
                      disabled={isLoading}
                      sx={{ borderRadius: 2, px: 4 }}
                    >
                      Next
                    </Button>
                  ) : (
                    <Button 
                      type="submit" 
                      variant="contained" 
                      disabled={isLoading}
                      endIcon={<CheckCircle />}
                      sx={{ 
                        borderRadius: 2, 
                        px: 4,
                        background: `linear-gradient(135deg, ${theme.palette.primary.main}, ${theme.palette.primary.dark})`,
                        '&:hover': {
                          background: `linear-gradient(135deg, ${theme.palette.primary.dark}, ${theme.palette.primary.dark})`,
                        }
                      }}
                    >
                      {isLoading ? 'Processing...' : 'Confirm Booking'}
                    </Button>
                  )}
                </Box>
              </Box>
          </form>
          )}
        </Box>
      </DialogContent>
    </Dialog>
  );
};

export default BookTourForm;