import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { agent } from "../api/agent";
import { FieldValues } from "react-hook-form";
import { useLocation } from "react-router";
import { Tour } from "../../src/types/tour";

export const useTours = (id?: string) => {

    const queryClient = useQueryClient();
    const location = useLocation();

    const {data: tours, isLoading} = useQuery({
        queryKey: ['tours'],
        queryFn: async () => {
        const response = await agent.get<Tour[]>('/tour')
        return response.data;
    },
    enabled: !id && location.pathname === '/tours'
    });

    const {data: tour, isLoading: isLoadingTour} = useQuery({
      queryKey: ['tours',id],
      queryFn: async () => {
        const response = await agent.get<Tour>(`/tours/${id}`)
        return response.data;
      },
      enabled:!!id
    });

  const updateTour = useMutation({
    mutationFn: async (tour:Tour) => {
        await agent.put('/tours',tour)
    },
    onSuccess: async () => {
        await queryClient.invalidateQueries({
            queryKey: ['tours']
        })
    }
  })

  const createTour = useMutation({
    mutationFn: async (tour:FieldValues) => {
        const response = await agent.post('/tours',tour)
        return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ['tours']
      })
    }
  })

  const deleteTour = useMutation({
    mutationFn: async(id:string) => {
      await agent.delete(`/tours/${id}`)
    },
    onSuccess: async() => {
      await queryClient.invalidateQueries({
        queryKey: ['tours']
      })
    }
  })

  return {
    tours,
    tour,
    isLoadingTour,
    isLoading,
    updateTour,
    createTour,
    deleteTour
  }
}