﻿using System.Security.Cryptography;

namespace RecruitXpress_BE.Helper
{
    public class TokenHelper
    {
        public static string GenerateRandomToken(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);

                // Convert the random bytes to a string (for example, in base64 format)
                return Convert.ToBase64String(bytes);
            }
        }
        public static string GenerateNumericToken(int length)
        {
            Random random = new Random();
            const string characters = "0123456789";
            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GenerateRandomToken()
        {
            var random = new Byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
    }
}
