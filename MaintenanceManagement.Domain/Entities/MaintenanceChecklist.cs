using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class MaintenanceChecklist : BaseEntity
    {
        [Required]
        [Display(Name = "Planlı Bakım")]
        public int PlannedMaintenanceId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Kontrol Başlığı")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Sıra")]
        public int SequenceNumber { get; set; } = 1;

        public bool IsRequired { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public PlannedMaintenance PlannedMaintenance
        { get; set; } = null!;

        public ICollection<WorkOrderChecklistResult> Results
        { get; set; } = new HashSet<WorkOrderChecklistResult>();
    }
}
