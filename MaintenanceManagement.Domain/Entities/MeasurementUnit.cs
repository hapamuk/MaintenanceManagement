using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class MeasurementUnit : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Ölçü Birimi")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Kısaltma")]
        public string Symbol { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Material> Materials { get; set; } =
            new HashSet<Material>();
    }
}
