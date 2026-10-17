using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class UserUnitRole : BaseEntity
    {
        [Required]
        [Display(Name = "Kullanıcı")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }

        [Display(Name = "Atanma Tarihi")]
        public DateTime AssignedAt { get; set; } =
            DateTime.UtcNow;

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;

        public User User { get; set; } = null!;

        public Role Role { get; set; } = null!;
    }
}
