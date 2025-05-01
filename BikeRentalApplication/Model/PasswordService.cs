using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRentalApplication.Model
{
    public class PasswordService
    {
        private static PasswordHasher<object> hasher = new();

        // Хеширование
        public static string HashPassword(string password)
        {
            return hasher.HashPassword(null, password);
        }

        // Проверка
        public static bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = hasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
