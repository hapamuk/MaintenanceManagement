using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class StockMovement : BaseEntity
    {
        [Required]
        [Display(Name = "Ambar")]
        public int WarehouseId { get; set; }

        [Required]
        [Display(Name = "Malzeme")]
        public int MaterialId { get; set; }

        [Required]
        [Display(Name = "İşlemi Yapan Kullanıcı")]
        public int PerformedByUserId { get; set; }

        [Display(Name = "Hareket Türü")]
        public StockMovementTypeEnum MovementType { get; set; } =
            StockMovementTypeEnum.In;

        [Range(typeof(decimal), "0.001", "999999999.999")]
        [Display(Name = "Miktar")]
        public decimal Quantity { get; set; }

        [Display(Name = "Hareket Tarihi")]
        public DateTime MovementDate { get; set; } =
            DateTime.UtcNow;

        [StringLength(100)]
        [Display(Name = "Referans Numarası")]
        public string? ReferenceNumber { get; set; }

        [Display(Name = "Transfer Grup Numarası")]
        public Guid? TransferGroupId { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public Warehouse Warehouse { get; set; } = null!;

        public Material Material { get; set; } = null!;

        public User PerformedByUser { get; set; } = null!;
    }
}
