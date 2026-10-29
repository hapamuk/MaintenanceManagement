using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderFilesDtos
{
    public class UpdateWorkOrderFileDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Dosyanın güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Dosya adı zorunludur.")]
        [StringLength(
            255,
            ErrorMessage = "Dosya adı en fazla 255 karakter olabilir.")]
        [Display(Name = "Dosya Adı")]
        public string FileName { get; set; } = string.Empty;
    }
}
