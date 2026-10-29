using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.StockMovementDtos
{
    public class UpdateStockMovementDto
    {

       
            public int Id { get; set; }

            /// <summary>
            /// Stok hareketinin güncel referans numarasıdır.
            /// </summary>
            [StringLength(100,
                ErrorMessage = "Referans numarası en fazla 100 karakter olabilir.")]
            [Display(Name = "Referans Numarası")]
            public string? ReferenceNumber { get; set; }

            /// <summary>
            /// Stok hareketinin güncel açıklamasıdır.
            /// </summary>
            [StringLength(1000,
                ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
            [Display(Name = "Açıklama")]
            public string? Description { get; set; }
        }
    }
