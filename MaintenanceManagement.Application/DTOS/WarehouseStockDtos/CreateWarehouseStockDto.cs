using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WarehouseStockDtos
{
    public class CreateWarehouseStockDto
    {
        /// <summary>
        /// Stok kaydının oluşturulacağı
        /// ambarın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir ambar seçiniz.")]
        [Display(Name = "Ambar")]
        public int WarehouseId { get; set; }

        /// <summary>
        /// Stok kaydının oluşturulacağı
        /// malzemenin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir malzeme seçiniz.")]
        [Display(Name = "Malzeme")]
        public int MaterialId { get; set; }

        /// <summary>
        /// Malzemenin başlangıç stok miktarıdır.
        ///
        /// Varsayılan değer sıfırdır.
        /// Pozitif başlangıç miktarı girilirse
        /// stok giriş hareketi oluşturulmalıdır.
        /// </summary>
        [Range(
            typeof(decimal),
            "0",
            "999999999.999",
            ErrorMessage = "Stok miktarı 0 ile 999999999,999 arasında olmalıdır.")]
        [Display(Name = "Başlangıç Stok Miktarı")]
        public decimal Quantity { get; set; } = 0;
    }
}
