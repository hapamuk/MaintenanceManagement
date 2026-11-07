using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class MaterialCategoryUnitRepository
        : GenericRepository<MaterialCategoryUnit>,
          IMaterialCategoryUnitRepository
    {
        #region Constructor

        public MaterialCategoryUnitRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        #endregion

        #region GetByCategoryId

        /// <summary>
        /// Malzeme kategorisine ait organizasyon
        /// ilişkilerini detaylarıyla getirir.
        /// </summary>
        public async Task<List<MaterialCategoryUnit>>
            GetByCategoryIdAsync(
                int materialCategoryId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                materialCategoryId,
                "Malzeme Kategorisi");

            // =====================================
            // 2. İLİŞKİLERİ GETİR
            // =====================================

            return await _context
                .Set<MaterialCategoryUnit>()
                .AsNoTracking()
                .Include(x => x.Unit)
                .Where(x =>
                    x.MaterialCategoryId == materialCategoryId)
                .OrderBy(x => x.Unit.Name)
                .ThenBy(x => x.UnitId)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetUnitIdsByCategoryId

        /// <summary>
        /// Kategoriye ait organizasyon ID listesini getirir.
        /// </summary>
        public async Task<List<int>>
            GetUnitIdsByCategoryIdAsync(
                int materialCategoryId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                materialCategoryId,
                "Malzeme Kategorisi");

            // =====================================
            // 2. ORGANİZASYON ID'LERİ
            // =====================================

            return await _context
                .Set<MaterialCategoryUnit>()
                .AsNoTracking()
                .Where(x =>
                    x.MaterialCategoryId == materialCategoryId)
                .Select(x => x.UnitId)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Organizasyon birimine doğrudan tanımlanan
        /// kategori ilişkilerini getirir.
        ///
        /// Tüm organizasyonlara açık kategoriler
        /// bu metotla ayrıca listelenmez.
        /// </summary>
        public async Task<List<MaterialCategoryUnit>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                unitId,
                "Organizasyon Birimi");

            // =====================================
            // 2. İLİŞKİLERİ GETİR
            // =====================================

            return await _context
                .Set<MaterialCategoryUnit>()
                .AsNoTracking()
                .Include(x => x.MaterialCategory)
                .Where(x => x.UnitId == unitId)
                .OrderBy(x => x.MaterialCategory.Name)
                .ThenBy(x => x.MaterialCategoryId)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region ExistsRelation

        /// <summary>
        /// Kategori ile organizasyon arasında
        /// doğrudan ilişki olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsRelationAsync(
            int materialCategoryId,
            int unitId,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                materialCategoryId,
                "Malzeme Kategorisi");

            ValidateId(
                unitId,
                "Organizasyon Birimi");

            // =====================================
            // 2. İLİŞKİ KONTROLÜ
            // =====================================

            return await _context
                .Set<MaterialCategoryUnit>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MaterialCategoryId == materialCategoryId &&
                    x.UnitId == unitId,
                    cancellationToken);
        }

        #endregion

        #region IsCategoryAllowedForUnit

        /// <summary>
        /// Kategorinin belirtilen organizasyonda
        /// kullanılabilir olup olmadığını kontrol eder.
        ///
        /// Kategoriye ait ilişki yoksa tüm
        /// aktif organizasyonlara açıktır.
        ///
        /// İlişki varsa yalnızca eşleşen
        /// organizasyonlarda kullanılabilir.
        /// </summary>
        public async Task<bool> IsCategoryAllowedForUnitAsync(
            int materialCategoryId,
            int unitId,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                materialCategoryId,
                "Malzeme Kategorisi");

            ValidateId(
                unitId,
                "Organizasyon Birimi");

            // =====================================
            // 2. KATEGORİ AKTİF Mİ?
            // =====================================

            bool categoryIsActive = await _context
                .Set<MaterialCategory>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == materialCategoryId &&
                    x.IsActive,
                    cancellationToken);

            if (!categoryIsActive)
            {
                return false;
            }

            // =====================================
            // 3. ORGANİZASYON AKTİF Mİ?
            // =====================================

            bool unitIsActive = await _context
                .Set<Unit>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == unitId &&
                    x.IsActive,
                    cancellationToken);

            if (!unitIsActive)
            {
                return false;
            }

            // =====================================
            // 4. KATEGORİNİN İLİŞKİSİ VAR MI?
            // =====================================

            bool hasAssignedUnits = await _context
                .Set<MaterialCategoryUnit>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MaterialCategoryId == materialCategoryId,
                    cancellationToken);

            // =====================================
            // 5. TÜM ORGANİZASYONLARA AÇIK
            // =====================================

            if (!hasAssignedUnits)
            {
                return true;
            }

            // =====================================
            // 6. SEÇİLİ ORGANİZASYON KONTROLÜ
            // =====================================

            return await _context
                .Set<MaterialCategoryUnit>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MaterialCategoryId == materialCategoryId &&
                    x.UnitId == unitId,
                    cancellationToken);
        }

        #endregion


        #region ReplaceCategoryUnits

        /// <summary>
        /// Kategoriye ait organizasyon ilişkilerini
        /// verilen listeye göre günceller.
        ///
        /// Boş liste = Tüm organizasyonlara açık.
        ///
        /// Kategori oluşturma ve güncelleme işlemlerinde
        /// ExecuteInTransactionAsync kapsamında çağrılır.
        /// </summary>
        public async Task ReplaceCategoryUnitsAsync(
            int materialCategoryId,
            IEnumerable<int> selectedUnitIds,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. PARAMETRE DOĞRULAMA
            // =====================================

            if (materialCategoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(materialCategoryId),
                    "Geçerli bir malzeme kategorisi seçiniz.");
            }

            ArgumentNullException.ThrowIfNull(
                selectedUnitIds);

            // =====================================
            // 2. SEÇİLEN ORGANİZASYONLAR
            // =====================================

            var selectedIds = selectedUnitIds
                .Distinct()
                .ToList();

            if (selectedIds.Any(x => x <= 0))
            {
                throw new ValidationException(
                    "Organizasyon listesinde geçersiz ID bulunmaktadır.");
            }

            // =====================================
            // 3. KATEGORİ KONTROLÜ
            // =====================================

            bool categoryExists = await _context
                .Set<MaterialCategory>()
                .AnyAsync(
                    x => x.Id == materialCategoryId,
                    cancellationToken);

            if (!categoryExists)
            {
                throw new InvalidOperationException(
                    "Malzeme kategorisi bulunamadı.");
            }

            // =====================================
            // 4. ORGANİZASYON KONTROLÜ
            // =====================================

            if (selectedIds.Count > 0)
            {
                int validCount = await _context
                    .Set<Unit>()
                    .CountAsync(
                        x => selectedIds.Contains(x.Id) &&
                             x.IsActive,
                        cancellationToken);

                if (validCount != selectedIds.Count)
                {
                    throw new ValidationException(
                        "Seçilen organizasyonlardan biri veya " +
                        "birkaçı bulunamadı ya da pasiftir.");
                }
            }

            // =====================================
            // 5. MEVCUT İLİŞKİLER
            // =====================================

            var existingRelations = await _context
                .Set<MaterialCategoryUnit>()
                .Where(
                    x => x.MaterialCategoryId ==
                         materialCategoryId)
                .ToListAsync(cancellationToken);

            // =====================================
            // 6. EKLENECEK / SİLİNECEK İLİŞKİLER
            // =====================================

            var selectedIdSet =
                selectedIds.ToHashSet();

            var existingIdSet = existingRelations
                .Select(x => x.UnitId)
                .ToHashSet();

            var toRemove = existingRelations
                .Where(x => !selectedIdSet.Contains(x.UnitId))
                .ToList();

            var toAdd = selectedIds
                .Where(x => !existingIdSet.Contains(x))
                .Select(x => new MaterialCategoryUnit
                {
                    MaterialCategoryId = materialCategoryId,
                    UnitId = x
                })
                .ToList();

            // =====================================
            // 7. İLİŞKİLERİ GÜNCELLE
            // =====================================

            if (toRemove.Count > 0)
            {
                _context.Set<MaterialCategoryUnit>()
                    .RemoveRange(toRemove);
            }

            if (toAdd.Count > 0)
            {
                await _context.Set<MaterialCategoryUnit>()
                    .AddRangeAsync(
                        toAdd,
                        cancellationToken);
            }

            // =====================================
            // 8. VERİTABANINA KAYDET
            // =====================================

            if (toRemove.Count > 0 ||
                toAdd.Count > 0)
            {
                await _context.SaveChangesAsync(
                    cancellationToken);
            }
        }

        #endregion


        #region Private Validation Methods

        /// <summary>
        /// ID değerinin pozitif olmasını sağlar.
        /// </summary>
        private static void ValidateId(
            int id,
            string fieldName)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    $"Geçerli bir {fieldName} seçiniz.");
            }
        }

        #endregion


        #region ExecuteInTransaction

        /// <summary>
        /// Aynı DbContext üzerinden yürütülen kategori
        /// ve organizasyon kayıtlarını tek transaction
        /// içerisinde gerçekleştirir.
        /// </summary>
        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);

            if (_context.Database.CurrentTransaction != null)
            {
                throw new InvalidOperationException(
                    "Bu işlem zaten açık bir transaction " +
                    "içerisinde yeniden başlatılamaz.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable,
                    cancellationToken);

            try
            {
                T result = await operation();

                await transaction.CommitAsync(
                    cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }

        #endregion



    }
}
