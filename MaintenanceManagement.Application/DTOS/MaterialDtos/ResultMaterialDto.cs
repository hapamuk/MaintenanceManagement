using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaterialDtos
{
    public class ResultMaterialDto
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
        public string MeasurementUnitName { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin kısa gösterimidir.
        /// Örnek: kg, m, L.
        /// </summary>
        [Display(Name = "Ölçü Birimi Sembolü")]
        public string MeasurementUnitSymbol { get; set; } = string.Empty;

        /// <summary>
        /// Malzeme kategorisinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kategori ID")]
        public int? MaterialCategoryId { get; set; }

        /// <summary>
        /// Malzemenin bağlı olduğu kategori adıdır.
        /// </summary>
        [Display(Name = "Malzeme Kategorisi")]
        public string? MaterialCategoryName { get; set; }

        /// <summary>
        /// Malzemenin ilişkili olduğu birimin
        /// ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int? UnitId { get; set; }

        /// <summary>
        /// Malzemenin ilişkili olduğu birimin adıdır.
        /// </summary>
        [Display(Name = "İlgili Birim")]
        public string? UnitName { get; set; }

        /// <summary>
        /// Malzemenin minimum stok seviyesidir.
        /// </summary>
        [Display(Name = "Minimum Stok Seviyesi")]
        public decimal MinimumStockLevel { get; set; }

        /// <summary>
        /// Malzemenin açıklamasıdır.
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
