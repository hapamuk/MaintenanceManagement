using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class MaterialCategoryUnit : BaseEntity
    {
        [Required]
        [Display(Name = "Malzeme Kategorisi")]
        public int MaterialCategoryId { get; set; }

        [Required]
        [Display(Name = "Organizasyon Birimi")]
        public int UnitId { get; set; }

        // =====================================
        // NAVIGATION PROPERTIES
        // =====================================

        public MaterialCategory MaterialCategory
        { get; set; } = null!;

        public Unit Unit
        { get; set; } = null!;
    }
}
