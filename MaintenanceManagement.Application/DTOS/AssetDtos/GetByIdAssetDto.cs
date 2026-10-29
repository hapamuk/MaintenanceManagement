using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetDtos
{
    public class GetByIdAssetDto
    {
        /// <summary>
        /// Demirbaşın ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş ID")]
        public int Id { get; set; }

        /// <summary>
        /// Demirbaşın sistem kodudur.
        /// </summary>
        [Display(Name = "Demirbaş Kodu")]
        public string AssetCode { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın seri numarasıdır.
        /// </summary>
        [Display(Name = "Seri Numarası")]
        public string? SerialNumber { get; set; }

        /// <summary>
        /// Demirbaşın envanter numarasıdır.
        /// </summary>
        [Display(Name = "Envanter Numarası")]
        public string? InventoryNumber { get; set; }

        /// <summary>
        /// Demirbaşın bağlı olduğu
        /// organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Demirbaşın bağlı olduğu
        /// organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin
        /// aktiflik durumudur.
        /// </summary>
        [Display(Name = "Birim Durumu")]
        public bool UnitIsActive { get; set; }

        /// <summary>
        /// Demirbaş modelinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Model ID")]
        public int AssetModelId { get; set; }

        /// <summary>
        /// Demirbaş modelinin adıdır.
        /// </summary>
        [Display(Name = "Model Adı")]
        public string AssetModelName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş modelinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Model Durumu")]
        public bool AssetModelIsActive { get; set; }

        /// <summary>
        /// Modelin bağlı olduğu marka ID bilgisidir.
        /// </summary>
        [Display(Name = "Marka ID")]
        public int BrandId { get; set; }

        /// <summary>
        /// Modelin bağlı olduğu markanın adıdır.
        /// </summary>
        [Display(Name = "Marka")]
        public string BrandName { get; set; } = string.Empty;

        /// <summary>
        /// Markanın aktiflik durumudur.
        /// </summary>
        [Display(Name = "Marka Durumu")]
        public bool BrandIsActive { get; set; }

        /// <summary>
        /// Demirbaş türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "Tür ID")]
        public int AssetKindId { get; set; }

        /// <summary>
        /// Demirbaş türünün adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Türü")]
        public string AssetKindName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş türünün aktiflik durumudur.
        /// </summary>
        [Display(Name = "Tür Durumu")]
        public bool AssetKindIsActive { get; set; }

        /// <summary>
        /// Demirbaş kategorisinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kategori ID")]
        public int AssetCategoryId { get; set; }

        /// <summary>
        /// Demirbaş kategorisinin adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Kategorisi")]
        public string AssetCategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş kategorisinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Kategori Durumu")]
        public bool AssetCategoryIsActive { get; set; }

        /// <summary>
        /// Demirbaşın satın alınma tarihidir.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Satın Alma Tarihi")]
        public DateTime? PurchaseDate { get; set; }

        /// <summary>
        /// Demirbaşın garanti bitiş tarihidir.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Garanti Bitiş Tarihi")]
        public DateTime? WarrantyEndDate { get; set; }

        /// <summary>
        /// Demirbaşın durum enum değeridir.
        /// </summary>
        [Display(Name = "Demirbaş Durumu")]
        public AssetStatusEnum Status { get; set; }

        /// <summary>
        /// Demirbaş durumunun Türkçe karşılığıdır.
        /// </summary>
        [Display(Name = "Durum Açıklaması")]
        public string StatusName => Status switch
        {
            AssetStatusEnum.Active => "Aktif",
            AssetStatusEnum.OutOfService => "Kullanım Dışı",
            AssetStatusEnum.Scrapped => "Hurda",
            _ => "Bilinmiyor"
        };

        /// <summary>
        /// Demirbaşa ait açıklama bilgisidir.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Demirbaşa ait toplam kullanıcı
        /// atama kaydı sayısıdır.
        ///
        /// Geçmiş atamalar da dahildir.
        /// </summary>
        [Display(Name = "Kullanıcı Atama Sayısı")]
        public int AssetUserCount { get; set; }

        /// <summary>
        /// Demirbaşa ait toplam hareket
        /// kaydı sayısıdır.
        /// </summary>
        [Display(Name = "Hareket Sayısı")]
        public int AssetMovementCount { get; set; }

        /// <summary>
        /// Demirbaşa ait toplam iş
        /// talebi kaydı sayısıdır.
        /// </summary>
        [Display(Name = "İş Talebi Sayısı")]
        public int WorkRequestCount { get; set; }

        /// <summary>
        /// Demirbaşa ait toplam iş
        /// emri kaydı sayısıdır.
        /// </summary>
        [Display(Name = "İş Emri Sayısı")]
        public int WorkOrderCount { get; set; }

        /// <summary>
        /// Demirbaşa ait toplam planlı
        /// bakım tanımı sayısıdır.
        /// </summary>
        [Display(Name = "Planlı Bakım Sayısı")]
        public int PlannedMaintenanceCount { get; set; }

        /// <summary>
        /// Demirbaşın oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Demirbaşın son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
