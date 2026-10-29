using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MeasurementUnitDtos
{
    public class UpdateMeasurementUnitDto
    {
       
        public int Id { get; set; }

        /// <summary>
        /// Ölçü biriminin güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Ölçü birimi adı zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Ölçü birimi adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Ölçü Birimi")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin güncel kısaltmasıdır.
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
