// theme/theme.ts
import { createTheme } from '@mui/material/styles';

const colors = {
  darkGreen: '#1a3c2a',
  midGreen: '#2d5a3d',
  lightGreen: '#4a7c5c',
  mintGreen: '#7aab8a',
  cream: '#f5f0e8',
  white: '#ffffff',
  gold: '#c9a84c',
  darkGold: '#a8892e',
};

export const theme = createTheme({
  palette: {
    primary: {
      main: colors.darkGreen,
      light: colors.midGreen,
      dark: '#0f2a1a',
      contrastText: colors.white,  // ← Text on primary backgrounds
    },
    secondary: {
      main: colors.gold,
      light: colors.mintGreen,
      dark: colors.darkGold,
      contrastText: colors.white,
    },
    background: {
      default: colors.cream,
      paper: colors.white,
    },
    text: {
      primary: colors.darkGreen,    // ← Dark green text on light backgrounds
      secondary: colors.midGreen,   // ← Mid green text on light backgrounds
      disabled: 'rgba(26, 60, 42, 0.5)',
    },
  },
  typography: {
    fontFamily: '"Georgia", "Times New Roman", serif',
    // ✅ Only set text colors for specific variants
    h1: {
      fontWeight: 700,
      color: colors.darkGreen,  // ← Only dark text
    },
    h2: {
      fontWeight: 600,
      color: colors.darkGreen,
    },
    h3: {
      fontWeight: 600,
      color: colors.darkGreen,
    },
    h4: {
      fontWeight: 600,
      color: colors.darkGreen,
    },
    h5: {
      fontWeight: 600,
      color: colors.darkGreen,
    },
    h6: {
      fontWeight: 500,
      color: colors.darkGreen,
    },
    body1: {
      color: colors.darkGreen,  // ← Dark text
    },
    body2: {
      color: colors.midGreen,   // ← Lighter but still visible
    },
  },
  components: {
    // ✅ DialogTitle - Light text on dark background
    MuiDialogTitle: {
      styleOverrides: {
        root: {
          backgroundColor: colors.darkGreen,
          color: colors.white,  // ← White text on green
          '& .MuiTypography-root': {
            color: colors.white,  // ← Force white for typography inside
          },
        },
      },
    },
    // ✅ AppBar - Light text on dark background
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundColor: colors.darkGreen,
          backgroundImage: `linear-gradient(90deg, ${colors.darkGreen}, ${colors.midGreen})`,
          color: colors.white,
          '& .MuiTypography-root': {
            color: colors.white,
          },
          '& .MuiButton-root': {
            color: colors.white,
          },
        },
      },
    },
    // ✅ Cards - Dark text on light background
    MuiCard: {
      styleOverrides: {
        root: {
          backgroundColor: colors.white,
          borderRadius: 16,
          '& .MuiTypography-root': {
            color: colors.darkGreen,  // ← Dark text on white card
          },
          '& .MuiTypography-body2': {
            color: colors.midGreen,
          },
        },
      },
    },
    // ✅ Buttons - Text color based on variant
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8,
          textTransform: 'none',
        },
        containedPrimary: {
          backgroundColor: colors.darkGreen,
          color: colors.white,
          '&:hover': {
            backgroundColor: colors.midGreen,
          },
        },
        containedSecondary: {
          backgroundColor: colors.gold,
          color: colors.white,
          '&:hover': {
            backgroundColor: colors.darkGold,
          },
        },
        outlinedPrimary: {
          borderColor: colors.darkGreen,
          color: colors.darkGreen,
          '&:hover': {
            backgroundColor: colors.darkGreen,
            color: colors.white,
          },
        },
      },
    },
  },
});

export default theme;