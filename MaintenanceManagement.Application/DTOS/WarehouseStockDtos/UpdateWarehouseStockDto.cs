using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WarehouseStockDtos
{
    public class UpdateWarehouseStockDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Stok düzeltmesi sonucunda
        /// geçerli olacak yeni miktardır.
        /// </summary>
        [Range(
            typeof(decimal),
            "0",
            "999999999.999",
            ErrorMessage = "Stok miktarı 0 ile 999999999,999 arasında olmalıdır.")]
        [Display(Name = "Yeni Stok Miktarı")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Eş zamanlı güncellemelerde
        /// veri tutarlılığını koruyan
        /// concurrency token bilgisidir.
        ///
        /// Kayıt veritabanından okunurken
        /// alınan RowVersion değeri gönderilmelidir.
        /// </summary>
        [Required(
            ErrorMessage = "Stok sürüm bilgisi zorunludur.")]
        [MinLength(
            1,
            ErrorMessage = "Geçerli stok sürüm bilgisi gönderilmelidir.")]
        [Display(Name = "Stok Sürüm Bilgisi")]
        public byte[] RowVersion { get; set; }
            = Array.Empty<byte>();
    }
}
