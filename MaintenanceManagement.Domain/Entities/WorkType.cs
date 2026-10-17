using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkType : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "İş Türü Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<WorkSubType> WorkSubTypes
        { get; set; } = new HashSet<WorkSubType>();

        public ICollection<UnitWorkType> UnitWorkTypes
        { get; set; } = new HashSet<UnitWorkType>();
    }
}
