using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkRequestActionDtos
{
    public class UpdateWorkRequestActionDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// İşlem kaydının güncel açıklamasıdır.
        /// </summary>
        [StringLength(
            2000,
            ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
        [Display(Name = "İşlem Açıklaması")]
        public string? Description { get; set; }
    }
}
