// lib/api/config.ts
export const API_BASE_URL = 'http://localhost:5080';

console.log('🔍 API_BASE_URL:', API_BASE_URL);

export const API_ENDPOINTS = {
    health: `${API_BASE_URL}/api/health`,
    tours: `${API_BASE_URL}/api/tour`,
    bookings: `${API_BASE_URL}/api/tour/bookings`,
    tourFirst: `${API_BASE_URL}/api/tour/first`,
    tourSeed: `${API_BASE_URL}/api/tour/seed`,
};