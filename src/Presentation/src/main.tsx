import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter, Routes, Route } from 'react-router-dom' // Import BrowserRouter
import App from './App'
import './index.css'
import { StoreProvider } from '../../Api/stores/StoreContext';
import { Tours } from "./../feature/tours/dashboard/Tours";


ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <StoreProvider>
    <BrowserRouter> 
      <Routes>  {/* ← Add Routes */}
          <Route path="/" element={<App />}>  {/* ← App as parent route */}
            <Route path="tours" element={<Tours />} />  {/* ← Tours as child route */}
            {/* Add other routes here that should show with Navbar */}
          </Route>
      </Routes>
    </BrowserRouter>
    </StoreProvider>
  </React.StrictMode>
)
