using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IMaterialCategoryUnitRepository
         : IGenericRepository<MaterialCategoryUnit>
    {
        #region GetByCategoryId

        /// <summary>
        /// Belirtilen malzeme kategorisine bağlı
        /// bütün organizasyon ilişkilerini getirir.
        /// </summary>
        Task<List<MaterialCategoryUnit>> GetByCategoryIdAsync(
            int materialCategoryId,
            CancellationToken cancellationToken = default);

        #endregion

        #region GetUnitIdsByCategoryId

        /// <summary>
        /// Kategoriye atanmış organizasyonların
        /// ID listesini getirir.
        ///
        /// Boş liste tüm organizasyonlar
        /// anlamına gelir.
        /// </summary>
        Task<List<int>> GetUnitIdsByCategoryIdAsync(
            int materialCategoryId,
            CancellationToken cancellationToken = default);

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Belirtilen organizasyon birimine
        /// doğrudan atanmış kategori ilişkilerini getirir.
        /// </summary>
        Task<List<MaterialCategoryUnit>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        #endregion

        #region ExistsRelation

        /// <summary>
        /// Belirtilen kategori ve organizasyon
        /// ilişkisinin mevcut olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsRelationAsync(
            int materialCategoryId,
            int unitId,
            CancellationToken cancellationToken = default);

        #endregion

        #region IsCategoryAllowedForUnit

        /// <summary>
        /// Kategorinin belirtilen organizasyonda
        /// kullanılabilir olup olmadığını kontrol eder.
        ///
        /// Hiç ilişki yoksa tüm organizasyonlara açıktır.
        /// İlişki varsa yalnızca eşleşen organizasyonlara açıktır.
        /// </summary>
        Task<bool> IsCategoryAllowedForUnitAsync(
            int materialCategoryId,
            int unitId,
            CancellationToken cancellationToken = default);

        #endregion

        #region ReplaceCategoryUnits

        /// <summary>
        /// Kategoriye ait organizasyon ilişkilerini
        /// verilen listeyle eşitler.
        ///
        /// Eski ve yeni ilişkiler karşılaştırılır.
        /// Kaldırılanlar silinir, yeniler eklenir.
        ///
        /// Boş liste bütün organizasyonlara
        /// açık olma anlamına gelir.
        /// </summary>
        Task ReplaceCategoryUnitsAsync(
            int materialCategoryId,
            IEnumerable<int> selectedUnitIds,
            CancellationToken cancellationToken = default);

        #endregion

        #region ExecuteInTransaction

        /// <summary>
        /// Kategori kaydı ve organizasyon ilişkilerini
        /// tek transaction içinde gerçekleştirir.
        ///
        /// İşlem başarısız olursa bütün değişiklikler
        /// geri alınır.
        /// </summary>
        Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> operation,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
