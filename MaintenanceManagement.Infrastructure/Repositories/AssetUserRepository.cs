using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class AssetUserRepository :
         GenericRepository<AssetUser>,
         IAssetUserRepository
    {
        public AssetUserRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün zimmet kayıtlarını demirbaş,
        /// personel ve zimmeti veren kullanıcı
        /// detaylarıyla birlikte getirir.
        /// </summary>
        public async Task<List<AssetUser>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.StartDate)
                .ThenBy(x => x.Asset.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen zimmet kaydını ilişkili
        /// bütün detaylarıyla getirir.
        /// </summary>
        public async Task<AssetUser?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaşın geçmiş ve mevcut bütün
        /// zimmet kayıtlarını getirir.
        /// </summary>
        public async Task<List<AssetUser>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının geçmiş ve mevcut bütün
        /// zimmet kayıtlarını getirir.
        /// </summary>
        public async Task<List<AssetUser>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Zimmet başlangıcı gerçekleşmiş ve
        /// henüz iade edilmemiş kayıtları getirir.
        /// </summary>
        public async Task<List<AssetUser>>
            GetActiveAssignmentsAsync(
                CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await GetDetailsQuery()
                .Where(x =>
                    x.StartDate <= now &&
                    x.EndDate == null)
                .OrderBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ThenBy(x => x.Asset.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Demirbaşın mevcut aktif zimmet
        /// kaydını getirir.
        /// </summary>
        public async Task<AssetUser?>
            GetActiveAssignmentByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await GetDetailsQuery()
                .Where(x =>
                    x.AssetId == assetId &&
                    x.StartDate <= now &&
                    x.EndDate == null)
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcının halen zimmetinde
        /// bulunan demirbaş kayıtlarını getirir.
        /// </summary>
        public async Task<List<AssetUser>>
            GetActiveAssignmentsByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await GetDetailsQuery()
                .Where(x =>
                    x.UserId == userId &&
                    x.StartDate <= now &&
                    x.EndDate == null)
                .OrderBy(x => x.Asset.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// gerçekleştirilen zimmet işlemlerini getirir.
        /// </summary>
        public async Task<List<AssetUser>>
            GetAssignmentsMadeByUserIdAsync(
                int assignedByUserId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.AssignedByUserId == assignedByUserId)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Demirbaşın halen aktif bir zimmetinin
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssignmentAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetId == assetId &&
                    x.StartDate <= now &&
                    x.EndDate == null,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaşın henüz iade edilmemiş
        /// herhangi bir zimmet kaydı olup olmadığını
        /// kontrol eder.
        ///
        /// İleri tarihli zimmetler de dahildir.
        /// </summary>
        public async Task<bool> HasUnreturnedAssignmentAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetId == assetId &&
                    x.EndDate == null,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcının üzerinde halen aktif
        /// zimmet bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssignmentsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.StartDate <= now &&
                    x.EndDate == null,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaşın belirtilen kullanıcıya
        /// aktif olarak zimmetli olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> IsAssetAssignedToUserAsync(
            int assetId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetId == assetId &&
                    x.UserId == userId &&
                    x.StartDate <= now &&
                    x.EndDate == null,
                    cancellationToken);
        }

        /// <summary>
        /// Zimmetin iade işlemini gerçekleştirir.
        ///
        /// EndDate alanını doldurur ve
        /// veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ReturnAssetAsync(
            int assignmentId,
            DateTime? returnedAtUtc = null,
            CancellationToken cancellationToken = default)
        {
            var assetUser = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == assignmentId,
                    cancellationToken);

            if (assetUser == null)
            {
                return false;
            }

            // Zimmet daha önce iade edilmişse
            // ikinci kez iade işlemi yapılmaz.
            if (assetUser.EndDate.HasValue)
            {
                return false;
            }

            DateTime returnDate =
                returnedAtUtc ?? DateTime.UtcNow;

            // Tarih değerleri UTC olarak tutulmalıdır.
            if (returnDate.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "İade tarihi UTC formatında olmalıdır.",
                    nameof(returnedAtUtc));
            }

            // Zimmet başlangıcından önce iade yapılamaz.
            if (returnDate < assetUser.StartDate)
            {
                throw new InvalidOperationException(
                    "İade tarihi zimmet başlangıç tarihinden önce olamaz.");
            }

            // Gelecek tarihli iade kaydı oluşturulmaz.
            if (returnDate > DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    "İade tarihi gelecekte olamaz.");
            }

            assetUser.EndDate = returnDate;

            assetUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Zimmet kayıtlarını demirbaş, marka,
        /// model, kategori, organizasyon, zimmetli
        /// personel ve zimmeti veren personel
        /// bilgileriyle getiren ortak sorgudur.
        /// </summary>
        private IQueryable<AssetUser> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // Demirbaş ve organizasyon
                .Include(x => x.Asset)
                    .ThenInclude(x => x.Unit)

                // Demirbaş modeli ve markası
                .Include(x => x.Asset)
                    .ThenInclude(x => x.AssetModel)
                        .ThenInclude(x => x.Brand)

                // Demirbaş türü ve kategorisi
                .Include(x => x.Asset)
                    .ThenInclude(x => x.AssetKind)
                        .ThenInclude(x => x.AssetCategory)

                // Zimmetli personel
                .Include(x => x.User)
                    .ThenInclude(x => x.PrimaryUnit)

                // Zimmet işlemini yapan kullanıcı
                .Include(x => x.AssignedByUser);
        }
    }
}
