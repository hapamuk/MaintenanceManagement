using MaintenanceManagement.Application.DTOS.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün kullanıcıları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif kullanıcıları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre kullanıcı detayını getirir.
        /// </summary>
        Task<GetByIdUserDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// E-posta adresine göre kullanıcı detayını getirir.
        /// </summary>
        Task<GetByIdUserDto?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı adına göre kullanıcı detayını getirir.
        /// </summary>
        Task<GetByIdUserDto?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon birimine bağlı
        /// kullanıcıları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserDto>> GetByUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ekibe bağlı kullanıcıları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserDto>> GetByTeamAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role atanmış kullanıcıları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserDto>> GetByRoleAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni kullanıcı oluşturur ve kullanıcı ID döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateUserDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut kullanıcı bilgilerini günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateUserDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status / Access Methods

        /// <summary>
        /// Kullanıcının aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının sisteme erişim yetkisini değiştirir.
        /// </summary>
        Task<bool> ChangeSystemAccessAsync(
            int id,
            bool hasSystemAccess,
            CancellationToken cancellationToken = default);

        #endregion

        #region Password Methods

        /// <summary>
        /// Kullanıcının mevcut parolasını doğrular
        /// ve yeni parolasını güvenli şekilde kaydeder.
        /// </summary>
        Task<bool> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// E-posta adresinin başka kullanıcı tarafından
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsEmailAsync(
            string email,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı adının başka kullanıcı tarafından
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsUsernameAsync(
            string username,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        #endregion

        #region Register Methods

        /// <summary>
        /// Yalnızca admin tarafından yeni kullanıcı oluşturulur.
        /// Kullanıcının başlangıç birim ve rol atamaları yapılır.
        /// </summary>
        Task<int> RegisterByAdminAsync(
            RegisterUserDto dto,
            CancellationToken cancellationToken = default);

        #endregion


        #region Authentication Methods

        /// <summary>
        /// Kullanıcı adı ve parola ile giriş işlemini gerçekleştirir.
        /// </summary>
        Task<LoginResultUserDto?> LoginAsync(
            LoginUserDto dto,
            CancellationToken cancellationToken = default);

        #endregion


        #region Profile Methods

        /// <summary>
        /// Kullanıcının profil bilgilerini getirir.
        /// </summary>
        Task<UserProfileDto?> GetProfileAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının profil bilgilerini günceller.
        /// Rol ve yetki bilgileri değiştirilemez.
        /// </summary>
        Task<bool> UpdateProfileAsync(
            int userId,
            UpdateUserProfileDto dto,
            CancellationToken cancellationToken = default);

        #endregion


        #region Password Reset Methods

        /// <summary>
        /// Kullanıcıya şifre sıfırlama talebi oluşturur.
        /// </summary>
        Task RequestPasswordResetAsync(
            ForgotPasswordUserDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Geçerli sıfırlama token'ı ile şifreyi yeniler.
        /// </summary>
        Task<bool> ResetPasswordAsync(
            ResetPasswordUserDto dto,
            CancellationToken cancellationToken = default);

        #endregion

    }
}
