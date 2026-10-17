using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkOrderStatusHistory : BaseEntity
    {
        [Required]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        [Display(Name = "Önceki Durum")]
        public WorkStatusEnum? PreviousStatus { get; set; }

        [Display(Name = "Yeni Durum")]
        public WorkStatusEnum NewStatus { get; set; }

        [Required]
        [Display(Name = "İşlemi Yapan")]
        public int ChangedByUserId { get; set; }

        [Display(Name = "Değişiklik Tarihi")]
        public DateTime ChangedAt { get; set; } =
            DateTime.UtcNow;

        [StringLength(1000)]
        public string? Description { get; set; }

        public WorkOrder WorkOrder { get; set; } = null!;

        public User ChangedByUser { get; set; } = null!;
    }
}
