
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendResetPasswordEmail(string toEmail, string tempPassword);
    }
}

