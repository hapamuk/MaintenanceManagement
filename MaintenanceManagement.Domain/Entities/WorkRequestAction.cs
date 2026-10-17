using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkRequestAction : BaseEntity
    {
        [Required]
        [Display(Name = "İş Talebi")]
        public int WorkRequestId { get; set; }

        [Required]
        [Display(Name = "İşlemi Yapan Kullanıcı")]
        public int PerformedByUserId { get; set; }

        [Display(Name = "İşlem Türü")]
        public WorkRequestActionTypeEnum ActionType { get; set; }

        [StringLength(2000)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "İşlem Tarihi")]
        public DateTime PerformedAt { get; set; } =
            DateTime.UtcNow;

        public WorkRequest WorkRequest { get; set; } = null!;

        public User PerformedByUser { get; set; } = null!;
    }
}
