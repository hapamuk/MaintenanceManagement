using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    public enum AssetMovementTypeEnum
    {
        [Display(Name = "İlk Kayıt")]
        Registration = 1,

        [Display(Name = "Zimmet")]
        Assignment = 2,

        [Display(Name = "Devir")]
        Transfer = 3,

        [Display(Name = "Zimmet İadesi")]
        Return = 4,

        [Display(Name = "Hurdaya Ayırma")]
        Scrap = 5
    }
}
