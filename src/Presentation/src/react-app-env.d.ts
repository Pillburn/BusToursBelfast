// <reference types="react-scripts" />

// Optional: Add custom env vars
declare namespace NodeJS {
  interface ProcessEnv {
    REACT_APP_API_URL: string;
    // Add other vars here as needed
  }
}