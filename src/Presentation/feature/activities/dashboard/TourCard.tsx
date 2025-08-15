import { AccessTime, Place } from "@mui/icons-material";
import { Card, CardContent, Typography, Chip, Button, Box, CardHeader, Avatar, Divider } from "@mui/material"
import { Link } from "react-router-dom";
import { formatDate } from "../../../lib/util/util.ts"

export default function TourCard() {
  return (
    <Card>
     <Box>
        <CardHeader>

        </CardHeader>
     </Box>   
     <CardContent
     title = "tour title"
     titleTypographyProps={{
        fontWeight:'bold', 
        fontSize : 20
    }}
    subheader={
    <> 
        
    </>}
        >
    
    </CardContent>
    </Card>

    
  )
}