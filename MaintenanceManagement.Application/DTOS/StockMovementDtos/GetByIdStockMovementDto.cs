using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.StockMovementDtos
{
    public class GetByIdStockMovementDto
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
        /// Ambarın fiziksel konumudur.
        /// </summary>
        [Display(Name = "Ambar Konumu")]
        public string? WarehouseLocation { get; set; }

        /// <summary>
        /// Ambarın aktiflik durumudur.
        /// </summary>
        [Display(Name = "Ambar Aktif")]
        public bool WarehouseIsActive { get; set; }

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
        /// Malzemenin açıklamasıdır.
        /// </summary>
        [Display(Name = "Malzeme Açıklaması")]
        public string? MaterialDescription { get; set; }

        /// <summary>
        /// Malzeme kategorisinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kategori ID")]
        public int? MaterialCategoryId { get; set; }

        /// <summary>
        /// Malzeme kategorisinin adıdır.
        /// </summary>
        [Display(Name = "Malzeme Kategorisi")]
        public string? MaterialCategoryName { get; set; }

        /// <summary>
        /// Ölçü biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ölçü Birimi ID")]
        public int MeasurementUnitId { get; set; }

        /// <summary>
        /// Ölçü biriminin adıdır.
        /// </summary>
        [Display(Name = "Ölçü Birimi")]
        public string MeasurementUnitName { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin sembolüdür.
        /// </summary>
        [Display(Name = "Ölçü Birimi Sembolü")]
        public string MeasurementUnitSymbol { get; set; } = string.Empty;

        /// <summary>
        /// İşlemi yapan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "İşlemi Yapan Kullanıcı ID")]
        public int PerformedByUserId { get; set; }

        /// <summary>
        /// İşlemi yapan kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "İşlemi Yapan")]
        public string PerformedByUserFullName { get; set; } = string.Empty;

        /// <summary>
        /// İşlemi yapan kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "Kullanıcı E-Posta")]
        public string PerformedByUserEmail { get; set; } = string.Empty;

        /// <summary>
        /// İşlemi yapan kullanıcının unvanıdır.
        /// </summary>
        [Display(Name = "Kullanıcı Unvanı")]
        public string? PerformedByUserTitle { get; set; }

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
        /// Hareket miktarıdır.
        /// </summary>
        [Display(Name = "Hareket Miktarı")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Hareket yönüne göre işaretli miktardır.
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
        /// Stok hareketinin referans numarasıdır.
        /// </summary>
        [Display(Name = "Referans Numarası")]
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Entity üzerinde bulunan transfer grup bilgisidir.
        /// Mevcut enumla transfer akışı kullanılmamaktadır.
        /// </summary>
        [Display(Name = "Transfer Grup ID")]
        public Guid? TransferGroupId { get; set; }

        /// <summary>
        /// Stok hareketine ilişkin açıklamadır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Kaydın oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kaydın güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
