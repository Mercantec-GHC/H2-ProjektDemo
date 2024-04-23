using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public class TokenGenerationService
{
    public static string GenerateToken(string email)
    {
        var secretKey = "your_secret_key_here_must_be_at_least_16_bytes_long"; 
        var issuer = "MAGS-Template"; 
        var audience = "H2-ProjektDemo";
        var expireMinutes = 30*2*24;

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            // Add other claims as needed
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        var finalToken = tokenHandler.WriteToken(token);

        return finalToken;
    }
}
