using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WarehousePersonnel : BaseEntity
    {
        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int UserId { get; set; }

        public Warehouse Warehouse { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
