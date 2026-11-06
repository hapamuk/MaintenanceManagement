
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class UnitRepository :
        GenericRepository<Unit>, IUnitRepository
    {
        #region Constructor

        public UnitRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        #endregion

        #region GetActiveUnits

        /// <summary>
        /// Aktif organizasyon birimlerini
        /// alfabetik sırayla getirir.
        /// </summary>
        public async Task<List<Unit>> GetActiveUnitsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetAllWithDetails

        /// <summary>
        /// Tüm organizasyon birimlerini,
        /// üst birim ve alt birim ilişkileriyle getirir.
        ///
        /// Organizasyon türü artık
        /// OrganizationTypeEnum üzerinden yönetilir.
        /// </summary>
        public async Task<List<Unit>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()

                // Üst birim ilişkisi
                .Include(x => x.ParentUnit)

                // Alt birimler
                .Include(x => x.ChildUnits)

                .AsSplitQuery()

                .OrderBy(x => x.Name)

                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetByIdWithDetails

        /// <summary>
        /// Belirtilen organizasyon birimini;
        /// üst birim, alt birimler, kullanıcılar,
        /// ekipler, roller ve iş türleriyle getirir.
        ///
        /// OrganizationType navigation kaldırılmıştır.
        /// Organizasyon türü enum alanında tutulur.
        /// </summary>
        public async Task<Unit?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _dbSet
                .AsNoTracking()

                // Parent - Child
                .Include(x => x.ParentUnit)
                .Include(x => x.ChildUnits)

                // Kullanıcılar
                .Include(x => x.Users)

                // Ekipler
                .Include(x => x.Teams)

                // Roller
                .Include(x => x.Roles)

                // Birime bağlı iş türleri
                .Include(x => x.UnitWorkTypes)
                    .ThenInclude(x => x.WorkType)

                .AsSplitQuery()

                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        #endregion

        #region GetUnitsByOrganizationType

        /// <summary>
        /// Belirtilen organizasyon türüne sahip
        /// aktif birimleri getirir.
        ///
        /// Örnek:
        /// OrganizationTypeEnum.HeadOffice
        /// OrganizationTypeEnum.DepartmentHeadship
        /// OrganizationTypeEnum.BranchManagement
        /// OrganizationTypeEnum.Headship
        /// </summary>
        public async Task<List<Unit>>
            GetUnitsByOrganizationTypeAsync(
                OrganizationTypeEnum organizationType,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ENUM DOĞRULAMA
            // =====================================

            if (!Enum.IsDefined(
                typeof(OrganizationTypeEnum),
                organizationType))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(organizationType),
                    "Geçerli bir organizasyon türü seçiniz.");
            }

            // =====================================
            // 2. AKTİF BİRİMLERİ GETİR
            // =====================================

            return await _dbSet
                .AsNoTracking()

                .Where(x =>
                    x.OrganizationTypeEnum == organizationType &&
                    x.IsActive)

                .OrderBy(x => x.Name)

                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetChildUnits

        /// <summary>
        /// Belirtilen üst birime bağlı
        /// aktif alt birimleri getirir.
        ///
        /// ParentUnitId üzerinden çalışır.
        /// </summary>
        public async Task<List<Unit>> GetChildUnitsAsync(
            int parentUnitId,
            CancellationToken cancellationToken = default)
        {
            if (parentUnitId <= 0)
            {
                return new List<Unit>();
            }

            return await _dbSet
                .AsNoTracking()

                .Where(x =>
                    x.ParentUnitId == parentUnitId &&
                    x.IsActive)

                .OrderBy(x => x.Name)

                .ToListAsync(cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Aynı üst birim altında aynı isimde
        /// başka bir birim olup olmadığını kontrol eder.
        ///
        /// Aktif ve pasif bütün birimler dikkate alınır.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? parentUnitId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string normalizedName = name.Trim();

            return await _dbSet
                .AsNoTracking()

                .AnyAsync(
                    x =>
                        x.Name == normalizedName &&

                        x.ParentUnitId == parentUnitId &&

                        (!excludedId.HasValue ||
                         x.Id != excludedId.Value),

                    cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Birim kodunun daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        ///
        /// Pasif birimlerin kodları da korunur.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            string normalizedCode = code.Trim();

            return await _dbSet
                .AsNoTracking()

                .AnyAsync(
                    x =>
                        x.Code == normalizedCode &&

                        (!excludedId.HasValue ||
                         x.Id != excludedId.Value),

                    cancellationToken);
        }

        #endregion

        #region HasChildUnits

        /// <summary>
        /// Organizasyon birimine bağlı
        /// alt birim bulunup bulunmadığını kontrol eder.
        ///
        /// Aktif ve pasif alt birimler dikkate alınır.
        /// </summary>
        public async Task<bool> HasChildUnitsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _dbSet
                .AsNoTracking()

                .AnyAsync(
                    x => x.ParentUnitId == id,
                    cancellationToken);
        }

        #endregion

        #region HasActiveTeams

        /// <summary>
        /// Organizasyon birimine bağlı
        /// aktif ekip bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveTeamsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _context.Teams
                .AsNoTracking()

                .AnyAsync(
                    x =>
                        x.UnitId == id &&
                        x.IsActive,

                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Organizasyon biriminin aktif/pasif
        /// durumunu değiştirir.
        ///
        /// Birim kodu değiştirilmez.
        /// Organizasyon türü değiştirilmez.
        /// ParentUnitId değiştirilmez.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID KONTROLÜ
            // =====================================

            if (id <= 0)
            {
                return false;
            }

            // =====================================
            // 2. BİRİMİ GETİR
            // =====================================

            var unit = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (unit == null)
            {
                return false;
            }

            // =====================================
            // 3. DURUM KONTROLÜ
            // =====================================

            if (unit.IsActive == isActive)
            {
                return true;
            }

            // =====================================
            // 4. DURUMU GÜNCELLE
            // =====================================

            unit.IsActive = isActive;

            unit.UpdatedAt = DateTime.UtcNow;

            // =====================================
            // 5. KAYDET
            // =====================================

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        

        #endregion
    }
}
