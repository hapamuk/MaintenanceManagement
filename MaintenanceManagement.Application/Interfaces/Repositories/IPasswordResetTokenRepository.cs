using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        /// <summary>
        /// Oluşturulan sıfırlama token hash'ini kaydeder.
        /// </summary>
        Task CreateAsync(
            PasswordResetToken token,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Token ve e-posta bilgilerini doğrular.
        ///
        /// Geçerli ise token'ı tüketir,
        /// parolayı yeniler ve diğer sıfırlama
        /// token'larını geçersiz hale getirir.
        ///
        /// Bütün işlemler tek transaction içindedir.
        /// </summary>
        Task<bool> ConsumeAndUpdatePasswordAsync(
            string email,
            string tokenHash,
            string newPasswordHash,
            CancellationToken cancellationToken = default);
    }
}
