using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

namespace BlazorWA.Service
{
    public class TokenService
    {
        public string? Token { get; set; }

        public static string GetUserEmailFromToken(string jwtToken)
        {
            // Split the JWT token into its components
            string[] jwtEncodedSegments = jwtToken.Split('.');
            if (jwtEncodedSegments.Length < 2)
            {
                throw new ArgumentException("Invalid JWT token format.");
            }

            // Decode the payload segment
            var payloadSegment = jwtEncodedSegments[1];
            // Ensure the payload segment is correctly padded
            payloadSegment = payloadSegment.PadRight(payloadSegment.Length + (4 - payloadSegment.Length % 4) % 4, '=');
            var decodePayload = Convert.FromBase64String(payloadSegment);
            var decodedUtf8Payload = Encoding.UTF8.GetString(decodePayload);

            // Parse the payload as JSON
            var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(decodedUtf8Payload);

            // Extract the email claim
            if (result.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", out var emailClaim))
            {
                return emailClaim.GetString();
            }

            return null;
        }
    }
}
