using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    //Talep değerlendirme sürecindeki işlemleri belirtir.
    public enum WorkRequestActionTypeEnum
    {
        [Display(Name = "Talep Gönderildi")]
        Submitted = 1,

        [Display(Name = "İş Emrine Dönüştürüldü")]
        Converted = 2,

        [Display(Name = "Talep Reddedildi")]
        Rejected = 3,

        [Display(Name = "Talep İptal Edildi")]
        Cancelled = 4
    }
}
