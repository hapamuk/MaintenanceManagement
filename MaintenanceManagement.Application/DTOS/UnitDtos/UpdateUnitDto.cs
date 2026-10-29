using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UnitDtos
{
    public class UpdateUnitDto
    {
     
        public int Id { get; set; }

        /// <summary>
        /// Organizasyon biriminin güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Organizasyon birimi adı zorunludur.")]
        [Display(Name = "Birim Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin güncel kodudur.
        /// </summary>
        [Required(
            ErrorMessage = "Organizasyon birimi kodu zorunludur.")]
        [Display(Name = "Birim Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin bağlı olduğu
        /// organizasyon türünün ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon türü seçiniz.")]
        [Display(Name = "Organizasyon Türü")]
        public int OrganizationTypeId { get; set; }

        /// <summary>
        /// Organizasyon biriminin bağlı
        /// olduğu üst birimin ID bilgisidir.
        ///
        /// null olduğunda üst düzey
        /// birim olarak kabul edilir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir üst birim seçiniz.")]
        [Display(Name = "Üst Birim")]
        public int? ParentUnitId { get; set; }
    }
}
