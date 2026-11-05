using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class UserRoleAssignmentDto
    {
        [Range(1, int.MaxValue)]
        public int UnitId { get; set; }

        [Range(1, int.MaxValue)]
        public int RoleId { get; set; }
    }

}
