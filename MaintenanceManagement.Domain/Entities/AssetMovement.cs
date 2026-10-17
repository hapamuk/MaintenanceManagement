using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class AssetMovement : BaseEntity
    {
        [Required]
        [Display(Name = "Demirbaş")]
        public int AssetId { get; set; }

        [Display(Name = "Önceki Birim")]
        public int? FromUnitId { get; set; }

        [Display(Name = "Yeni Birim")]
        public int? ToUnitId { get; set; }

        [Display(Name = "Önceki Kullanıcı")]
        public int? FromUserId { get; set; }

        [Display(Name = "Yeni Kullanıcı")]
        public int? ToUserId { get; set; }

        [Required]
        [Display(Name = "İşlemi Yapan")]
        public int PerformedByUserId { get; set; }

        [Display(Name = "Hareket Türü")]
        public AssetMovementTypeEnum MovementType { get; set; }

        [Display(Name = "Hareket Tarihi")]
        public DateTime MovementDate { get; set; } =
            DateTime.UtcNow;

        [StringLength(1000)]
        public string? Description { get; set; }

        public Asset Asset { get; set; } = null!;

        public Unit? FromUnit { get; set; }

        public Unit? ToUnit { get; set; }

        public User? FromUser { get; set; }

        public User? ToUser { get; set; }

        public User PerformedByUser { get; set; } = null!;
    }
}
