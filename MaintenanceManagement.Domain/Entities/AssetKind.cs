using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class AssetKind : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Cins Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Kategori")]
        public int AssetCategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public AssetCategory AssetCategory { get; set; } = null!;

        public ICollection<Asset> Assets { get; set; } =
            new HashSet<Asset>();
    }
}
