using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetCategoryDtos
{
    public class ResultAssetCategoryDto
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
