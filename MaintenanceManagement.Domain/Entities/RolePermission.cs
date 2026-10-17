using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        [Required]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }

        [Required]
        [Display(Name = "Yetki")]
        public int PermissionId { get; set; }
        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;
        public Role Role { get; set; } = null!;


        public Permission Permission { get; set; } = null!;
    }
}
