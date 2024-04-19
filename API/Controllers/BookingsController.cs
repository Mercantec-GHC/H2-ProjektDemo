using DomainModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly string _connectionString;

        public BookingsController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = new List<Booking>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM Bookings", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var booking = new Booking
                        {
                            Id = reader.GetInt32(0),
                            CheckInDate = reader.GetDateTime(1),
                            CheckOutDate = reader.GetDateTime(2),
                            UserId = reader.GetInt32(3),
                            RoomId = reader.GetInt32(4),
                            CreatedAt = reader.GetDateTime(5),
                            UpdatedAt = reader.GetDateTime(6)
                        };

                        bookings.Add(booking);
                    }
                }
            }

            return Ok(bookings);
        }

    }
}
