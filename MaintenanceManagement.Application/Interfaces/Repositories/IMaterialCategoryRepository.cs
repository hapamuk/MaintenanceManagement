using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IMaterialCategoryRepository
        : IGenericRepository<MaterialCategory>
    {
        /// <summary>
        /// Bütün malzeme kategorilerini
        /// ilişkili malzemeleriyle birlikte getirir.
        /// </summary>
        Task<List<MaterialCategory>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen malzeme kategorisini
        /// ilişkili malzemeleriyle getirir.
        /// </summary>
        Task<MaterialCategory?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif malzeme kategorilerini getirir.
        /// </summary>
        Task<List<MaterialCategory>> GetActiveCategoriesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Pasif malzeme kategorilerini getirir.
        /// </summary>
        Task<List<MaterialCategory>> GetInactiveCategoriesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori adına göre kayıt getirir.
        /// </summary>
        Task<MaterialCategory?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adında veya açıklamasında belirtilen
        /// ifadeyi içeren kategorileri getirir.
        /// </summary>
        Task<List<MaterialCategory>> SearchCategoriesAsync(
            string searchText,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen malzemenin bağlı olduğu
        /// kategoriyi getirir.
        /// </summary>
        Task<MaterialCategory?> GetByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// En az bir malzemeyle ilişkili
        /// kategorileri getirir.
        /// </summary>
        Task<List<MaterialCategory>> GetCategoriesWithMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Hiçbir malzemeyle ilişkisi
        /// bulunmayan kategorileri getirir.
        /// </summary>
        Task<List<MaterialCategory>> GetCategoriesWithoutMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoriye ait aktif
        /// ve pasif bütün malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetMaterialsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoriye bağlı
        /// aktif malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetActiveMaterialsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoride ve organizasyonda
        /// tanımlı malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetMaterialsByCategoryAndUnitIdAsync(
            int categoryId,
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı isimde başka bir kategori
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı herhangi bir
        /// malzeme olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasMaterialsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif
        /// malzeme olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveMaterialsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı toplam
        /// malzeme sayısını getirir.
        /// </summary>
        Task<int> GetMaterialCountAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif
        /// malzeme sayısını getirir.
        /// </summary>
        Task<int> GetActiveMaterialCountAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategorinin aktiflik durumunu değiştirir.
        ///
        /// Aktif malzemelerin kullandığı
        /// kategori pasife alınamaz.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
