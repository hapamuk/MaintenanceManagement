using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WarehouseStockDtos
{
    public class ResultWarehouseStockDto
    {
        /// <summary>
        /// Stok kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Stok Kaydı ID")]
        public int Id { get; set; }

        /// <summary>
        /// Stok kaydının bulunduğu ambarın ID bilgisidir.
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
        /// Stokta bulunan malzemenin ID bilgisidir.
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
        public string MeasurementUnitName { get; set; }
            = string.Empty;

        /// <summary>
        /// Ölçü biriminin kısa gösterimidir.
        /// </summary>
        [Display(Name = "Ölçü Birimi Sembolü")]
        public string MeasurementUnitSymbol { get; set; }
            = string.Empty;

        /// <summary>
        /// Ambarda bulunan mevcut malzeme miktarıdır.
        /// </summary>
        [Display(Name = "Mevcut Stok Miktarı")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Malzeme kartında tanımlanan
        /// minimum stok seviyesidir.
        /// </summary>
        [Display(Name = "Minimum Stok Seviyesi")]
        public decimal MinimumStockLevel { get; set; }

        /// <summary>
        /// Mevcut miktarın minimum stok
        /// seviyesinin altında olup olmadığını gösterir.
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
        /// kullanılan stok sürüm bilgisidir.
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
