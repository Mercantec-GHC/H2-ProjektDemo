using DomainModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly string _connectionString;

        public RoomsController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            var rooms = new List<Room>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM Rooms", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var room = new Room
                        {
                            Id = reader.GetInt32(0),
                            RoomNumber = reader.GetString(1),
                            RoomSize = reader.GetString(2),
                            RoomType = reader.GetString(3),
                            IsAvailable = reader.GetBoolean(4),
                            CreatedAt = reader.GetDateTime(5),
                            UpdatedAt = reader.GetDateTime(6)
                        };

                        rooms.Add(room);
                    }
                }
            }

            return Ok(rooms);
        }

    }
}
