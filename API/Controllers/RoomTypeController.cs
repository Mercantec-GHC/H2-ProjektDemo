using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Service;
using DomainModels;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _connectionString;

        public RoomTypeController(AppConfiguration config)
        {
            _connectionString = config.ConnectionString;
            _accessKey = config.AccessKey;
            _secretKey = config.SecretKey;
        }

        [HttpPost("UploadPicture")]
        public async Task<IActionResult> UploadPicture(IFormFile file, int roomTypeId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was provided.");
            }
            if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only image files are allowed.");
            }
            if (file.Length > 10485760)
            {
                return BadRequest("The file size must be less than 10MB.");
            }

            var r2Service = new R2Service(_accessKey, _secretKey);
            var imageUrl = await r2Service.UploadToR2(file.OpenReadStream(), file.FileName);

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"UPDATE roomtypes 
                               SET roompictures = array_append(roompictures, @ImageUrl) 
                               WHERE Id = @roomTypeId;";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("ImageUrl", imageUrl);
                    command.Parameters.AddWithValue("roomTypeId", roomTypeId);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { message = $"Room picture updated successfully at URL: {imageUrl}" });
        }


    }
}
