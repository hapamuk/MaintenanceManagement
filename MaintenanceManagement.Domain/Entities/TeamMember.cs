using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class TeamMember : BaseEntity
    {
        [Required]
        [Display(Name = "Takım")]
        public int TeamId { get; set; }

        [Required]
        [Display(Name = "Personel")]
        public int UserId { get; set; }

        [Display(Name = "Takıma Katılma Tarihi")]
        public DateTime JoinedAt { get; set; } =
            DateTime.UtcNow;

        [Display(Name = "Takımdan Ayrılma Tarihi")]
        public DateTime? LeftAt { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;

        public Team Team { get; set; } = null!;

        public User User { get; set; } = null!;

        public ICollection<WorkOrderPersonnel> WorkOrderAssignments
        { get; set; } = new HashSet<WorkOrderPersonnel>();
    }
}
