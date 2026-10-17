using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class Role : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Rol Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Rol Kodu")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Organizasyon")]
        public int UnitId { get; set; }

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public Unit Unit { get; set; } = null!;

        public ICollection<UserUnitRole> UserUnitRoles
        { get; set; } = new HashSet<UserUnitRole>();

        public ICollection<RolePermission> RolePermissions
        { get; set; } = new HashSet<RolePermission>();

        public ICollection<RoleWorkSubTypePermission> WorkSubTypePermissions
        { get; set; } = new HashSet<RoleWorkSubTypePermission>();
    }
}
