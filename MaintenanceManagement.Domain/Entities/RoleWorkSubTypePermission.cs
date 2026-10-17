using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    //Bir rolün hangi iş alt türünde hangi işlemleri yapabileceğini belirler.
    public class RoleWorkSubTypePermission : BaseEntity
    {
        [Required]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }

        [Required]
        [Display(Name = "İş Alt Türü")]
        public int WorkSubTypeId { get; set; }

        [Required]
        [Display(Name = "Yetki")]
        public int PermissionId { get; set; }

        public bool IsActive { get; set; } = true;

        public Role Role { get; set; } = null!;

        public WorkSubType WorkSubType { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
    }
}
