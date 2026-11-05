using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository
        : IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public PasswordResetTokenRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        #region Create

        public async Task CreateAsync(
            PasswordResetToken token,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(token);

            await _context.Set<PasswordResetToken>()
                .AddAsync(
                    token,
                    cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        #endregion

        #region Consume And Update Password

        public async Task<bool> ConsumeAndUpdatePasswordAsync(
            string email,
            string tokenHash,
            string newPasswordHash,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(tokenHash) ||
                string.IsNullOrWhiteSpace(newPasswordHash))
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;

            // Token tüketimi ve parola değişikliği
            // aynı veritabanı işlemi içerisinde yapılır.
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            // =====================================
            // 1. TOKEN DOĞRULAMA
            // =====================================

            var resetToken =
                await _context.Set<PasswordResetToken>()
                    .FirstOrDefaultAsync(
                        x => x.TokenHash == tokenHash &&
                             x.UsedAtUtc == null &&
                             x.ExpiresAtUtc > now,
                        cancellationToken);

            if (resetToken == null)
                return false;

            // =====================================
            // 2. KULLANICI DOĞRULAMA
            // =====================================

            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(
                    x => x.Id == resetToken.UserId &&
                         x.Email == email &&
                         x.IsActive &&
                         x.HasSystemAccess,
                    cancellationToken);

            if (user == null)
                return false;

            // =====================================
            // 3. PAROLAYI GÜNCELLE
            // =====================================

            user.PasswordHash = newPasswordHash;

            // =====================================
            // 4. TÜM AÇIK TOKEN'LARI TÜKET
            // =====================================

            var activeTokens =
                await _context.Set<PasswordResetToken>()
                    .Where(
                        x => x.UserId == user.Id &&
                             x.UsedAtUtc == null)
                    .ToListAsync(cancellationToken);

            foreach (var item in activeTokens)
            {
                item.UsedAtUtc = now;
            }

            // =====================================
            // 5. DEĞİŞİKLİKLERİ KAYDET
            // =====================================

            await _context.SaveChangesAsync(
                cancellationToken);

            // =====================================
            // 6. TRANSACTION TAMAMLA
            // =====================================

            await transaction.CommitAsync(
                cancellationToken);

            return true;
        }

        #endregion
    }
}
