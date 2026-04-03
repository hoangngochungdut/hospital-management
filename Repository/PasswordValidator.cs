using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Validators
{
    public class PasswordValidator
    {
        private readonly string _password;
        PasswordValidator(string password)
        {
            _password = password;
        }

        public bool IsEnoughCharacters() => _password.Length >= 8;
        public bool ContainsNumbers() => _password.Any(char.IsDigit);

    }
}
