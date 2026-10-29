using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.OrganizationTypeDtos
{
    public class UpdateOrganizationTypeDto
    {
       
        public int Id { get; set; }

        /// <summary>
        /// Organizasyon türünün adı.
        /// </summary>
        [Required(ErrorMessage = "Organizasyon türü adı zorunludur.")]
        [Display(Name = "Organizasyon Türü Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon türünün benzersiz kodu.
        /// </summary>
        [Required(ErrorMessage = "Organizasyon türü kodu zorunludur.")]
        [Display(Name = "Organizasyon Türü Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon türünün açıklaması.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
