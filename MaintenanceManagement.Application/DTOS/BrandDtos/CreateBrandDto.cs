using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.BrandDtos
{
    public class CreateBrandDto
    {
        /// <summary>
        /// Oluşturulacak markanın adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Marka adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Marka adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Marka Adı")]
        public string Name { get; set; } = string.Empty;
    }
}
