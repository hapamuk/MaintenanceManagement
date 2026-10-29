using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaterialCategoryDtos
{
    public class ResultMaterialCategoryDto
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
        /// Kategorinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Aktiflik durumunun Türkçe karşılığıdır.
        /// </summary>
        [Display(Name = "Durum")]
        public string IsActiveName =>
            IsActive ? "Aktif" : "Pasif";

        /// <summary>
        /// Bu kategoriye bağlı toplam
        /// malzeme kayıt sayısıdır.
        /// </summary>
        [Display(Name = "Malzeme Sayısı")]
        public int MaterialCount { get; set; }

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
