import { Box, debounce, List, ListItemButton, TextField, Typography } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { FieldValues, useController, UseControllerProps } from "react-hook-form";
import { LocationIQSuggestion } from "../../../lib/types";
import axios from "axios";

type Props<T extends FieldValues> = {
    label:string
} & UseControllerProps<T>

export default function LocationInput<T extends FieldValues>(props: Props<T>) {
    const {field, fieldState} = useController({...props});
    const [loading, setLoading] = useState(false);
    const [suggestions, setSuggestions] = useState<LocationIQSuggestion[]>([]);
    const [inputValue, setInput] = useState(field.value || '');

    useEffect(() => {
        if(field.value && typeof field.value === 'object')
            {
                setInput(field.value.venue || '');
            }
            else
            {
                setInput(field.value || '');    
            }
    }, [field.value])

    const locationURL = 'https://api.locationiq.com/v1/autocomplete?key=pk.7b052f0b8740dbcff697fd2a1f61a73c&limit=5&dedupe=1&';
    const fetchSuggestion = useMemo(
        () => debounce(async(query: string) => {
            if(!query || query.length < 3)
            {
                setSuggestions([]);
                return;
            }
            setLoading(true)
            try {
                const res = await axios.get<LocationIQSuggestion[]>(`${locationURL}q=${query}`);
                setSuggestions(res.data)
            } catch (error) {
                console.log(error);
            }
            finally{
               setLoading(false); 
            }
        }, 1000), [locationURL]
    );

    const handleChange =  async (value:string) => 
        {
            setInput(value);
            field.onChange(
                {venue: value,city: '',latitude:0, longitude:0});
            await fetchSuggestion(value);
        }

    const handleSelect = async (location:LocationIQSuggestion) =>
        {
            const city = location.address?.city || location.address?.road || location.address?.village || '';
            console.log(city);
            const venue = location.display_name;
            console.log(venue)
            const latitude = parseFloat(location.lat);
            console.log(latitude)
            const longitude = parseFloat(location.lon);
            console.log(longitude)

            setInput(venue);
            console.log(inputValue)
            field.onChange({venue,city,latitude,longitude});
            setSuggestions([]);
        }
    
  return (
    <Box>
        <TextField 
            {...props}
            value={inputValue}
            onChange={e=> handleChange(e.target.value)}
            fullWidth
            error={!!fieldState.error}
            helperText={fieldState.error?.message}
            variant="outlined"
        />
        {loading && <Typography> Loading... </Typography>}
        {suggestions.length > 0 && (
            <List sx={{border:1}}>
                {suggestions.map(suggestion => (
                    <ListItemButton
                        divider
                        key={suggestion.place_id}
                        onClick={(e) => {
                            e.preventDefault();
                            handleSelect(suggestion);}
                        }
                    >
                        {suggestion.display_name}
                    </ListItemButton>))}
            </List>
        )}
    </Box>
  )
}