using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.StockMovementDtos
{
    public class CreateStockMovementDto
    {
        /// <summary>
        /// Stok hareketinin gerçekleştiği ambarın ID bilgisidir.
        /// </summary>
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir ambar seçiniz.")]
        [Display(Name = "Ambar")]
        public int WarehouseId { get; set; }

        /// <summary>
        /// Stok hareketine konu olan malzemenin ID bilgisidir.
        /// </summary>
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir malzeme seçiniz.")]
        [Display(Name = "Malzeme")]
        public int MaterialId { get; set; }

        /// <summary>
        /// Stok hareketinin türüdür.
        /// In = 1: Stok Girişi
        /// Out = 2: Stok Çıkışı
        /// </summary>
        [EnumDataType(typeof(StockMovementTypeEnum),
            ErrorMessage = "Geçerli bir stok hareket türü seçiniz.")]
        [Display(Name = "Hareket Türü")]
        public StockMovementTypeEnum MovementType { get; set; }

        /// <summary>
        /// Stok giriş veya çıkış miktarıdır.
        /// Her iki işlemde de pozitif miktar gönderilir.
        /// </summary>
        [Range(typeof(decimal), "0.001", "999999999.999",
            ErrorMessage = "Miktar sıfırdan büyük olmalıdır.")]
        [Display(Name = "Hareket Miktarı")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Fiş, belge veya işlem referans numarasıdır.
        /// </summary>
        [StringLength(100,
            ErrorMessage = "Referans numarası en fazla 100 karakter olabilir.")]
        [Display(Name = "Referans Numarası")]
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Stok hareketine ait açıklamadır.
        /// </summary>
        [StringLength(1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
