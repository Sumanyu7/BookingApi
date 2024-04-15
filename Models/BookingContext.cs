using Microsoft.EntityFrameworkCore;

namespace BookingApi.Models
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; } = null!;
    }
}