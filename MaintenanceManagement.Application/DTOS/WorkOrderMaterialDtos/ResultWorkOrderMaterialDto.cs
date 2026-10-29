using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderMaterialDtos
{
    public class ResultWorkOrderMaterialDto
    {
        /// <summary>
        /// Malzeme kullanım kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kayıt ID")]
        public int Id { get; set; }

        /// <summary>
        /// İş emrinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Emri ID")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// İş emrinin numarasıdır.
        /// </summary>
        [Display(Name = "İş Emri Numarası")]
        public string WorkOrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin başlığıdır.
        /// </summary>
        [Display(Name = "İş Emri Başlığı")]
        public string WorkOrderTitle { get; set; } = string.Empty;

        /// <summary>
        /// Kullanılan malzemenin ID bilgisidir.
        /// </summary>
        [Display(Name = "Malzeme ID")]
        public int MaterialId { get; set; }

        /// <summary>
        /// Kullanılan malzemenin kodudur.
        /// </summary>
        [Display(Name = "Malzeme Kodu")]
        public string MaterialCode { get; set; } = string.Empty;

        /// <summary>
        /// Kullanılan malzemenin adıdır.
        /// </summary>
        [Display(Name = "Malzeme Adı")]
        public string MaterialName { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin ölçü birimidir.
        /// </summary>
        [Display(Name = "Ölçü Birimi")]
        public string MeasurementUnitName { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin kısa gösterimidir.
        /// </summary>
        [Display(Name = "Ölçü Birimi Sembolü")]
        public string MeasurementUnitSymbol { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin alındığı ambarın ID bilgisidir.
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
        /// Kullanılan miktar ile birim maliyetin çarpımıdır.
        /// Birim maliyet girilmemişse null döner.
        /// </summary>
        [Display(Name = "Toplam Maliyet")]
        public decimal? TotalCost =>
            UnitCost.HasValue
                ? Quantity * UnitCost.Value
                : null;

        /// <summary>
        /// İlişkili stok hareketinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Stok Hareketi ID")]
        public int? StockMovementId { get; set; }

        /// <summary>
        /// Malzemenin kullanım açıklamasıdır.
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
