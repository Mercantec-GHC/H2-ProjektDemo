using DomainModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Service;
using System.Text;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly string _connectionString;

        public UsersController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = new List<User>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM Users", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new User
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            PhoneNumber = reader.GetString(4),
                            CreatedAt = reader.GetDateTime(6),
                            UpdatedAt = reader.GetDateTime(7)
                        };

                        users.Add(user);
                    }
                }
            }

            return Ok(users);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.PasswordBackdoor = user.HashedPassword;
            user.HashedPassword = HashingService.HashPassword(user.HashedPassword);

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, HashedPassword, PasswordBackdoor, CreatedAt, UpdatedAt) VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @HashedPassword, @PasswordBackdoor, @CreatedAt, @UpdatedAt) RETURNING Id", connection))
                {
                    command.Parameters.AddWithValue("FirstName", user.FirstName);
                    command.Parameters.AddWithValue("LastName", user.LastName);
                    command.Parameters.AddWithValue("Email", user.Email);
                    command.Parameters.AddWithValue("PhoneNumber", user.PhoneNumber);
                    command.Parameters.AddWithValue("HashedPassword", user.HashedPassword);
                    command.Parameters.AddWithValue("PasswordBackdoor", user.PasswordBackdoor);
                    command.Parameters.AddWithValue("CreatedAt", user.CreatedAt);
                    command.Parameters.AddWithValue("UpdatedAt", user.UpdatedAt);

                    var id = (int)await command.ExecuteScalarAsync();
                    user.Id = id;
                }
            }

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user.Id);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            user.UpdatedAt = DateTime.Now;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = new StringBuilder("UPDATE Users SET ");
                var parameters = new List<NpgsqlParameter>();

                if (!string.IsNullOrEmpty(user.FirstName))
                {
                    commandText.Append("FirstName = @FirstName, ");
                    parameters.Add(new NpgsqlParameter("FirstName", user.FirstName));
                }

                if (!string.IsNullOrEmpty(user.LastName))
                {
                    commandText.Append("LastName = @LastName, ");
                    parameters.Add(new NpgsqlParameter("LastName", user.LastName));
                }

                if (!string.IsNullOrEmpty(user.Email))
                {
                    commandText.Append("Email = @Email, ");
                    parameters.Add(new NpgsqlParameter("Email", user.Email));
                }

                if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    commandText.Append("PhoneNumber = @PhoneNumber, ");
                    parameters.Add(new NpgsqlParameter("PhoneNumber", user.PhoneNumber));
                }

                commandText.Append("UpdatedAt = @UpdatedAt ");
                parameters.Add(new NpgsqlParameter("UpdatedAt", user.UpdatedAt));

                commandText.Append("WHERE Id = @Id");
                parameters.Add(new NpgsqlParameter("Id", user.Id));

                using (var command = new NpgsqlCommand(commandText.ToString(), connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("DELETE FROM Users WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("Id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login login)
        {
            if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Username and password are required.");
            }

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Check if the email exists in the database
                using (var command = new NpgsqlCommand("SELECT EXISTS(SELECT 1 FROM Users WHERE Email = @Email)", connection))
                {
                    command.Parameters.AddWithValue("Email", login.Email);

                    bool emailExists = (bool)await command.ExecuteScalarAsync();
                    if (!emailExists)
                    {
                        return NotFound("User not found.");
                    }
                }

                using (var command = new NpgsqlCommand("SELECT HashedPassword FROM Users WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("Email", login.Email);

                    var hashedPassword = await command.ExecuteScalarAsync();
                    if (hashedPassword == null)
                    {
                        return NotFound("User not found.");
                    }

                    // Assuming you have a method to verify the password
                    bool isPasswordValid = HashingService.VerifyPassword(hashedPassword.ToString(), login.Password);
                    if (!isPasswordValid)
                    {
                        return Unauthorized("Invalid username or password.");
                    }

                    // Assuming you have a method to generate a token
                    var token = TokenGenerationService.GenerateToken(login.Email);
                    return Ok(new { Token = token });
                }
            }
        }

    }
}
