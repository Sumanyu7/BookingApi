using BookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingContext _context;

        public BookingService(BookingContext context)
        {
            _context = context;
        }

        public async Task<(BookingResult, Guid?)> CreateAsync(string name, string startTime)
        {
            if (!IsValidTime(startTime) || string.IsNullOrEmpty(name))
            {
                return (BookingResult.InvalidData, null);
            }

            if (IsFullyBooked(startTime))
            {
                return (BookingResult.FullyBooked, null);
            }
            
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                Name = name,
                StartTime = TimeSpan.Parse(startTime),
                EndTime = TimeSpan.Parse(startTime).Add(TimeSpan.FromMinutes(59))
            };

            if (BookingExists(booking))
            {
                return (BookingResult.AlreadyExists, null);
            }

            _context.Entry(booking).State = EntityState.Added;
            await _context.SaveChangesAsync();

            return (BookingResult.Success, booking.Id);
        }
        
        private static bool IsValidTime(string time)
        {
            if (!TimeSpan.TryParse(time, out var bookingTime))
            {
                return false;
            }
            var businessStart = new TimeSpan(9, 0, 0);
            var businessEnd = new TimeSpan(16, 0, 0);
            return bookingTime >= businessStart && bookingTime <= businessEnd;
        }

        private bool IsFullyBooked(string time)
        {
            var periodStart = TimeSpan.Parse(time);
            var periodEnd = periodStart.Add(TimeSpan.FromMinutes(59));
            return _context.Bookings.Count(b => 
                b.StartTime <= periodEnd && 
                b.EndTime >= periodStart
            ) >= 4;
        }

        private bool BookingExists(Booking booking)
        {
            return _context.Bookings.Any(b => b.Name == booking.Name && b.StartTime == booking.StartTime);
        }
    }

    public interface IBookingService
    {
        Task<(BookingResult, Guid?)> CreateAsync(string name, string startTime);
    }
    
    public enum BookingResult
    {
        Success,
        InvalidData,
        FullyBooked,
        AlreadyExists
    }
}