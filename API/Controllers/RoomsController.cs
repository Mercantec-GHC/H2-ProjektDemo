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
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms()
        {
            var rooms = new List<RoomDTO>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"SELECT r.Id, r.RoomNumber, r.CreatedAt, r.UpdatedAt, rt.RoomName, rt.PricePerDay, rt.NumberOfBeds 
                               FROM Rooms r 
                               INNER JOIN roomtypes rt 
                               ON r.RoomTypeId = rt.Id";

                using (var command = new NpgsqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var roomDetail = new RoomDTO
                        {
                            Id = reader.GetInt32(0),
                            RoomNumber = reader.GetString(1),
                            CreatedAt = reader.GetDateTime(2),
                            UpdatedAt = reader.GetDateTime(3),
                            RoomName = reader.GetString(4),
                            PricePerDay = reader.GetFloat(5),
                            NumberOfBeds = reader.GetInt32(6)
                        };

                        rooms.Add(roomDetail);
                    }
                }
            }

            return Ok(rooms);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDTO>> GetRoomById(int id)
        {
            RoomDTO roomDetail = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"SELECT r.Id, r.RoomNumber, r.CreatedAt, r.UpdatedAt, rt.RoomName, rt.PricePerDay, rt.NumberOfBeds 
                               FROM Rooms r 
                               INNER JOIN roomtypes rt 
                               ON r.RoomTypeId = rt.Id
                               WHERE r.Id = @Id";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            roomDetail = new RoomDTO
                            {
                                Id = reader.GetInt32(0),
                                RoomNumber = reader.GetString(1),
                                CreatedAt = reader.GetDateTime(2),
                                UpdatedAt = reader.GetDateTime(3),
                                RoomName = reader.GetString(4),
                                PricePerDay = reader.GetFloat(5),
                                NumberOfBeds = reader.GetInt32(6)
                            };
                        }
                    }
                }
            }

            if (roomDetail == null)
            {
                return NotFound();
            }

            return Ok(roomDetail);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAvailableRooms(int numberOfPeople, DateTime startDate, DateTime endDate)
        {
            List<RoomDTO> allRooms = new List<RoomDTO>();
            List<RoomDTO> availableRooms = new List<RoomDTO>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sqlAllRooms = @"
                    SELECT r.id, r.RoomNumber, rt.RoomName, rt.PricePerDay, rt.NumberOfBeds
                    FROM Rooms r
                    INNER JOIN roomtypes rt ON r.RoomTypeId = rt.Id;";

                using (var command = new NpgsqlCommand(sqlAllRooms, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var roomDetail = new RoomDTO
                            {
                                Id = reader.GetInt32(0),
                                RoomNumber = reader.GetString(1),
                                RoomName = reader.GetString(2),
                                PricePerDay = reader.GetFloat(3),
                                NumberOfBeds = reader.GetInt32(4)
                            };

                            allRooms.Add(roomDetail);
                        }
                    }
                }

                foreach (var room in allRooms)
                {
                    string sqlCheckAvailability = @"
                        SELECT COUNT(*)
                        FROM bookings b
                        WHERE b.RoomId = @RoomId
                        AND (b.CheckInDate, b.CheckOutDate) OVERLAPS (@StartDate, @EndDate);";

                    using (var command = new NpgsqlCommand(sqlCheckAvailability, connection))
                    {
                        command.Parameters.AddWithValue("RoomId", room.Id);
                        command.Parameters.AddWithValue("StartDate", startDate);
                        command.Parameters.AddWithValue("EndDate", endDate);

                        long bookingCount = (long)await command.ExecuteScalarAsync();

                        if (bookingCount == 0 && room.NumberOfBeds >= numberOfPeople)
                        {
                            availableRooms.Add(room);
                        }
                    }
                }
            }

            return Ok(availableRooms);
        }

    }
}
