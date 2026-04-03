import {
  Card,
  CardContent,
  CardMedia,
  Typography,
  Button,
  Box,
  Chip,
  Divider,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Rating
} from '@mui/material';
import {
  LocationOn,
  AccessTime,
  CheckCircle,
  LocalActivity
} from '@mui/icons-material';
import type {  TourCardProps } from '../../../src/types/tour';

// ✅ Fixed the syntax - ) instead of }
export const TourCard = ({ tour, onBookNow }: TourCardProps) => {
  const handleBookNow = () => {
    onBookNow?.(tour.id);
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: 'GBP'
    }).format(price);
  };

  return (
    <Card 
      sx={{ 
        maxWidth: 400, 
        height: '100%', 
        display: 'flex', 
        flexDirection: 'column',
        borderRadius: 2,
        boxShadow: 3,
        transition: 'transform 0.3s, box-shadow 0.3s',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: 6
        }
      }}
    >
      <CardMedia
        component="img"
        height="220"
        image={tour.imageUrl}
        alt={tour.title}
      />
      
      <Box sx={{ position: 'absolute', top: 16, right: 16 }}>
        <Chip
          icon={<LocationOn />}
          label="Belfast, NI"
          size="small"
          sx={{ 
            backgroundColor: 'primary.main', 
            color: 'white', 
            fontWeight: 'bold',
            mb: 1
          }}
        />
        <Chip
          icon={<AccessTime />}
          label={tour.duration}
          size="small"
          sx={{ 
            backgroundColor: 'secondary.main', 
            color: 'white', 
            fontWeight: 'bold',
            display: 'block'
          }}
        />
      </Box>
      
      <CardContent sx={{ flexGrow: 1, p: 3 }}>
        <Typography variant="h5" component="h2" gutterBottom sx={{ fontWeight: 'bold' }}>
          {tour.title}
        </Typography>
        
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Rating value={tour.rating} readOnly precision={0.5} size="small" />
          <Typography variant="body2" color="text.secondary" sx={{ ml: 1 }}>
            {tour.rating} (128 reviews)
          </Typography>
        </Box>
        
        <Typography variant="body1" color="text.secondary" paragraph>
          {tour.description}
        </Typography>
        
        {tour.includes && tour.includes.length > 0 && (
          <Box sx={{ mt: 2 }}>
            <Typography variant="h6" gutterBottom sx={{ fontSize: '1rem', fontWeight: 'bold' }}>
              Tour Includes:
            </Typography>
            <List dense sx={{ py: 0 }}>
              {tour.includes.map((item, index) => (
                <ListItem key={index} sx={{ py: 0, px: 0 }}>
                  <ListItemIcon sx={{ minWidth: 30 }}>
                    <CheckCircle color="primary" sx={{ fontSize: 20 }} />
                  </ListItemIcon>
                  <ListItemText primary={item} />
                </ListItem>
              ))}
            </List>
          </Box>
        )}
      </CardContent>
      
      <Divider />
      
      <Box sx={{ 
        p: 3, 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center',
        backgroundColor: 'grey.50'
      }}>
        <Box>
          <Typography variant="body2" color="text.secondary">
            From
          </Typography>
          <Typography variant="h5" color="primary.main" sx={{ fontWeight: 'bold' }}>
            {formatPrice(tour.price)}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            per person
          </Typography>
        </Box>
        
        <Button
          variant="contained"
          size="large"
          endIcon={<LocalActivity />}
          onClick={handleBookNow}
          sx={{ 
            borderRadius: 2,
            px: 3,
            py: 1
          }}
        >
          Book Now
        </Button>
      </Box>
    </Card>
  );
};