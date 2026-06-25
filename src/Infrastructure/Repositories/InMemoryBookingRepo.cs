// Infrastructure/Repositories/InMemoryBookingRepo.cs
using System.Collections.Concurrent;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Interfaces;

namespace ToursApp.Infrastructure.Repositories
{
    public class InMemoryBookingRepo : IBookingRepository
    {
        private readonly List<Booking> _bookings = new();
        private readonly object _lock = new();

        // ✅ Create Booking
        public Task AddBookingAsync(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            lock (_lock)
            {
                if (booking.Id == Guid.Empty)
                    booking.Id = Guid.NewGuid();
                
                _bookings.Add(booking);
            }
            return Task.CompletedTask;
        }

        // ✅ Create Booking (alternative name)
        public Task CreateBookingAsync(Booking booking)
        {
            return AddBookingAsync(booking);
        }

        // ✅ Get by ID
        public Task<Booking?> GetBookingByIdAsync(Guid id)
        {
            lock (_lock)
            {
                var booking = _bookings.FirstOrDefault(b => b.Id == id);
                return Task.FromResult(booking);
            }
        }

        // ✅ Get by Tour ID
        public Task<IEnumerable<Booking>> GetBookingByTourIdAsync(Guid tourId)
        {
            lock (_lock)
            {
                var bookings = _bookings.Where(b => b.TourId == tourId).ToList();
                return Task.FromResult(bookings.AsEnumerable());
            }
        }

        // ✅ Get by Tour ID (alternative name)
        public Task<IEnumerable<Booking>> GetBookingsByTourIdAsync(Guid tourId)
        {
            return GetBookingByTourIdAsync(tourId);
        }

        // ✅ Get by Email
        public Task<IEnumerable<Booking>> GetUserBookingsAsync(string email)
        {
            lock (_lock)
            {
                var bookings = _bookings.Where(b => b.Email == email).ToList();
                return Task.FromResult(bookings.AsEnumerable());
            }
        }

        // ✅ Get by Reference
        public Task<Booking?> GetBookingByReferenceAsync(string referenceNumber)
        {
            lock (_lock)
            {
                var booking = _bookings.FirstOrDefault(b => b.ReferenceNumber == referenceNumber);
                return Task.FromResult(booking);
            }
        }

        // ✅ Get by Reference and Email
        public Task<Booking?> GetBookingByReferenceAndEmailAsync(string referenceNumber, string email)
        {
            lock (_lock)
            {
                var booking = _bookings.FirstOrDefault(b => 
                    b.ReferenceNumber == referenceNumber && b.Email == email);
                return Task.FromResult(booking);
            }
        }

        // ✅ Get by Payment Intent ID
        public Task<Booking?> GetBookingByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var booking = _bookings.FirstOrDefault(b => b.PaymentIntentId == paymentIntentId);
                return Task.FromResult(booking);
            }
        }

        // ✅ Get by Tour and Date
        public Task<IEnumerable<Booking>> GetBookingsByTourAndDateAsync(Guid tourId, DateOnly date)
        {
            lock (_lock)
            {
                var bookings = _bookings.Where(b => 
                    b.TourId == tourId && b.PreferredDate == date).ToList();
                return Task.FromResult(bookings.AsEnumerable());
            }
        }

        // ✅ Update Booking
        public Task UpdateBookingAsync(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            lock (_lock)
            {
                var index = _bookings.FindIndex(b => b.Id == booking.Id);
                if (index != -1)
                {
                    _bookings[index] = booking;
                }
            }
            return Task.CompletedTask;
        }

        // ✅ Delete Booking
        public Task DeleteBookingAsync(Guid id)
        {
            lock (_lock)
            {
                var booking = _bookings.FirstOrDefault(b => b.Id == id);
                if (booking != null)
                {
                    _bookings.Remove(booking);
                }
            }
            return Task.CompletedTask;
        }

        // ✅ Check if Booking Exists
        public Task<bool> BookingExistsAsync(Guid id)
        {
            lock (_lock)
            {
                var exists = _bookings.Any(b => b.Id == id);
                return Task.FromResult(exists);
            }
        }

        // ✅ Check if Booking Exists by Reference
        public Task<bool> BookingExistsByReferenceAsync(string referenceNumber)
        {
            lock (_lock)
            {
                var exists = _bookings.Any(b => b.ReferenceNumber == referenceNumber);
                return Task.FromResult(exists);
            }
        }

        // ✅ Save Changes (for in-memory, just return success)
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }

        // ✅ Get All Bookings (optional, for admin)
        public Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_bookings.AsEnumerable());
            }
        }
    }
}