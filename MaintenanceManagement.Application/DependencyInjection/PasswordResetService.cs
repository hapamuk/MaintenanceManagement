using MaintenanceManagement.Application.DTOS.UserDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace MaintenanceManagement.Application.DependencyInjection
{
    public class PasswordResetService : IPasswordResetService
    {
        #region Fields

        private readonly IUserRepository _userRepository;

        private readonly IPasswordResetTokenRepository
            _passwordResetTokenRepository;

        private readonly IPasswordResetEmailSender
            _emailSender;

        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor

        public PasswordResetService(
            IUserRepository userRepository,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IPasswordResetEmailSender emailSender,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordResetTokenRepository =
                passwordResetTokenRepository;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        #endregion

        #region Request Password Reset

        /// <summary>
        /// Kullanıcının e-posta adresine güvenli,
        /// süreli şifre sıfırlama bağlantısı gönderir.
        /// </summary>
        public async Task RequestAsync(
            ForgotPasswordUserDto dto,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. DTO KONTROLÜ
            // =====================================

            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ValidationException(
                    "E-posta adresi zorunludur.");
            }

            // =====================================
            // 2. E-POSTA NORMALİZASYONU
            // =====================================

            string email = dto.Email.Trim()
                .ToLowerInvariant();

            // =====================================
            // 3. KULLANICIYI BUL
            // =====================================

            var user = await _userRepository
                .GetByEmailAsync(
                    email,
                    cancellationToken);

            // Kullanıcının bulunup bulunmadığı
            // dışarıya açıklanmaz.
            if (user == null)
                return;

            // =====================================
            // 4. HESAP DURUMU
            // =====================================

            if (!user.IsActive ||
                !user.HasSystemAccess ||
                string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return;
            }

            // =====================================
            // 5. GÜVENLİ TOKEN OLUŞTUR
            // =====================================

            byte[] tokenBytes =
                RandomNumberGenerator.GetBytes(32);

            string token = Convert
                .ToBase64String(tokenBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            // =====================================
            // 6. TOKEN HASH OLUŞTUR
            // =====================================

            string tokenHash = HashToken(token);

            DateTime now = DateTime.UtcNow;

            // =====================================
            // 7. TOKEN KAYDINI HAZIRLA
            // =====================================

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,

                TokenHash = tokenHash,

                CreatedAtUtc = now,

                ExpiresAtUtc = now.AddMinutes(30),

                UsedAtUtc = null
            };

            // =====================================
            // 8. TOKEN HASH'İNİ KAYDET
            // =====================================

            await _passwordResetTokenRepository
                .CreateAsync(
                    resetToken,
                    cancellationToken);

            // =====================================
            // 9. SIFIRLAMA ADRESİNİ AL
            // =====================================

            string resetPageUrl =
                _configuration["PasswordReset:ResetUrl"]
                ?? throw new InvalidOperationException(
                    "Şifre sıfırlama URL ayarı bulunamadı.");

            // =====================================
            // 10. SIFIRLAMA BAĞLANTISI
            // =====================================

            string resetUrl =
                $"{resetPageUrl}" +
                $"?email={Uri.EscapeDataString(email)}" +
                $"&token={Uri.EscapeDataString(token)}";

            // =====================================
            // 11. E-POSTA GÖNDER
            // =====================================

            await _emailSender.SendAsync(
                email,
                resetUrl,
                cancellationToken);
        }

        #endregion

        #region Reset Password

        /// <summary>
        /// Geçerli sıfırlama token'ı ile
        /// kullanıcının şifresini yeniler.
        /// </summary>
        public async Task<bool> ResetAsync(
            ResetPasswordUserDto dto,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. DTO KONTROLÜ
            // =====================================

            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            // =====================================
            // 2. ZORUNLU ALANLAR
            // =====================================

            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Token))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                throw new ValidationException(
                    "Yeni parola zorunludur.");
            }

            // =====================================
            // 3. PAROLA UZUNLUĞU
            // =====================================

            if (dto.NewPassword.Length < 12)
            {
                throw new ValidationException(
                    "Parola en az 12 karakter olmalıdır.");
            }

            // =====================================
            // 4. E-POSTAYI NORMALİZE ET
            // =====================================

            string email = dto.Email.Trim()
                .ToLowerInvariant();

            // =====================================
            // 5. TOKEN HASH'İ OLUŞTUR
            // =====================================

            string tokenHash = HashToken(
                dto.Token);

            // =====================================
            // 6. YENİ PAROLAYI HASH'LE
            // =====================================

            string newPasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.NewPassword,
                    workFactor: 12);

            // =====================================
            // 7. TOKEN DOĞRULA VE ŞİFREYİ GÜNCELLE
            // =====================================

            // Token doğrulama, token tüketme ve
            // parola güncelleme tek transaction
            // içerisinde gerçekleştirilir.

            return await _passwordResetTokenRepository
                .ConsumeAndUpdatePasswordAsync(
                    email,
                    tokenHash,
                    newPasswordHash,
                    cancellationToken);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Token için SHA-256 hash oluşturur.
        /// </summary>
        private static string HashToken(string token)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(token);

            byte[] hash =
                SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }

        /// <summary>
        /// DTO DataAnnotations doğrulaması.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            var context = new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                context,
                validateAllProperties: true);
        }

        #endregion
    }
}