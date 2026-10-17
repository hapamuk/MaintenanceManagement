using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        [StringLength(100)]
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        [Display(Name = "Şifre Özeti")]
        public string? PasswordHash { get; set; }

        [StringLength(20)]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [StringLength(150)]
        [Display(Name = "Unvan")]
        public string? Title { get; set; }

        [Required]
        [Display(Name = "Asıl Birim")]
        public int PrimaryUnitId { get; set; }

        [Display(Name = "Sistem Giriş Yetkisi")]
        public bool HasSystemAccess { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;

        public Unit PrimaryUnit { get; set; } = null!;

        public ICollection<UserUnitRole> UserUnitRoles
        { get; set; } = new HashSet<UserUnitRole>();

        public ICollection<TeamMember> TeamMemberships
        { get; set; } = new HashSet<TeamMember>();

        public ICollection<AssetUser> AssetAssignments
        { get; set; } = new HashSet<AssetUser>();

        public ICollection<AssetUser> AssignmentsMade
        { get; set; } = new HashSet<AssetUser>();

        public ICollection<AssetMovement> AssetMovementsPerformed
        { get; set; } = new HashSet<AssetMovement>();

        public ICollection<AssetMovement> OutgoingUserAssetMovements
        { get; set; } = new HashSet<AssetMovement>();

        public ICollection<AssetMovement> IncomingUserAssetMovements
        { get; set; } = new HashSet<AssetMovement>();

        public ICollection<WorkRequest> CreatedWorkRequests
        { get; set; } = new HashSet<WorkRequest>();

        public ICollection<WorkRequest> ManagedWorkRequests
        { get; set; } = new HashSet<WorkRequest>();

        public ICollection<WorkRequestAction> WorkRequestActions
        { get; set; } = new HashSet<WorkRequestAction>();

        public ICollection<WorkRequestFile> UploadedRequestFiles
        { get; set; } = new HashSet<WorkRequestFile>();

        public ICollection<WorkOrder> CreatedWorkOrders
        { get; set; } = new HashSet<WorkOrder>();

        public ICollection<WorkOrder> ApprovedWorkOrders
        { get; set; } = new HashSet<WorkOrder>();

        public ICollection<WorkOrderPersonnel> PersonnelAssignmentsMade
        { get; set; } = new HashSet<WorkOrderPersonnel>();

        public ICollection<WorkOrderAction> WorkOrderActions
        { get; set; } = new HashSet<WorkOrderAction>();

        public ICollection<WorkOrderStatusHistory> WorkOrderStatusChanges
        { get; set; } = new HashSet<WorkOrderStatusHistory>();

        public ICollection<WorkOrderFile> UploadedWorkOrderFiles
        { get; set; } = new HashSet<WorkOrderFile>();

        public ICollection<PlannedMaintenance> CreatedMaintenancePlans
        { get; set; } = new HashSet<PlannedMaintenance>();

        public ICollection<WorkOrderChecklistResult> ChecklistResults
        { get; set; } = new HashSet<WorkOrderChecklistResult>();

        public ICollection<StockMovement> StockMovements
        { get; set; } = new HashSet<StockMovement>();
    }
}
