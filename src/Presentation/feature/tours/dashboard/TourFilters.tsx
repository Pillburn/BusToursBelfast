// features/tours/TourFilters.tsx
import { useState } from 'react';
import {
  Box,
  TextField,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
  Slider,
  Button,
  useTheme,
  Typography
} from '@mui/material';
import { Search, Clear } from '@mui/icons-material';

interface TourFiltersProps {
  onFilter: (filters: unknown) => void;
}

export const TourFilters = ({ onFilter }: TourFiltersProps) => {
  const theme = useTheme();
  const [search, setSearch] = useState('');
  const [priceRange, setPriceRange] = useState<[number, number]>([0, 300]);
  const [duration, setDuration] = useState('');

  const handleApply = () => {
    onFilter({
      search,
      minPrice: priceRange[0],
      maxPrice: priceRange[1],
      duration,
    });
  };

  const handleClear = () => {
    setSearch('');
    setPriceRange([0, 300]);
    setDuration('');
    onFilter({});
  };

  return (
    <Box
      sx={{
        p: 3,
        bgcolor: theme.palette.background.paper,
        borderRadius: 3,
        border: `1px solid ${theme.palette.divider}`,
        mb: 3,
      }}
    >
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, alignItems: 'center' }}>
        <TextField
          placeholder="Search tours..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          size="small"
          sx={{ flex: 2, minWidth: 200 }}
          InputProps={{
            startAdornment: <Search sx={{ mr: 1, color: 'text.secondary' }} />,
          }}
        />

        <FormControl size="small" sx={{ flex: 1, minWidth: 150 }}>
          <InputLabel>Duration</InputLabel>
          <Select
            value={duration}
            label="Duration"
            onChange={(e) => setDuration(e.target.value)}
          >
            <MenuItem value="">Any</MenuItem>
            <MenuItem value="half-day">Half Day</MenuItem>
            <MenuItem value="full-day">Full Day</MenuItem>
            <MenuItem value="multi-day">Multi Day</MenuItem>
          </Select>
        </FormControl>

        <Box sx={{ flex: 1.5, minWidth: 150 }}>
          <Typography variant="caption" color="text.secondary">
            Price: £{priceRange[0]} - £{priceRange[1]}
          </Typography>
          <Slider
            value={priceRange}
            onChange={(_, newValue) => setPriceRange(newValue as [number, number])}
            min={0}
            max={300}
            sx={{ color: theme.palette.primary.main }}
          />
        </Box>

        <Button
          variant="contained"
          onClick={handleApply}
          sx={{
            backgroundColor: theme.palette.primary.main,
            '&:hover': { backgroundColor: theme.palette.primary.dark },
          }}
        >
          Apply Filters
        </Button>

        <Button
          variant="outlined"
          onClick={handleClear}
          startIcon={<Clear />}
        >
          Clear
        </Button>
      </Box>
    </Box>
  );
};