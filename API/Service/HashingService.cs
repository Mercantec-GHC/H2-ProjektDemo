using System.Security.Cryptography;
using System.Text;

namespace API.Service
{
    public class HashingService
    {
        private const int SaltSize = 16;

        public static string HashPassword(string password)
        {
            byte[] salt = GenerateSalt();

            string hashedPassword = HashPasswordWithSalt(password, salt);

            return Convert.ToBase64String(salt) + hashedPassword;
        }

        public static bool VerifyPassword(string hashedPasswordWithSalt, string password)
        {
            byte[] salt = Convert.FromBase64String(hashedPasswordWithSalt.Substring(0, 24));

            string hashedPassword = HashPasswordWithSalt(password, salt);

            return hashedPasswordWithSalt.Substring(24) == hashedPassword;
        }

        private static string HashPasswordWithSalt(string password, byte[] salt)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                return ByteArrayToHexString(hashedBytes);
            }
        }

        private static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);
                return salt;
            }
        }

        private static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }
    }
}
