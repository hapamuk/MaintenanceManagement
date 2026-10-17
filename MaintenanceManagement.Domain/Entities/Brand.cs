using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class Brand : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Marka Adı")]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<AssetModel> Models { get; set; } =
            new HashSet<AssetModel>();
    }
}
