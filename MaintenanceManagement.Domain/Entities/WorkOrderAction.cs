using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkOrderAction : BaseEntity
    {
        [Required]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        [Required]
        [Display(Name = "İşlemi Yapan Personel")]
        public int PerformedByUserId { get; set; }

        [Required]
        [StringLength(2000)]
        [Display(Name = "Yapılan İşlem")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "İşlem Tarihi")]
        public DateTime PerformedAt { get; set; } =
            DateTime.UtcNow;

        [Range(0, int.MaxValue)]
        [Display(Name = "Çalışma Süresi (Dakika)")]
        public int LaborMinutes { get; set; }

        [StringLength(1000)]
        [Display(Name = "Not")]
        public string? Notes { get; set; }

        public WorkOrder WorkOrder { get; set; } = null!;

        public User PerformedByUser { get; set; } = null!;
    }
}
