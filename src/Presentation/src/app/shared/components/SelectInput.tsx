/**imports */
import { FormControl, FormHelperText, InputLabel, MenuItem } from "@mui/material";
import Select, { SelectProps } from "@mui/material/Select/";
import { FieldValues, useController, UseControllerProps } from "react-hook-form";

/**type/interface declarations */
type Props<T extends FieldValues> ={
    items: {text:string, value:string}[];
    label:string;
}&UseControllerProps<T>&Partial<SelectProps>
/**Component Declaration export function export type */
export default function SelectInput<T extends FieldValues>(props:Props<T>){
/**Hook Zone */
    const {field, fieldState}=useController({...props});

/**Logic Section  */
/**TSX Render Block*/
    return (
    
    <FormControl fullWidth error={!!fieldState.error}>
        <InputLabel>{props.label}</InputLabel>
        <Select
            value={field.value|| ''}
            label={props.label}
            onChange={field.onChange}
            
        >
            {props.items.map(item => 
                <MenuItem key={item.value} value={item.value}>
                    {item.text}
                </MenuItem>
        )}
        </Select>
        <FormHelperText>{fieldState.error?.message}</FormHelperText>
    </FormControl>
    
    )
}


/**exports */