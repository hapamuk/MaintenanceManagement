using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WarehouseDtos
{
    public class WarehousePersonnelCandidateDto
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public int UnitId { get; set; }

        public bool IsActive { get; set; }
    }
}
