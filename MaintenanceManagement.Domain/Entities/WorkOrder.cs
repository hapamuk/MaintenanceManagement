using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkOrder : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "İş Emri Numarası")]
        public string WorkOrderNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "İş Emri Başlığı")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Demirbaş")]
        public int? AssetId { get; set; }

        [Display(Name = "İş Talebi")]
        public int? WorkRequestId { get; set; }

        [Display(Name = "Planlı Bakım")]
        public int? PlannedMaintenanceId { get; set; }

        [Required]
        [Display(Name = "Sorumlu Organizasyon")]
        public int ResponsibleUnitId { get; set; }

        [Required]
        [Display(Name = "Görevli Takım")]
        public int TeamId { get; set; }

        [Required]
        [Display(Name = "İş Alt Türü")]
        public int WorkSubTypeId { get; set; }

        [Required]
        [Display(Name = "Oluşturan Kullanıcı")]
        public int CreatedByUserId { get; set; }

        [Display(Name = "Onaylayan Kullanıcı")]
        public int? ApprovedByUserId { get; set; }

        [Display(Name = "İş Durumu")]
        public WorkStatusEnum Status { get; set; } =
            WorkStatusEnum.New;

        [Display(Name = "Öncelik")]
        public WorkPriorityEnum Priority { get; set; } =
            WorkPriorityEnum.Normal;

        [Display(Name = "Planlanan Başlangıç")]
        public DateTime? PlannedStartDate { get; set; }

        [Display(Name = "Planlanan Bitiş")]
        public DateTime? PlannedEndDate { get; set; }

        [Display(Name = "İşe Başlama Tarihi")]
        public DateTime? StartedAt { get; set; }

        [Display(Name = "Onaya Gönderilme Tarihi")]
        public DateTime? SubmittedForApprovalAt { get; set; }

        [Display(Name = "Tamamlanma Tarihi")]
        public DateTime? CompletedAt { get; set; }

        [Display(Name = "İptal Tarihi")]
        public DateTime? CancelledAt { get; set; }

        [StringLength(2000)]
        [Display(Name = "Sonuç Açıklaması")]
        public string? ResultDescription { get; set; }

        public Asset? Asset { get; set; }

        public WorkRequest? WorkRequest { get; set; }

        public PlannedMaintenance? PlannedMaintenance { get; set; }

        public Unit ResponsibleUnit { get; set; } = null!;

        public Team Team { get; set; } = null!;

        public WorkSubType WorkSubType { get; set; } = null!;

        public User CreatedByUser { get; set; } = null!;

        public User? ApprovedByUser { get; set; }

        public ICollection<WorkOrderPersonnel> Personnel
        { get; set; } = new HashSet<WorkOrderPersonnel>();

        public ICollection<WorkOrderMaterial> Materials
        { get; set; } = new HashSet<WorkOrderMaterial>();

        public ICollection<WorkOrderAction> Actions
        { get; set; } = new HashSet<WorkOrderAction>();

        public ICollection<WorkOrderStatusHistory> StatusHistory
        { get; set; } = new HashSet<WorkOrderStatusHistory>();

        public ICollection<WorkOrderFile> Files
        { get; set; } = new HashSet<WorkOrderFile>();

        public ICollection<WorkOrderChecklistResult> ChecklistResults
        { get; set; } = new HashSet<WorkOrderChecklistResult>();
    }
}
