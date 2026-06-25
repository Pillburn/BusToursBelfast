// components/tour/TourCard.tsx
import { Card, CardContent, CardMedia, Typography, Button, Box, Chip, useTheme } from "@mui/material";
import { LocalActivity } from "@mui/icons-material";

interface Tour {
  id: string;
  title: string;
  description: string;
  price: number;
  imageUrl?: string;
  duration?: string;
  rating?: number;
  includes?: string[];
}

interface TourCardProps {
  tour: Tour;
  onBookNow?: (tourId: string) => void;
}

export const TourCard = ({ tour, onBookNow }: TourCardProps) => {
  const theme = useTheme();  // ← Use theme hook

  return (
    <Card sx={{ 
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      border: `1px solid ${theme.palette.primary.light}20`, 
      borderRadius: theme.spacing(2),  // Returns '16px' if spacing unit is 8
      overflow: 'hidden',
      transition: 'all 0.3s ease',
      '&:hover': {
        transform: 'translateY(-8px)',
        boxShadow: theme.shadows[8],
      },
    }}>
      <Box sx={{ position: 'relative' }}>
        <CardMedia
          component="img"
          height="200"
          image={tour.imageUrl || '/images/default-tour.jpg'}
          alt={tour.title}
          sx={{ objectFit: 'cover' }}
        />
        {tour.rating && (
          <Chip
            label={`⭐ ${tour.rating}`}
            size="small"
            sx={{
              position: 'absolute',
              top: 12,
              right: 12,
              bgcolor: theme.palette.secondary.main,
              color: theme.palette.common.white,
              fontWeight: 600,
            }}
          />
        )}
      </Box>
      
      <CardContent sx={{ flexGrow: 1, p: 3 }}>
        <Typography 
          
          component="h2" 
          sx={{ 
            fontWeight: 700,
          }}
        >
          {tour.title}
        </Typography>
        
        <Typography 
          variant="body2" 
          color="text.secondary" 
          sx={{ mb: 2, lineHeight: 1.6 }}
        >
          {tour.description}
        </Typography>

        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5, mb: 2 }}>
          {tour.includes?.slice(0, 3).map((item, index) => (
            <Chip
              key={index}
              label={item}
              size="small"
              sx={{ 
                bgcolor: theme.palette.primary.light + '20', // 20% opacity
                color: theme.palette.primary.main,
                fontSize: '0.7rem',
              }}
            />
          ))}
          {tour.includes && tour.includes.length > 3 && (
            <Chip
              label={`+${tour.includes.length - 3} more`}
              size="small"
              sx={{ 
                bgcolor: 'rgba(0,0,0,0.05)',
                color: theme.palette.text.secondary,
                fontSize: '0.7rem',
              }}
            />
          )}
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 2 }}>
          <Box>
            <Typography 
              sx={{ fontWeight: 700 }}
            >
              £{tour.price}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              / person
            </Typography>
          </Box>
          
          {tour.duration && (
            <Typography variant="body2" color="text.secondary">
              🕐 {tour.duration}
            </Typography>
          )}
        </Box>

        <Button
          variant="contained"
          fullWidth
          size="large"
          endIcon={<LocalActivity />}
          onClick={() => onBookNow?.(tour.id)}
          sx={{ 
            mt: 2,
            py: 1.5,
            borderRadius: theme.shape.borderRadius,
            fontWeight: 600,
          }}
        >
          Book Now
        </Button>
      </CardContent>
    </Card>
  );
};