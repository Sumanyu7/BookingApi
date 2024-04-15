# BookingApi

A booking API that will accept a booking time and respond indicating
whether the reservation was successful or not.

### Technical requirements
- Assume that all bookings are for the same day (do not worry about handling dates)
- InfoTrack's hours of business are 9am-5pm, all bookings must complete by 5pm (latest booking
is 4:00pm)
- Bookings are for 1 hour (booking at 9:00am means the spot is held from 9:00am to 9:59am)
- InfoTrack accepts up to 4 simultaneous settlements
- API needs to accept POST requests of the following format:
{
"bookingTime": "09:30",
"name":"John Smith"
}
- Successful bookings should respond with an OK status and a booking Id in GUID form
{
"bookingId": "d90f8c55-90a5-4537-a99d-c68242a6012b"
}
- Requests for out of hours times should return a Bad Request status
- Requests with invalid data should return a Bad Request status
- Requests when all settlements at a booking time are reserved should return a Conflict status
- The name property should be a non-empty string
- The bookingTime property should be a 24-hour time (00:00 - 23:59)
- Bookings can be stored in-memory, it is fine for them to be forgotten when the application is
restarted

### Running the API
 - Clone this repo to your machine
- Run dotnet restore to install the required packages
- Run dotnet run to run the app in development mode
- Open http://localhost:5087/swagger/index.html to view Swagger UI to test API in your browser.
- Can also test it in Postman by making POST request to http://localhost:5087/Booking with request body as {
  "bookingTime": "string",
  "name": "string"
  }  