import { AppBar, Box, LinearProgress, MenuItem, Toolbar, Typography } from "@mui/material";
import { useStore } from "../../../lib/hooks/useStore";
import { useEffect, useState } from "react";
import { Group } from "@mui/icons-material";
import { NavLink } from "react-router";
import MenuItemLink from "../shared/components/MenuItemLink";


// Navbar.tsx
export default function Navbar() {
  const [debouncedIsLoading, setDebouncedIsLoading] = useState(false);
  const store = useStore();

  // ✅ Get uiStore safely at the top level
  const uiStore = store?.uiStore;
  const isLoading = uiStore?.isLoading || false;

  // ✅ useEffect depends on safe variables
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedIsLoading(isLoading), 300);
    return () => clearTimeout(timer);
  }, [isLoading]);

  // ✅ Early return after all hooks
  if (!store || !uiStore) {
    return (
      <AppBar position="static">
        <Toolbar>
          <Typography>Loading...</Typography>
        </Toolbar>
      </AppBar>
    );
  }

  return (
    <AppBar position="static">
     
      <Toolbar sx={{display:'flex', justifyContent: 'space-between'}}>
                <Box>
                    <MenuItem component={NavLink} to='/'>
                        <Group fontSize="large"/>
                        <Typography variant="h4" fontWeight='bold'>
                             Belfast Bus Tours
                        </Typography>
                    </MenuItem>
                </Box>
                <Box sx={{display:'flex'}}>
                    <MenuItemLink to='/activities'>
                    Tours
                    </MenuItemLink>
                    <MenuItemLink to='/about'>
                    Airport Transfers
                    </MenuItemLink>
                    <MenuItemLink to='/counter'>
                    Contact Us
                    </MenuItemLink>
                    <MenuItemLink to='/errors'>
                    Test Errors
                    </MenuItemLink>
                </Box>
        {debouncedIsLoading && <LinearProgress />}
      </Toolbar>
    </AppBar>
  );
}