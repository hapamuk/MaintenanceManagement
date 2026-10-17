using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class AssetUser : BaseEntity
    {
        [Required]
        [Display(Name = "Demirbaş")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Zimmetli Kullanıcı")]
        public int UserId { get; set; }

        [Display(Name = "Zimmet Veren")]
        public int? AssignedByUserId { get; set; }

        [Required]
        [Display(Name = "Zimmet Başlangıç")]
        public DateTime StartDate { get; set; } =
            DateTime.UtcNow;

        [Display(Name = "Zimmet Bitiş")]
        public DateTime? EndDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public Asset Asset { get; set; } = null!;

        public User User { get; set; } = null!;

        public User? AssignedByUser { get; set; }
    }
}
