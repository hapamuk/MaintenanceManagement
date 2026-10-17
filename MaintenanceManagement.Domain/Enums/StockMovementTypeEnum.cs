using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Enums
{
    public enum StockMovementTypeEnum
    {
        [Display(Name = "Stok Girişi")]
        In = 1,

        [Display(Name = "Stok Çıkışı")]
        Out = 2,

  

    }
}
