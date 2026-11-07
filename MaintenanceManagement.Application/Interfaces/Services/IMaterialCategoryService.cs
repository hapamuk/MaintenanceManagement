using MaintenanceManagement.Application.DTOS.MaterialCategoryDtos;
using MaintenanceManagement.Application.DTOS.MaterialDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IMaterialCategoryService
    {
        #region Get Methods

        /// <summary>
        /// Bütün malzeme kategorilerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre kategori detayını getirir.
        /// </summary>
        Task<GetByIdMaterialCategoryDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif kategorileri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Pasif kategorileri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetInactiveAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori adına göre kayıt getirir.
        /// </summary>
        Task<GetByIdMaterialCategoryDto?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori adına göre arama yapar.
        /// İsteğe bağlı olarak sadece aktif
        /// kategoriler listelenebilir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialCategoryDto>>
            SearchAsync(
                string searchText,
                bool onlyActive = false,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemeye bağlı kategoriyi getirir.
        /// </summary>
        Task<GetByIdMaterialCategoryDto?> GetByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// En az bir malzemesi bulunan
        /// kategorileri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetCategoriesWithMaterialsAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Hiçbir malzemesi bulunmayan
        /// kategorileri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetCategoriesWithoutMaterialsAsync(
                CancellationToken cancellationToken = default);

        #endregion

        #region Material Methods

        /// <summary>
        /// Kategoriye ait bütün malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetMaterialsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye ait aktif malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetActiveMaterialsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori ve organizasyon birimine göre
        /// malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetMaterialsByCategoryAndUnitIdAsync(
                int categoryId,
                int unitId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni malzeme kategorisi oluşturur.
        /// Kategori adı benzersiz olmalıdır.
        /// Organizasyon seçimleri de kaydedilir.
        /// </summary>
        Task<int> CreateAsync(
            CreateMaterialCategoryDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori bilgilerini ve organizasyon
        /// seçimlerini günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateMaterialCategoryDto dto,
    CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriyi aktif veya pasif yapar.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı isimde kategori bulunuyor mu?
        /// excludedId güncellemede mevcut
        /// kaydı hariç tutmak için kullanılır.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı herhangi bir malzeme
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasMaterialsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif malzeme
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveMaterialsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        /// <summary>
        /// Kategoriye bağlı toplam malzeme sayısı.
        /// </summary>
        Task<int> GetMaterialCountAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif malzeme sayısı.
        /// </summary>
        Task<int> GetActiveMaterialCountAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        #endregion

        #region GetSelectedUnitIds

        /// <summary>
        /// Kategoriye atanmış organizasyonların
        /// ID listesini getirir.
        /// </summary>
        Task<IReadOnlyList<int>> GetSelectedUnitIdsAsync(
            int materialCategoryId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
