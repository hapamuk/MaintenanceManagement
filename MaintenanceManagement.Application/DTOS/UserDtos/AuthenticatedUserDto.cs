using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class AuthenticatedUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string FullName { get; set; } = "";
        public bool IsActive { get; set; }
        public bool HasSystemAccess { get; set; }

        public bool IsSystemAdmin { get; set; }
    }
}
