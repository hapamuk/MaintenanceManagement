using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Services
{
    public class SmtpPasswordResetEmailSender
        : IPasswordResetEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpPasswordResetEmailSender(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(
            string email,
            string resetUrl,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(resetUrl);

            cancellationToken.ThrowIfCancellationRequested();

            string host =
                _configuration["Smtp:Host"]
                ?? throw new InvalidOperationException(
                    "SMTP sunucusu tanımlanmamış.");

            string from =
                _configuration["Smtp:From"]
                ?? throw new InvalidOperationException(
                    "SMTP gönderici adresi tanımlanmamış.");

            string username =
                _configuration["Smtp:Username"]
                ?? throw new InvalidOperationException(
                    "SMTP kullanıcı adı tanımlanmamış.");

            string password =
                _configuration["Smtp:Password"]
                ?? throw new InvalidOperationException(
                    "SMTP şifresi tanımlanmamış.");

            if (!int.TryParse(
                _configuration["Smtp:Port"],
                out int port))
            {
                throw new InvalidOperationException(
                    "SMTP port bilgisi geçersiz.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = "Şifre Sıfırlama Talebi",
                Body =
                    "Demirbaş ve Bakım Yönetim Sistemi\n\n" +
                    "Şifrenizi yenilemek için aşağıdaki " +
                    "bağlantıyı kullanınız:\n\n" +
                    resetUrl + "\n\n" +
                    "Bu bağlantı 30 dakika geçerlidir.\n\n" +
                    "Bu işlemi siz talep etmediyseniz " +
                    "e-postayı dikkate almayınız.",
                IsBodyHtml = false
            };

            message.To.Add(email);

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    username,
                    password)
            };

            await client.SendMailAsync(message);

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
