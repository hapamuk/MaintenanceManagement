using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderChecklistResultDtos
{
    public class UpdateWorkOrderChecklistResultDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// İlgili kontrol işleminin
        /// tamamlanıp tamamlanmadığını belirtir.
        /// </summary>
        [Display(Name = "Kontrol Tamamlandı")]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gerçekleştirilen kontrolün sonucudur.
        ///
        /// true  : Uygun
        /// false : Uygun Değil
        /// null  : Sonuç Belirtilmedi
        /// </summary>
        [Display(Name = "Kontrol Uygunluk Durumu")]
        public bool? IsSuccessful { get; set; }

        /// <summary>
        /// Yapılan kontrol hakkında
        /// açıklama ve not bilgisidir.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Kontrol Açıklaması")]
        public string? Notes { get; set; }
    }
}
