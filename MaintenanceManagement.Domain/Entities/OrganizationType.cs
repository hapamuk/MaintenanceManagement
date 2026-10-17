using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class OrganizationType : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Organizasyon Türü")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Organizasyon Türü Kodu")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Unit> Units { get; set; } =
            new HashSet<Unit>();
    }
}
