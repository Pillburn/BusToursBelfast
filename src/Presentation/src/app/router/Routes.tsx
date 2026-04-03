import {createBrowserRouter, Navigate} from "react-router";
import App from "../layout/App";
import HomePage from "../../../feature/home/HomePage";
import Tours from "../../../feature/tours/dashboard/Tours";
import AboutPage from "../../../feature/about/AboutPage";
import Counter from "../../../feature/counter/Counter";
import TestErrors from "../../../feature/errors/TestError";
import NotFound from "../../../feature/errors/NotFound";
import ServerError from "../../../feature/errors/ServerError";


export const router = createBrowserRouter([
    {
        path:'/',
        element: <App/>,
        children: [
            
            {path: 'tours', element: <Tours /> },
            //{path: 'tours/:id', element: <TourDetailsPage/>},//(the :id is a placeholder for the url link to the specific id)
            //{path: 'createTour', element: <TourForm key='create'/> },
            //{path: 'manage/:id', element:<TourForm/> },
           
            {path: '', element: <HomePage/> },
            {path: 'counter', element:<Counter/> },
            {path: 'errors', element: <TestErrors/>},
            {path: 'not-found', element:<NotFound/>},
            {path: 'server-error', element:<ServerError/>},
            {path: 'about', element: <AboutPage/>},
            {path: '*', element: <Navigate replace to='/not-found'/>}
        ]
    }
])