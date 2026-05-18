using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyPhongKham.Services.Implementations
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private static readonly string chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";

        public  string Generate(int length = 10)
        {
            var data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var result = new StringBuilder(length);

            foreach (var b in data)
            {
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }
        private static readonly string numbers = "0123456789";

        public string GenerateNumericOtp(int length = 6)
        {
            var data = new byte[length];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var result = new System.Text.StringBuilder(length);
            foreach (var b in data)
            {
                result.Append(numbers[b % numbers.Length]);
            }
            return result.ToString();
        }
    }
}
