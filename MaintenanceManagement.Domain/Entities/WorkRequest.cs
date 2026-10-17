using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkRequest : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Talep Numarası")]
        public string RequestNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Talep Başlığı")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Talep Açıklaması")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Demirbaş")]
        public int? AssetId { get; set; }

        [Required]
        [Display(Name = "Talep Eden Kullanıcı")]
        public int RequestedByUserId { get; set; }

        [Required]
        [Display(Name = "Talep Eden Birim")]
        public int RequesterUnitId { get; set; }

        [Required]
        [Display(Name = "Talebi Karşılayacak Birim")]
        public int ResponsibleUnitId { get; set; }

        [Required]
        [Display(Name = "İş Alt Türü")]
        public int WorkSubTypeId { get; set; }

        [Display(Name = "Seçilen Yönetici")]
        public int? ManagerUserId { get; set; }

        [Display(Name = "Konum")]
        public int? SiteLocationId { get; set; }

        [StringLength(500)]
        [Display(Name = "Konum Detayı")]
        public string? LocationDescription { get; set; }

        [Display(Name = "Öncelik")]
        public WorkPriorityEnum Priority { get; set; } =
            WorkPriorityEnum.Normal;

        [Display(Name = "Talep Durumu")]
        public WorkRequestStatusEnum Status { get; set; } =
            WorkRequestStatusEnum.Draft;

        [Display(Name = "Talep Tarihi")]
        public DateTime RequestDate { get; set; } =
            DateTime.UtcNow;

        [Display(Name = "Gönderilme Tarihi")]
        public DateTime? SubmittedAt { get; set; }

        public Asset? Asset { get; set; }

        public User RequestedByUser { get; set; } = null!;

        public Unit RequesterUnit { get; set; } = null!;

        public Unit ResponsibleUnit { get; set; } = null!;

        public User? ManagerUser { get; set; }

        public WorkSubType WorkSubType { get; set; } = null!;

        public SiteLocation? SiteLocation { get; set; }

        public WorkOrder? WorkOrder { get; set; }

        public ICollection<WorkRequestFile> Files
        { get; set; } = new HashSet<WorkRequestFile>();

        public ICollection<WorkRequestAction> Actions
        { get; set; } = new HashSet<WorkRequestAction>();
    }
}
