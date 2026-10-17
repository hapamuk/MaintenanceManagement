using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    public enum AssignmentRoleEnum
    {
        [Display(Name = "Şef")]
        Responsible = 1,

        [Display(Name = "Usta Başı")]
        Assistant = 2
    }
}
