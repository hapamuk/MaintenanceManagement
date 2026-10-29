using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderActionDtos
{
    public class UpdateWorkOrderActionDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// İş emri kapsamında gerçekleştirilen
        /// işlemin güncel açıklamasıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Yapılan işlem zorunludur.")]
        [StringLength(
            2000,
            ErrorMessage = "Yapılan işlem en fazla 2000 karakter olabilir.")]
        [Display(Name = "Yapılan İşlem")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// İşlem için harcanan güncel
        /// çalışma süresidir (dakika).
        /// </summary>
        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "Çalışma süresi negatif olamaz.")]
        [Display(Name = "Çalışma Süresi (Dakika)")]
        public int LaborMinutes { get; set; }

        /// <summary>
        /// İşlemle ilgili güncellenebilir
        /// ek not ve açıklamalardır.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Not en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama / Not")]
        public string? Notes { get; set; }
    }
}
