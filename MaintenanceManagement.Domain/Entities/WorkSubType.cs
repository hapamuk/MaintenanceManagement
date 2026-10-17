using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkSubType : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "İş Alt Türü Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "İş Türü")]
        public int WorkTypeId { get; set; }

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties

        public WorkType WorkType { get; set; } = null!;

        public ICollection<RoleWorkSubTypePermission> RolePermissions
        { get; set; } =
                new HashSet<RoleWorkSubTypePermission>();

        public ICollection<WorkRequest> WorkRequests
        { get; set; } =
                new HashSet<WorkRequest>();

        public ICollection<WorkOrder> WorkOrders
        { get; set; } =
                new HashSet<WorkOrder>();

        public ICollection<PlannedMaintenance> PlannedMaintenances
        { get; set; } =
                new HashSet<PlannedMaintenance>();
    }
}
