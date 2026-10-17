using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class Permission : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Yetki Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Yetki Kodu")]
        public string Code { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Modül")]
        public string Module { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<RolePermission> RolePermissions
        { get; set; } = new HashSet<RolePermission>();

        public ICollection<RoleWorkSubTypePermission> WorkSubTypePermissions
        { get; set; } = new HashSet<RoleWorkSubTypePermission>();
    }
}
