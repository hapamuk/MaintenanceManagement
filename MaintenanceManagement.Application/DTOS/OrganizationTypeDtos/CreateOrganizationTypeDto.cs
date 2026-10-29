using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.OrganizationTypeDtos
{
    public class CreateOrganizationTypeDto
    {
        /// <summary>
        /// Organizasyon türünün adını belirtir.
        /// </summary>
        [Required(ErrorMessage = "Organizasyon türü adı zorunludur.")]
        [Display(Name = "Organizasyon Türü Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon türünü sistem içerisinde
        /// benzersiz olarak tanımlayan kod.
        /// </summary>
        [Required(ErrorMessage = "Organizasyon türü kodu zorunludur.")]
        [Display(Name = "Organizasyon Türü Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon türü hakkında
        /// isteğe bağlı açıklama bilgisi.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
