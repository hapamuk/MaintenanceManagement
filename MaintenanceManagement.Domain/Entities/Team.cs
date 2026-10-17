using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class Team : BaseEntity
    {
        [Required]
        [Display(Name = "Organizasyon")]
        public int UnitId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Takım Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Takım Kodu")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties

        public Unit Unit { get; set; } = null!;

        public ICollection<TeamMember> TeamMembers
        { get; set; } = new HashSet<TeamMember>();

        public ICollection<WorkOrder> WorkOrders
        { get; set; } = new HashSet<WorkOrder>();
    }
}
