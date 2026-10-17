using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class SiteLocation : BaseEntity
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Konum Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        [StringLength(100)]
        [Display(Name = "Bina / Blok")]
        public string? Building { get; set; }

        [StringLength(50)]
        [Display(Name = "Kat")]
        public string? Floor { get; set; }

        [Range(typeof(decimal), "-90", "90")]
        public decimal? Latitude { get; set; }

        [Range(typeof(decimal), "-180", "180")]
        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<WorkRequest> WorkRequests
        { get; set; } = new HashSet<WorkRequest>();
    }
}
