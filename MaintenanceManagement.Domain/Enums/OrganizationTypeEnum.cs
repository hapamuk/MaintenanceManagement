using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    public enum OrganizationTypeEnum
    {
        [Display(Name = "Genel Müdürlük")]
        HeadOffice = 1,

        [Display(Name = "Daire Başkanlığı")]
        DepartmentHeadship = 2,

        [Display(Name = "Şube Müdürlüğü")]
        BranchManagement = 3,

        [Display(Name = "Şeflik")]
        Headship = 4
    }
}
