import {createBrowserRouter, Navigate} from "react-router";
import App from "../layout/App";
import HomePage from "../../../feature/home/HomePage";
import Tours from "../../../feature/tours/dashboard/Tours";
import AboutPage from "../../../feature/about/AboutPage";
import TestErrors from "../../../feature/errors/TestError";
import NotFound from "../../../feature/errors/NotFound";
import ServerError from "../../../feature/errors/ServerError";
import {TourDetails} from "../../../feature/tours/details/TourDetails";
import { BookingConfirmation } from "../../../feature/bookings/BookingConfirmation";
import { MyBooking } from "../../../feature/bookings/MyBooking"


// router.tsx
export const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      { index: true, element: <HomePage /> },  // ✅ Use index: true for the default route
      { path: 'tours', element: <Tours /> },
      { path: 'tours/:id', element: <TourDetails /> },  // ← Add this
      { path: 'booking-confirmation', element: <BookingConfirmation /> },
      { path: 'my-booking', element: <MyBooking /> },
      { path: 'about', element: <AboutPage /> },
      { path: 'errors', element: <TestErrors /> },
      { path: 'not-found', element: <NotFound /> },
      { path: 'server-error', element: <ServerError /> },
      { path: '*', element: <Navigate replace to='/not-found' /> }
    ]
  }
]);