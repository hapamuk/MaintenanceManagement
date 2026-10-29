using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderMaterialDtos
{
    public class UpdateWorkOrderMaterialDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Malzemenin güncel birim maliyetidir.
        /// </summary>
        [Range(
            typeof(decimal),
            "0",
            "999999999.99",
            ErrorMessage = "Birim maliyet geçerli aralıkta olmalıdır.")]
        [Display(Name = "Birim Maliyet")]
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// Malzeme kullanımıyla ilgili güncel açıklamadır.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
