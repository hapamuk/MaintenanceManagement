using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class UserAssignedRoleDto
    {
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; } = "";

        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
    }
}
