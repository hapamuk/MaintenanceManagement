using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderActionDtos
{
    public class CreateWorkOrderActionDto
    {
        /// <summary>
        /// İşlemin gerçekleştirildiği iş emrinin
        /// ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş emri seçiniz.")]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// İş emri kapsamında gerçekleştirilen
        /// işlemin ayrıntılı açıklamasıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Yapılan işlem zorunludur.")]
        [StringLength(
            2000,
            ErrorMessage = "Yapılan işlem en fazla 2000 karakter olabilir.")]
        [Display(Name = "Yapılan İşlem")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// İşlem için harcanan çalışma
        /// süresidir (dakika).
        ///
        /// Negatif değer girilemez.
        /// </summary>
        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "Çalışma süresi negatif olamaz.")]
        [Display(Name = "Çalışma Süresi (Dakika)")]
        public int LaborMinutes { get; set; }

        /// <summary>
        /// Gerçekleştirilen işlemle ilgili
        /// ek not ve açıklamalardır.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Not en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama / Not")]
        public string? Notes { get; set; }
    }
}
