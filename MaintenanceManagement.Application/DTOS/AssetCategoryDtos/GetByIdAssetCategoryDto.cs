using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetCategoryDtos
{
    public class GetByIdAssetCategoryDto
    {
        /// <summary>
        /// Demirbaş kategorisinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kategori ID")]
        public int Id { get; set; }

        /// <summary>
        /// Demirbaş kategorisinin adıdır.
        /// </summary>
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş kategorisinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Kategorinin aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Kategoriye bağlı toplam demirbaş türü
        /// sayısını belirtir.
        ///
        /// Aktif ve pasif türler dahildir.
        /// </summary>
        [Display(Name = "Toplam Demirbaş Türü Sayısı")]
        public int KindCount { get; set; }

        /// <summary>
        /// Kategoriye bağlı aktif
        /// demirbaş türlerinin sayısıdır.
        /// </summary>
        [Display(Name = "Aktif Demirbaş Türü Sayısı")]
        public int ActiveKindCount { get; set; }

        /// <summary>
        /// Kategoriye bağlı pasif
        /// demirbaş türlerinin sayısıdır.
        /// </summary>
        [Display(Name = "Pasif Demirbaş Türü Sayısı")]
        public int InactiveKindCount { get; set; }

        /// <summary>
        /// Kategorinin oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kategorinin son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
