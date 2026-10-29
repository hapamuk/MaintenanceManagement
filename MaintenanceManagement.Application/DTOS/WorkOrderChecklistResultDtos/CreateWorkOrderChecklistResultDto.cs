using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderChecklistResultDtos
{
    public class CreateWorkOrderChecklistResultDto
    {
        /// <summary>
        /// Kontrol kaydının bağlı olduğu
        /// iş emrinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş emri seçiniz.")]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// Kontrolü yapılacak bakım kontrol
        /// maddesinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir kontrol maddesi seçiniz.")]
        [Display(Name = "Bakım Kontrol Maddesi")]
        public int MaintenanceChecklistId { get; set; }

        /// <summary>
        /// Kontrol maddesine ilişkin
        /// isteğe bağlı açıklama bilgisidir.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Notes { get; set; }
    }
}
