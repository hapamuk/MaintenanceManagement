using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class UnitCodeRepository : IUnitCodeRepository
    {
        #region Fields

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public UnitCodeRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Create With Next Code

        /// <summary>
        /// Yeni birime sıradaki sayısal kodu atar.
        ///
        /// Sayaç kilidi ve birim kaydı aynı
        /// transaction içerisinde gerçekleştirilir.
        ///
        /// Başarısız transaction numara tüketmez.
        /// </summary>
        public async Task<Unit> CreateWithNextCodeAsync(
            Unit unit,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(unit);

            // =====================================
            // 1. TRANSACTION BAŞLAT
            // =====================================

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            try
            {
                // =====================================
                // 2. SAYAÇ KAYDINI KİLİTLEYEREK OKU
                // =====================================

                var counter = await _context.UnitCodeCounters
                    .FromSqlRaw(
                        @"SELECT *
                          FROM dbo.UnitCodeCounters
                          WITH (UPDLOCK, HOLDLOCK)
                          WHERE Id = 1")
                    .AsTracking()
                    .SingleAsync(cancellationToken);

                // =====================================
                // 3. SIRADAKİ KODU BELİRLE
                // =====================================

                long nextNumber = counter.NextNumber;

                if (nextNumber <= 0 ||
                    nextNumber == long.MaxValue)
                {
                    throw new InvalidOperationException(
                        "Birim kodu sayacı geçersiz veya dolu.");
                }

                string generatedCode =
                    nextNumber.ToString(
                        CultureInfo.InvariantCulture);

                // =====================================
                // 4. YENİ BİRİME KODU ATA
                // =====================================

                unit.Code = generatedCode;

                unit.IsActive = true;

                unit.CreatedAt = DateTime.UtcNow;

                // =====================================
                // 5. SAYAÇ DEĞERİNİ ARTIR
                // =====================================

                counter.NextNumber = nextNumber + 1;

                // =====================================
                // 6. BİRİM KAYDINI EKLE
                // =====================================

                await _context.Units.AddAsync(
                    unit,
                    cancellationToken);

                // =====================================
                // 7. DEĞİŞİKLİKLERİ KAYDET
                // =====================================

                await _context.SaveChangesAsync(
                    cancellationToken);

                // =====================================
                // 8. TRANSACTION'I TAMAMLA
                // =====================================

                await transaction.CommitAsync(
                    cancellationToken);

                return unit;
            }
            catch
            {
                // =====================================
                // 9. HATA DURUMUNDA GERİ AL
                // =====================================

                await transaction.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }

        #endregion
    }
}
