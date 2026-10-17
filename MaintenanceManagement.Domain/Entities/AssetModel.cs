using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class AssetModel : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Model Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Marka")]
        public int BrandId { get; set; }

        public bool IsActive { get; set; } = true;

        public Brand Brand { get; set; } = null!;

        public ICollection<Asset> Assets { get; set; } =
            new HashSet<Asset>();
    }
}
