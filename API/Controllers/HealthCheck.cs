using DomainModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Threading.Tasks;
using API.Service;
using Microsoft.Extensions.Options;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheck : ControllerBase
    {
        private readonly string _connectionString;

        public HealthCheck(AppConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        [HttpGet]
        public async Task<ActionResult<bool>> GetHealthCheck()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(@"SELECT * FROM Users
                                                         ORDER BY id
                                                         LIMIT 1", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    
                    bool userExists = await reader.ReadAsync();
                    
                    reader.Close();
                    connection.Close();

                    return userExists ? Ok(userExists) : BadRequest();
                }
            }
        }
    }
}
