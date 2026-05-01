using Microsoft.Extensions.Configuration;
using QuanLyPhongKham.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendResetPasswordEmail(string toEmail, string tempPassword)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(
            _configuration["SendGrid:FromEmail"],
            _configuration["SendGrid:FromName"]
        );

        var to = new EmailAddress(toEmail);

        var subject = "Reset mật khẩu";

        var plainText = $"Mật khẩu tạm thời của bạn là: {tempPassword}";

        var html = $@"
            <h2>Reset mật khẩu</h2>
            <p>Mật khẩu tạm thời của bạn là:</p>
            <strong>{tempPassword}</strong>
            <p>Vui lòng đổi mật khẩu sau khi đăng nhập.</p>
        ";

        var msg = MailHelper.CreateSingleEmail(
            from,
            to,
            subject,
            plainText,
            html
        );

        var response = await client.SendEmailAsync(msg);

        Console.WriteLine(response.StatusCode);

        var body = await response.Body.ReadAsStringAsync();
        Console.WriteLine(body);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Gửi email thất bại");
        }
    }
}