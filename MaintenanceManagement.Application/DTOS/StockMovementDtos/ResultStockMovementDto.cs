using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.StockMovementDtos
{
    public class ResultStockMovementDto
    {
        /// <summary>
        /// Stok hareketinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Stok Hareketi ID")]
        public int Id { get; set; }

        /// <summary>
        /// Ambarın ID bilgisidir.
        /// </summary>
        [Display(Name = "Ambar ID")]
        public int WarehouseId { get; set; }

        /// <summary>
        /// Ambarın kodudur.
        /// </summary>
        [Display(Name = "Ambar Kodu")]
        public string WarehouseCode { get; set; } = string.Empty;

        /// <summary>
        /// Ambarın adıdır.
        /// </summary>
        [Display(Name = "Ambar Adı")]
        public string WarehouseName { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin ID bilgisidir.
        /// </summary>
        [Display(Name = "Malzeme ID")]
        public int MaterialId { get; set; }

        /// <summary>
        /// Malzemenin kodudur.
        /// </summary>
        [Display(Name = "Malzeme Kodu")]
        public string MaterialCode { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin adıdır.
        /// </summary>
        [Display(Name = "Malzeme Adı")]
        public string MaterialName { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin ölçü birimi adıdır.
        /// </summary>
        [Display(Name = "Ölçü Birimi")]
        public string MeasurementUnitName { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin sembolüdür.
        /// </summary>
        [Display(Name = "Ölçü Birimi Sembolü")]
        public string MeasurementUnitSymbol { get; set; } = string.Empty;

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "İşlemi Yapan Kullanıcı ID")]
        public int PerformedByUserId { get; set; }

        /// <summary>
        /// İşlemi yapan kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "İşlemi Yapan")]
        public string PerformedByUserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Stok hareketinin türüdür.
        /// </summary>
        [Display(Name = "Hareket Türü")]
        public StockMovementTypeEnum MovementType { get; set; }

        /// <summary>
        /// Stok hareket türünün Türkçe açıklamasıdır.
        /// </summary>
        [Display(Name = "Hareket Türü Açıklaması")]
        public string MovementTypeName => MovementType switch
        {
            StockMovementTypeEnum.In => "Stok Girişi",
            StockMovementTypeEnum.Out => "Stok Çıkışı",
            _ => "Bilinmeyen Hareket"
        };

        /// <summary>
        /// Hareketin pozitif olarak saklanan miktarıdır.
        /// </summary>
        [Display(Name = "Hareket Miktarı")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Hareket yönüne göre işaretli miktardır.
        /// Stok girişinde pozitif, çıkışında negatiftir.
        /// </summary>
        [Display(Name = "İşaretli Miktar")]
        public decimal SignedQuantity => MovementType switch
        {
            StockMovementTypeEnum.In => Quantity,
            StockMovementTypeEnum.Out => -Quantity,
            _ => 0m
        };

        /// <summary>
        /// Stok hareketinin gerçekleştiği tarihtir.
        /// </summary>
        [Display(Name = "Hareket Tarihi")]
        public DateTime MovementDate { get; set; }

        /// <summary>
        /// Hareketin referans numarasıdır.
        /// </summary>
        [Display(Name = "Referans Numarası")]
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Entity üzerinde tanımlı transfer grup bilgisidir.
        /// Mevcut iki hareket türüyle aktif olarak kullanılmaz.
        /// </summary>
        [Display(Name = "Transfer Grup ID")]
        public Guid? TransferGroupId { get; set; }

        /// <summary>
        /// Stok hareketine ait açıklamadır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Kaydın oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kaydın son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}

