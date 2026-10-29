using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaterialCategoryDtos
{
    public class GetByIdMaterialCategoryDto
    {
        /// <summary>
        /// Malzeme kategorisinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kategori ID")]
        public int Id { get; set; }

        /// <summary>
        /// Malzeme kategorisinin adıdır.
        /// </summary>
        [Display(Name = "Malzeme Kategorisi")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Malzeme kategorisinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Malzeme kategorisinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Aktiflik durumunun Türkçe açıklamasıdır.
        /// </summary>
        [Display(Name = "Durum")]
        public string IsActiveName =>
            IsActive ? "Aktif" : "Pasif";

        /// <summary>
        /// Bu kategoriye bağlı toplam
        /// malzeme sayısıdır.
        /// </summary>
        [Display(Name = "Toplam Malzeme Sayısı")]
        public int MaterialCount { get; set; }

        /// <summary>
        /// Kategoriye bağlı aktif
        /// malzeme sayısıdır.
        /// </summary>
        [Display(Name = "Aktif Malzeme Sayısı")]
        public int ActiveMaterialCount { get; set; }

        /// <summary>
        /// Kategoriye bağlı pasif
        /// malzeme sayısıdır.
        /// </summary>
        [Display(Name = "Pasif Malzeme Sayısı")]
        public int PassiveMaterialCount { get; set; }

        /// <summary>
        /// Kategori kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kategori kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
