using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkOrderChecklistResult : BaseEntity
    {
        [Required]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        [Required]
        [Display(Name = "Kontrol Maddesi")]
        public int MaintenanceChecklistId { get; set; }

        [Display(Name = "Kontrol Tamamlandı mı?")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Kontrol Sonucu Uygun mu?")]
        public bool? IsSuccessful { get; set; }

        [Display(Name = "Kontrol Tarihi")]
        public DateTime? CheckedAt { get; set; }

        [Display(Name = "Kontrol Eden Kullanıcı")]
        public int? CheckedByUserId { get; set; }

        [StringLength(1000)]
        [Display(Name = "Açıklama")]
        public string? Notes { get; set; }

        public WorkOrder WorkOrder { get; set; } = null!;

        public MaintenanceChecklist MaintenanceChecklist
        { get; set; } = null!;

        public User? CheckedByUser { get; set; }
    }
}
