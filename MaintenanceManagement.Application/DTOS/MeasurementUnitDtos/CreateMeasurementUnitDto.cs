using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MeasurementUnitDtos
{
    public class CreateMeasurementUnitDto
    {
        /// <summary>
        /// Ölçü biriminin adıdır.
        ///
        /// Örnek: Adet, Kilogram, Metre, Litre.
        /// </summary>
        [Required(
            ErrorMessage = "Ölçü birimi adı zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Ölçü birimi adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Ölçü Birimi")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin kısa gösterimidir.
        ///
        /// Örnek: Adet, kg, m, L.
        /// </summary>
        [Required(
            ErrorMessage = "Ölçü birimi kısaltması zorunludur.")]
        [StringLength(
            20,
            ErrorMessage = "Kısaltma en fazla 20 karakter olabilir.")]
        [Display(Name = "Kısaltma")]
        public string Symbol { get; set; } = string.Empty;
    }
}
