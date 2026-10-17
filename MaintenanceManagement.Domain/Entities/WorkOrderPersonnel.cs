using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    //çıkarılacak olabilir 
    public class WorkOrderPersonnel : BaseEntity
    {
        [Required]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        [Required]
        [Display(Name = "Takım Üyesi")]
        public int TeamMemberId { get; set; }

        [Display(Name = "Görev Türü")]
        public AssignmentRoleEnum AssignmentRole { get; set; } =
            AssignmentRoleEnum.Responsible;

        [Required]
        [Display(Name = "Atayan Kullanıcı")]
        public int AssignedByUserId { get; set; }

        [Display(Name = "Atanma Tarihi")]
        public DateTime AssignedAt { get; set; } =
            DateTime.UtcNow;

        [Display(Name = "Görevden Ayrılma Tarihi")]
        public DateTime? ReleasedAt { get; set; }

        [Display(Name = "Aktif Görev")]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        public WorkOrder WorkOrder { get; set; } = null!;

        public TeamMember TeamMember { get; set; } = null!;

        public User AssignedByUser { get; set; } = null!;
    }
}
