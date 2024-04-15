using Microsoft.AspNetCore.Mvc;
using BookingApi.Models;
using BookingApi.Services;

namespace BookingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync([FromBody] BookingRequest request)
        {
            var (result, bookingId) = await _bookingService.CreateAsync(request.Name, request.BookingTime);
        
            switch (result)
            {
                case BookingResult.Success:
                    return Ok(new BookingResponse { BookingId = bookingId.Value });
                case BookingResult.FullyBooked:
                    return Conflict("All meetings are booked for this time.");
                case BookingResult.AlreadyExists:
                    return Conflict("Booking already exists");
                case BookingResult.InvalidData:
                default:
                    return BadRequest("Invalid booking request.");
            }
        }
    }
}