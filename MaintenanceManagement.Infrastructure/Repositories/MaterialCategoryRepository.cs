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
    public class MaterialCategoryRepository :
          GenericRepository<MaterialCategory>,
          IMaterialCategoryRepository
    {
        public MaterialCategoryRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün kategorileri ilişkili
        /// malzemeleriyle birlikte getirir.
        /// </summary>
        public async Task<List<MaterialCategory>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kategoriyi ID bilgisine göre
        /// malzemeleriyle birlikte getirir.
        /// </summary>
        public async Task<MaterialCategory?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Aktif malzeme kategorilerini
        /// adlarına göre sıralı getirir.
        /// </summary>
        public async Task<List<MaterialCategory>>
            GetActiveCategoriesAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Pasif malzeme kategorilerini getirir.
        /// </summary>
        public async Task<List<MaterialCategory>>
            GetInactiveCategoriesAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kategori adına göre
        /// mevcut kaydı getirir.
        /// </summary>
        public async Task<MaterialCategory?>
            GetByNameAsync(
                string name,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            string normalizedName = name.Trim();

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Name == normalizedName,
                    cancellationToken);
        }

        /// <summary>
        /// Kategori adı veya açıklamasında
        /// belirtilen ifadeyi arar.
        ///
        /// onlyActive true ise sadece
        /// aktif kategoriler getirilir.
        /// </summary>
        public async Task<List<MaterialCategory>>
            SearchCategoriesAsync(
                string searchText,
                bool onlyActive = false,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new List<MaterialCategory>();
            }

            string normalizedSearchText = searchText.Trim();

            IQueryable<MaterialCategory> query =
                _dbSet.AsNoTracking();

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            query = query.Where(x =>
                x.Name.Contains(normalizedSearchText) ||
                (
                    x.Description != null &&
                    x.Description.Contains(normalizedSearchText)
                ));

            return await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen malzemenin
        /// bağlı olduğu kategoriyi getirir.
        ///
        /// Malzemeye kategori atanmamışsa
        /// null döndürür.
        /// </summary>
        public async Task<MaterialCategory?>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            if (materialId <= 0)
            {
                return null;
            }

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Materials.Any(
                        material => material.Id == materialId),
                    cancellationToken);
        }

        /// <summary>
        /// En az bir malzeme kaydıyla
        /// ilişkili kategorileri getirir.
        ///
        /// Aktif ve pasif malzemeler
        /// ilişki kontrolüne dahildir.
        /// </summary>
        public async Task<List<MaterialCategory>>
            GetCategoriesWithMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.Materials.Any())
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Hiçbir malzeme kaydıyla
        /// ilişkisi bulunmayan kategorileri getirir.
        /// </summary>
        public async Task<List<MaterialCategory>>
            GetCategoriesWithoutMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => !x.Materials.Any())
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategoriye bağlı
        /// bütün malzemeleri getirir.
        ///
        /// Aktif ve pasif malzemeler dahildir.
        /// </summary>
        public async Task<List<Material>>
            GetMaterialsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .Include(x => x.MeasurementUnit)
                .Where(x =>
                    x.MaterialCategoryId == categoryId)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategoriye bağlı
        /// aktif malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>>
            GetActiveMaterialsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .Include(x => x.MeasurementUnit)
                .Where(x =>
                    x.MaterialCategoryId == categoryId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategori ve organizasyonda
        /// tanımlanmış malzemeleri getirir.
        ///
        /// Aktif ve pasif kayıtlar dahildir.
        /// </summary>
        public async Task<List<Material>>
            GetMaterialsByCategoryAndUnitIdAsync(
                int categoryId,
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .Include(x => x.MeasurementUnit)
                .Where(x =>
                    x.MaterialCategoryId == categoryId &&
                    x.UnitId == unitId)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aynı isimde başka bir kategori
        /// bulunup bulunmadığını kontrol eder.
        ///
        /// Güncelleme sırasında mevcut kayıt
        /// excludedId ile hariç tutulabilir.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
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
                .AnyAsync(x =>
                    x.Name == normalizedName &&
                    (
                        !excludedId.HasValue ||
                        x.Id != excludedId.Value
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// Kategoriye bağlı herhangi bir
        /// malzeme olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasMaterialsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.MaterialCategoryId == categoryId,
                    cancellationToken);
        }

        /// <summary>
        /// Kategoriye bağlı aktif
        /// malzeme olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveMaterialsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MaterialCategoryId == categoryId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Kategoriye bağlı toplam
        /// malzeme sayısını getirir.
        ///
        /// Aktif ve pasif kayıtlar dahildir.
        /// </summary>
        public async Task<int> GetMaterialCountAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .CountAsync(
                    x => x.MaterialCategoryId == categoryId,
                    cancellationToken);
        }

        /// <summary>
        /// Kategoriye bağlı aktif
        /// malzemelerin sayısını getirir.
        /// </summary>
        public async Task<int> GetActiveMaterialCountAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .CountAsync(x =>
                    x.MaterialCategoryId == categoryId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Kategorinin aktiflik durumunu değiştirir.
        ///
        /// Kategori aktif malzemeler tarafından
        /// kullanılıyorsa pasife alınamaz.
        ///
        /// Kayıt fiziksel olarak silinmez.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var category = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (category == null)
            {
                return false;
            }

            // Kategori zaten istenen
            // durumdaysa işlem yapılmaz.
            if (category.IsActive == isActive)
            {
                return true;
            }

            // Pasife alınırken aktif malzeme
            // ilişkilerinin bulunmadığını doğrula.
            if (!isActive)
            {
                bool hasActiveMaterials =
                    await _context.Set<Material>()
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.MaterialCategoryId == id &&
                            x.IsActive,
                            cancellationToken);

                if (hasActiveMaterials)
                {
                    throw new InvalidOperationException(
                        "Bu kategori aktif malzemeler " +
                        "tarafından kullanılmaktadır. " +
                        "Malzeme kategorisi pasife alınamaz.");
                }
            }

            category.IsActive = isActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Kategorileri bağlı malzemelerle
        /// birlikte getiren ortak sorgudur.
        ///
        /// AsSplitQuery ile ilişkili koleksiyonlar
        /// ayrı sorgular üzerinden yüklenir.
        /// </summary>
        private IQueryable<MaterialCategory> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.Materials)
                .AsSplitQuery();
        }
    }
}
