import {z} from 'zod';

export const bookingSchema = z.object({
    customerEmail: z.string().email(),
    bookingDate: z.coerce.date({
        message: 'Date is required'
    })
})

export type BookingSchema = z.infer<typeof bookingSchema>;