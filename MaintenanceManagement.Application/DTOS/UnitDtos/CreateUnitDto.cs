using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UnitDtos
{
    public class CreateUnitDto
    {
        /// <summary>
        /// Organizasyon biriminin adını belirtir.
        /// </summary>
        [Required(
            ErrorMessage = "Organizasyon birimi adı zorunludur.")]
        [Display(Name = "Birim Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin kodunu belirtir.
        /// </summary>
        
        [Display(Name = "Birim Kodu")]
        public string? Code { get; set; } 

        /// <summary>
        /// Birimin bağlı olduğu organizasyon
        /// türünün ID bilgisini belirtir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon türü seçiniz.")]
        [Display(Name = "Organizasyon Türü")]
        public int OrganizationTypeId { get; set; }

        /// <summary>
        /// Birimin bağlı olacağı üst
        /// organizasyon birimini belirtir.
        ///
        /// En üst düzey birimler için
        /// null olabilir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir üst birim seçiniz.")]
        [Display(Name = "Üst Birim")]
        public int? ParentUnitId { get; set; }

        [StringLength(150)]
        public string? Description { get; set; }
    }
}
