using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.LowLevelValidators
{
    public class PasswordValidator
    {
        private readonly string _password;
        public PasswordValidator(string password)
        {
            _password = password;
        }

        public bool IsNullOrWhiteSpace() => string.IsNullOrWhiteSpace(_password);
        public bool IsEnoughCharacters() => _password.Length >= 8;
        public bool ContainsDigits() => _password.Any(char.IsDigit);
        public bool IsValid() => !IsNullOrWhiteSpace() && ContainsDigits() && IsEnoughCharacters();
    }
}
