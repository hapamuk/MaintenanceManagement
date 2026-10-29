using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderStatusHistoryDtos
{
    public class UpdateWorkOrderStatusHistoryDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Durum değişikliğiyle ilgili
        /// güncel açıklama bilgisidir.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
