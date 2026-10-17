using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    //öncelik sıralaması
    public enum WorkPriorityEnum
    {
        [Display(Name = "Düşük")]
        Low = 1,

        [Display(Name = "Normal")]
        Normal = 2,

        [Display(Name = "Yüksek")]
        High = 3,

        [Display(Name = "Acil")]
        Critical = 4
    }
}
