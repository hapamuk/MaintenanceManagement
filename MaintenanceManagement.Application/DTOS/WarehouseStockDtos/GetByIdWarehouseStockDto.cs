using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WarehouseStockDtos
{
    public class GetByIdWarehouseStockDto
    {
        /// <summary>
        /// Stok kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Stok Kaydı ID")]
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
        /// Ambarın aktif olup olmadığını belirtir.
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
        /// Malzemeye ait açıklamadır.
        /// </summary>
        [Display(Name = "Malzeme Açıklaması")]
        public string? MaterialDescription { get; set; }

        /// <summary>
        /// Malzemenin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Malzeme Aktif")]
        public bool MaterialIsActive { get; set; }

        /// <summary>
        /// Malzemenin kategori ID bilgisidir.
        /// </summary>
        [Display(Name = "Kategori ID")]
        public int? MaterialCategoryId { get; set; }

        /// <summary>
        /// Malzeme kategorisinin adıdır.
        /// </summary>
        [Display(Name = "Malzeme Kategorisi")]
        public string? MaterialCategoryName { get; set; }

        /// <summary>
        /// Malzemenin ölçü birimi ID bilgisidir.
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
        /// Ambarda bulunan mevcut stok miktarıdır.
        /// </summary>
        [Display(Name = "Mevcut Stok Miktarı")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Malzemenin minimum stok seviyesidir.
        /// </summary>
        [Display(Name = "Minimum Stok Seviyesi")]
        public decimal MinimumStockLevel { get; set; }

        /// <summary>
        /// Mevcut stok miktarının minimum
        /// seviyenin altında olup olmadığını gösterir.
        /// </summary>
        [Display(Name = "Kritik Stok")]
        public bool IsBelowMinimum =>
            Quantity < MinimumStockLevel;

        /// <summary>
        /// Malzemenin stokta tükenip
        /// tükenmediğini belirtir.
        /// </summary>
        [Display(Name = "Stok Tükendi")]
        public bool IsOutOfStock =>
            Quantity <= 0;

        /// <summary>
        /// Stok durumunun Türkçe açıklamasıdır.
        /// </summary>
        [Display(Name = "Stok Durumu")]
        public string StockStatusName
        {
            get
            {
                if (IsOutOfStock)
                    return "Stok Yok";

                if (IsBelowMinimum)
                    return "Kritik Stok";

                return "Yeterli Stok";
            }
        }

        /// <summary>
        /// Son stok hareketinin tarihidir.
        /// </summary>
        [Display(Name = "Son Stok Hareketi")]
        public DateTime? LastMovementDate { get; set; }

        /// <summary>
        /// Eş zamanlı güncelleme kontrolü için
        /// kullanılan RowVersion bilgisidir.
        /// </summary>
        [Display(Name = "Stok Sürüm Bilgisi")]
        public byte[] RowVersion { get; set; }
            = Array.Empty<byte>();

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
