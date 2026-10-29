using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.BrandDtos
{
    public class GetByIdBrandDto
    {
        /// <summary>
        /// Markanın ID bilgisidir.
        /// </summary>
        [Display(Name = "Marka ID")]
        public int Id { get; set; }

        /// <summary>
        /// Markanın adıdır.
        /// </summary>
        [Display(Name = "Marka Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Markanın aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Markaya bağlı toplam model sayısıdır.
        ///
        /// Aktif ve pasif modeller dahildir.
        /// </summary>
        [Display(Name = "Toplam Model Sayısı")]
        public int ModelCount { get; set; }

        /// <summary>
        /// Markaya bağlı aktif
        /// demirbaş modeli sayısıdır.
        /// </summary>
        [Display(Name = "Aktif Model Sayısı")]
        public int ActiveModelCount { get; set; }

        /// <summary>
        /// Markaya bağlı pasif
        /// demirbaş modeli sayısıdır.
        /// </summary>
        [Display(Name = "Pasif Model Sayısı")]
        public int InactiveModelCount { get; set; }

        /// <summary>
        /// Markanın oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Markanın son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
