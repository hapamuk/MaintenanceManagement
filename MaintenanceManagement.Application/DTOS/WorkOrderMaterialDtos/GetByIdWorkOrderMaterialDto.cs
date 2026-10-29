using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderMaterialDtos
{
    public class GetByIdWorkOrderMaterialDto
    {
        /// <summary>
        /// Malzeme kullanım kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kayıt ID")]
        public int Id { get; set; }

        /// <summary>
        /// İlgili iş emrinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Emri ID")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// İlgili iş emrinin numarasıdır.
        /// </summary>
        [Display(Name = "İş Emri Numarası")]
        public string WorkOrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin başlığıdır.
        /// </summary>
        [Display(Name = "İş Emri Başlığı")]
        public string WorkOrderTitle { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin açıklamasıdır.
        /// </summary>
        [Display(Name = "İş Emri Açıklaması")]
        public string? WorkOrderDescription { get; set; }

        /// <summary>
        /// Kullanılan malzemenin ID bilgisidir.
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
        /// Malzemenin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Malzeme Aktif")]
        public bool MaterialIsActive { get; set; }

        /// <summary>
        /// Malzemenin minimum stok seviyesidir.
        /// </summary>
        [Display(Name = "Minimum Stok Seviyesi")]
        public decimal MaterialMinimumStockLevel { get; set; }

        /// <summary>
        /// Malzemenin ölçü birimi ID bilgisidir.
        /// </summary>
        [Display(Name = "Ölçü Birimi ID")]
        public int MeasurementUnitId { get; set; }

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
        /// Kullanım yapılan ambarın ID bilgisidir.
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
        /// Ambarın konum bilgisidir.
        /// </summary>
        [Display(Name = "Ambar Konumu")]
        public string? WarehouseLocation { get; set; }

        /// <summary>
        /// Ambarın aktiflik durumudur.
        /// </summary>
        [Display(Name = "Ambar Aktif")]
        public bool WarehouseIsActive { get; set; }

        /// <summary>
        /// İş emrinde kullanılan malzeme miktarıdır.
        /// </summary>
        [Display(Name = "Kullanılan Miktar")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Malzemenin birim maliyetidir.
        /// </summary>
        [Display(Name = "Birim Maliyet")]
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// Miktar ve birim maliyetin çarpımıdır.
        /// </summary>
        [Display(Name = "Toplam Maliyet")]
        public decimal? TotalCost =>
            UnitCost.HasValue
                ? Quantity * UnitCost.Value
                : null;

        /// <summary>
        /// İlgili stok hareketinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Stok Hareketi ID")]
        public int? StockMovementId { get; set; }

        /// <summary>
        /// Stok hareketinin referans numarasıdır.
        /// </summary>
        [Display(Name = "Stok Hareketi Referans Numarası")]
        public string? StockMovementReferenceNumber { get; set; }

        /// <summary>
        /// Stok hareketinin gerçekleştiği tarihtir.
        /// </summary>
        [Display(Name = "Stok Hareketi Tarihi")]
        public DateTime? StockMovementDate { get; set; }

        /// <summary>
        /// Kullanılan malzemeye ait açıklamadır.
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

