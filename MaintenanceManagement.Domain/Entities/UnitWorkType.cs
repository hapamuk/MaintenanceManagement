using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class UnitWorkType : BaseEntity
    {
        [Required]
        [Display(Name = "Organizasyon")]
        public int UnitId { get; set; }

        [Required]
        [Display(Name = "İş Türü")]
        public int WorkTypeId { get; set; }

        public bool IsActive { get; set; } = true;

        public Unit Unit { get; set; } = null!;

        public WorkType WorkType { get; set; } = null!;
    }
}
