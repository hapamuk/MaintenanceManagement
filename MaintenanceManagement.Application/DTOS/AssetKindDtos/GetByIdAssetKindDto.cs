using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetKindDtos
{
    public class GetByIdAssetKindDto
    {
        /// <summary>
        /// Demirbaş türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş Türü ID")]
        public int Id { get; set; }

        /// <summary>
        /// Demirbaş türünün bağlı olduğu
        /// kategorinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kategori ID")]
        public int AssetCategoryId { get; set; }

        /// <summary>
        /// Demirbaş türünün bağlı olduğu
        /// kategorinin adıdır.
        /// </summary>
        [Display(Name = "Kategori Adı")]
        public string AssetCategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş kategorisinin
        /// aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Kategori Durumu")]
        public bool AssetCategoryIsActive { get; set; }

        /// <summary>
        /// Demirbaş türünün adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Türü Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş türünün
        /// aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Bu türe bağlı toplam
        /// demirbaş kayıtlarının sayısıdır.
        ///
        /// AssetKind.Assets koleksiyonundan
        /// hesaplanır.
        /// </summary>
        [Display(Name = "Toplam Demirbaş Sayısı")]
        public int AssetCount { get; set; }

        /// <summary>
        /// Demirbaş türünün oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Demirbaş türünün son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
