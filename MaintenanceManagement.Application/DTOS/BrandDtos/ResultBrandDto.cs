using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.BrandDtos
{
    public class ResultBrandDto
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
