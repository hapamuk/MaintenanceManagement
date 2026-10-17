using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class Asset : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Demirbaş Kodu")]
        public string AssetCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Demirbaş Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(150)]
        [Display(Name = "Seri Numarası")]
        public string? SerialNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Envanter Numarası")]
        public string? InventoryNumber { get; set; }

        [Required]
        [Display(Name = "Birim")]
        public int UnitId { get; set; }

        [Required]
        [Display(Name = "Model")]
        public int AssetModelId { get; set; }

        [Required]
        [Display(Name = "Demirbaş Cinsi")]
        public int AssetKindId { get; set; }

        [Display(Name = "Alım Tarihi")]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Garanti Bitiş Tarihi")]
        public DateTime? WarrantyEndDate { get; set; }

        [Display(Name = "Durum")]
        public AssetStatusEnum Status { get; set; } =
            AssetStatusEnum.Active;

        [StringLength(1000)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        public Unit Unit { get; set; } = null!;

        public AssetModel AssetModel { get; set; } = null!;

        public AssetKind AssetKind { get; set; } = null!;

        public ICollection<AssetUser> AssetUsers
        { get; set; } = new HashSet<AssetUser>();

        public ICollection<AssetMovement> AssetMovements
        { get; set; } = new HashSet<AssetMovement>();

        public ICollection<WorkRequest> WorkRequests
        { get; set; } = new HashSet<WorkRequest>();

        public ICollection<WorkOrder> WorkOrders
        { get; set; } = new HashSet<WorkOrder>();

        public ICollection<PlannedMaintenance> PlannedMaintenances
        { get; set; } = new HashSet<PlannedMaintenance>();
    }
}
