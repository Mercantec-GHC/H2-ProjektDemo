using API.Service;
using DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        public BookingsController(AppConfiguration config)
        {
            _connectionString = config.ConnectionString;
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
                            CheckInDate = DateOnly.FromDateTime(reader.GetDateTime(1)),
                            CheckOutDate = DateOnly.FromDateTime(reader.GetDateTime(2)),
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

        //Get active bookings for userId
        [HttpGet("active/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings(int userId)
        {
            var bookings = new List<Booking>();

            string sqlQuery = @"SELECT Id, CheckInDate, CheckOutDate, RoomId FROM Bookings 
                                WHERE UserId = @UserId
                                AND CheckOutDate > CURRENT_DATE";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(sqlQuery, connection))
                {

                    command.Parameters.AddWithValue("@UserId", userId);


                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var booking = new Booking
                            {
                                Id = reader.GetInt32(0),
                                CheckInDate = DateOnly.FromDateTime(reader.GetDateTime(1)),
                                CheckOutDate = DateOnly.FromDateTime(reader.GetDateTime(2)),
                                RoomId = reader.GetInt32(3)
                                
                            };

                            bookings.Add(booking);
                        }
                    }
                }
            }

            return Ok(bookings);
        }

        //Get active bookings for userId
        [HttpGet("past/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBooking(int userId)
        {
            var bookings = new List<Booking>();

            string sqlQuery = @"SELECT Id, CheckInDate, CheckOutDate, RoomId FROM Bookings  
                                WHERE UserId = @UserId
                                AND CheckOutDate < CURRENT_DATE";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(sqlQuery, connection))
                {

                    command.Parameters.AddWithValue("@UserId", userId);


                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var booking = new Booking
                            {
                                Id = reader.GetInt32(0),
                                CheckInDate = DateOnly.FromDateTime(reader.GetDateTime(1)),
                                CheckOutDate = DateOnly.FromDateTime(reader.GetDateTime(2)),
                                RoomId = reader.GetInt32(3)
                            };

                            bookings.Add(booking);
                        }
                    }
                }
            }

            return Ok(bookings);
        }

    }
}
