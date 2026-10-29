using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WarehouseDtos
{
    public class GetByIdWarehouseDto
    {
        /// <summary>
        /// Ambarın ID bilgisidir.
        /// </summary>
        [Display(Name = "Ambar ID")]
        public int Id { get; set; }

        /// <summary>
        /// Ambarın benzersiz kodudur.
        /// </summary>
        [Display(Name = "Ambar Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Ambarın adıdır.
        /// </summary>
        [Display(Name = "Ambar Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ambarın bağlı olduğu birimin
        /// ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int? UnitId { get; set; }

        /// <summary>
        /// Ambarın bağlı olduğu birimin adıdır.
        /// </summary>
        [Display(Name = "Bağlı Birim")]
        public string? UnitName { get; set; }

        /// <summary>
        /// Ambarın bağlı olduğu birimin kodudur.
        /// </summary>
        [Display(Name = "Birim Kodu")]
        public string? UnitCode { get; set; }

        /// <summary>
        /// Ambarın fiziksel konum bilgisidir.
        /// </summary>
        [Display(Name = "Ambar Konumu")]
        public string? Location { get; set; }

        /// <summary>
        /// Ambar hakkında açıklama bilgisidir.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Ambarın aktiflik durumudur.
        /// </summary>
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Aktiflik durumunun Türkçe karşılığıdır.
        /// </summary>
        [Display(Name = "Durum")]
        public string IsActiveName =>
            IsActive ? "Aktif" : "Pasif";

        /// <summary>
        /// Ambara bağlı toplam stok kayıt sayısıdır.
        /// </summary>
        [Display(Name = "Stok Kayıt Sayısı")]
        public int WarehouseStockCount { get; set; }

        /// <summary>
        /// Miktarı sıfırdan büyük olan
        /// stok kayıtlarının sayısıdır.
        /// </summary>
        [Display(Name = "Stokta Bulunan Malzeme Sayısı")]
        public int AvailableMaterialCount { get; set; }

        /// <summary>
        /// Ambarda miktarı sıfır olan
        /// stok kayıtlarının sayısıdır.
        /// </summary>
        [Display(Name = "Tükenen Stok Kayıt Sayısı")]
        public int OutOfStockCount { get; set; }

        /// <summary>
        /// Ambara ait stok hareketlerinin
        /// toplam sayısıdır.
        /// </summary>
        [Display(Name = "Stok Hareketi Sayısı")]
        public int StockMovementCount { get; set; }

        /// <summary>
        /// Bu ambardan karşılanan iş emri
        /// malzeme kullanım kayıtlarının sayısıdır.
        /// </summary>
        [Display(Name = "İş Emri Malzeme Kayıt Sayısı")]
        public int WorkOrderMaterialCount { get; set; }

        /// <summary>
        /// Ambar kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Ambar kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
