using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    public enum AssetStatusEnum
    {
        [Display(Name = "Aktif")]
        Active = 1,

        [Display(Name = "Kullanım Dışı")]
        OutOfService = 2,

        [Display(Name = "Hurda")]
        Scrapped = 3
    }
}
