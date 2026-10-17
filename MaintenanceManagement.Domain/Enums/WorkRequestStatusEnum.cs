using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    public enum WorkRequestStatusEnum
    {
        [Display(Name = "Yeni")]
        Draft = 1,


        [Display(Name = "Reddedildi")]
        Rejected = 2,

        [Display(Name = "İş Emrine Dönüştürüldü")]
        ConvertedToWorkOrder = 3,

        [Display(Name = "İptal Edildi")]
        Cancelled = 4
    }
}
