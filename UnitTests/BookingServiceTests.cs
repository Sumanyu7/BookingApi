using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookingApi.Services;
using BookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.UnitTests
{
    [TestClass]
    public class BookingServiceTests
    {
        private BookingService _service;
        private BookingContext _context;

        [TestInitialize]
        public void Setup()
        {
           var options = new DbContextOptionsBuilder<BookingContext>()
                .UseInMemoryDatabase(databaseName: "BookingDb")
                .Options;
            _context = new BookingContext(options);
            _service = new BookingService(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_ReturnsSuccess()
        { 
            var (result, bookingId) = await _service.CreateAsync("John Doe", "10:00");
            
            Assert.AreEqual(BookingResult.Success, result);
            Assert.IsNotNull(bookingId);
        }

        [TestMethod]
        public async Task CreateAsync_FullyBooked_ReturnsFullyBooked()
        {
            const string startTime = "10:00";
            // Pre-populate the database to simulate a fully booked slot
            for (var i = 0; i < 4; i++)
            {
                _context.Bookings.Add(new Booking { Id = Guid.NewGuid(), Name = $"User{i}", StartTime = TimeSpan.Parse(startTime), EndTime = TimeSpan.Parse(startTime).Add(TimeSpan.FromMinutes(59)) });
            }
            await _context.SaveChangesAsync();
            
            var (result, bookingId) = await _service.CreateAsync("John Doe", startTime);
            
            Assert.AreEqual(BookingResult.FullyBooked, result);
            Assert.IsNull(bookingId);
        }
        
        [TestMethod]
        public async Task CreateAsync_FullyBookedDifferentTimes_ReturnsFullyBooked()
        {
            const string startTime = "10:00";
            // Pre-populate the database with 10 min increments to simulate a fully booked slot
            for (var i = 0; i < 4; i++)
            {
                _context.Bookings.Add(new Booking { Id = Guid.NewGuid(), Name = $"User{i}", StartTime = TimeSpan.Parse(startTime).Add(TimeSpan.FromMinutes(i*10)), EndTime = TimeSpan.Parse(startTime).Add(TimeSpan.FromMinutes(i*10 + 59)) });
            }
            await _context.SaveChangesAsync();
            
            var (result, bookingId) = await _service.CreateAsync("John Doe", startTime);
            
            Assert.AreEqual(BookingResult.FullyBooked, result);
            Assert.IsNull(bookingId);
        }

        [TestMethod]
        public async Task CreateAsync_AlreadyExists_ReturnsAlreadyExists()
        {
            const string name = "John Doe";
            const string startTime = "09:00";
            _context.Bookings.Add(new Booking { Id = Guid.NewGuid(), Name = name, StartTime = TimeSpan.Parse(startTime), EndTime = TimeSpan.Parse(startTime).Add(TimeSpan.FromMinutes(59)) });
            await _context.SaveChangesAsync();
            
            var (result, bookingId) = await _service.CreateAsync(name, startTime);

            Assert.AreEqual(BookingResult.AlreadyExists, result);
            Assert.IsNull(bookingId);
        }

        
        [TestMethod]
        public async Task CreateAsync_InvalidTime_ReturnsInvalidData()
        {
            const string name = "John Doe";
            const string startTime = "18:00";  // Outside of business hours
            
            var (result, bookingId) = await _service.CreateAsync(name, startTime);
            
            Assert.AreEqual(BookingResult.InvalidData, result);
            Assert.IsNull(bookingId);
        }
        
        [TestMethod]
        public async Task CreateAsync_EmptyName_ReturnsInvalidData()
        {
            const string  name = "";
            const string startTime = "12:00";
            
            var (result, bookingId) = await _service.CreateAsync(name, startTime);

            Assert.AreEqual(BookingResult.InvalidData, result);
            Assert.IsNull(bookingId);
        }
    }
}

