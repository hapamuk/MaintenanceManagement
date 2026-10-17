using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    public enum WorkStatusEnum
    {
        [Display(Name = "Yeni")]
        New = 1,

        [Display(Name = "İptal Edildi")]
        Cancelled = 2,

        [Display(Name = "İşe Başlandı")]
        InProgress = 3,

        [Display(Name = "Onaya Gönderildi")]
        PendingApproval = 4,

        [Display(Name = "Tamamlandı")]
        Completed = 5
    }
}
