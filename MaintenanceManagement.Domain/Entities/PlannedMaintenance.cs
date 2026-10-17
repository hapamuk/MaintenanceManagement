using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class PlannedMaintenance : BaseEntity
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Bakım Planı Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Demirbaş")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Sorumlu Birim")]
        public int UnitId { get; set; }

        [Required]
        [Display(Name = "İş Alt Türü")]
        public int WorkSubTypeId { get; set; }

        [Display(Name = "Oluşturan Kullanıcı")]
        public int? CreatedByUserId { get; set; }

        [Range(1, 3650)]
        [Display(Name = "Bakım Periyodu (Gün)")]
        public int PeriodDays { get; set; }

        [Display(Name = "Son Bakım Tarihi")]
        public DateTime? LastMaintenanceDate { get; set; }

        [Required]
        [Display(Name = "Sonraki Bakım Tarihi")]
        public DateTime NextMaintenanceDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public Asset Asset { get; set; } = null!;

        public Unit Unit { get; set; } = null!;

        public WorkSubType WorkSubType { get; set; } = null!;

        public User? CreatedByUser { get; set; }

        public ICollection<WorkOrder> WorkOrders
        { get; set; } = new HashSet<WorkOrder>();

        public ICollection<MaintenanceChecklist> Checklists
        { get; set; } = new HashSet<MaintenanceChecklist>();
    }
}
