using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaterialDtos
{
    public class GetByIdMaterialDto
    {
        /// <summary>
        /// Malzemenin ID bilgisidir.
        /// </summary>
        [Display(Name = "Malzeme ID")]
        public int Id { get; set; }

        /// <summary>
        /// Malzemenin benzersiz kodudur.
        /// </summary>
        [Display(Name = "Malzeme Kodu")]
        public string MaterialCode { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin adıdır.
        /// </summary>
        [Display(Name = "Malzeme Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ölçü Birimi ID")]
        public int MeasurementUnitId { get; set; }

        /// <summary>
        /// Ölçü biriminin adıdır.
        /// </summary>
        [Display(Name = "Ölçü Birimi")]
        public string MeasurementUnitName { get; set; }
            = string.Empty;

        /// <summary>
        /// Ölçü biriminin kısa gösterimidir.
        /// </summary>
        [Display(Name = "Ölçü Birimi Sembolü")]
        public string MeasurementUnitSymbol { get; set; }
            = string.Empty;

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
        /// Malzeme kategorisinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Kategori Açıklaması")]
        public string? MaterialCategoryDescription { get; set; }

        /// <summary>
        /// Kategorinin aktiflik durumudur.
        /// Kategori tanımlı değilse null döner.
        /// </summary>
        [Display(Name = "Kategori Aktif")]
        public bool? MaterialCategoryIsActive { get; set; }

        /// <summary>
        /// Malzemenin ilişkili olduğu birimin
        /// ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int? UnitId { get; set; }

        /// <summary>
        /// İlgili organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "İlgili Birim")]
        public string? UnitName { get; set; }

        /// <summary>
        /// İlgili organizasyon biriminin kodudur.
        /// </summary>
        [Display(Name = "Birim Kodu")]
        public string? UnitCode { get; set; }

        /// <summary>
        /// Malzemenin minimum stok seviyesidir.
        /// </summary>
        [Display(Name = "Minimum Stok Seviyesi")]
        public decimal MinimumStockLevel { get; set; }

        /// <summary>
        /// Malzemeye ait açıklamadır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Malzemenin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Aktiflik durumunun Türkçe açıklamasıdır.
        /// </summary>
        [Display(Name = "Durum")]
        public string IsActiveName =>
            IsActive ? "Aktif" : "Pasif";

        /// <summary>
        /// Malzemenin kullanıldığı iş emri
        /// malzeme kayıtlarının sayısıdır.
        /// </summary>
        [Display(Name = "İş Emri Malzeme Kayıt Sayısı")]
        public int WorkOrderMaterialCount { get; set; }

        /// <summary>
        /// Malzemeye ait stok hareketi
        /// kayıtlarının sayısıdır.
        /// </summary>
        [Display(Name = "Stok Hareketi Sayısı")]
        public int StockMovementCount { get; set; }

        /// <summary>
        /// Malzemenin bulunduğu ambar stok
        /// kayıtlarının sayısıdır.
        /// </summary>
        [Display(Name = "Ambar Stok Kayıt Sayısı")]
        public int WarehouseStockCount { get; set; }

        /// <summary>
        /// Malzeme kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Malzeme kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
