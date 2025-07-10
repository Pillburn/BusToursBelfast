import {Box, Card, CardContent, CardHeader } from "@mui/material";

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