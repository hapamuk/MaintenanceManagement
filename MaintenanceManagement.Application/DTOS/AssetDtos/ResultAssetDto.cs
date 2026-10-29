using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetDtos
{
    public class ResultAssetDto
    {
        /// <summary>
        /// Demirbaşın ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş ID")]
        public int Id { get; set; }

        /// <summary>
        /// Demirbaşın benzersiz kodudur.
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
        /// Bağlı olduğu organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş modelinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Model ID")]
        public int AssetModelId { get; set; }

        /// <summary>
        /// Demirbaş modelinin adıdır.
        /// </summary>
        [Display(Name = "Model")]
        public string AssetModelName { get; set; } = string.Empty;

        /// <summary>
        /// Modelin bağlı olduğu markanın ID bilgisidir.
        ///
        /// AssetModel.BrandId üzerinden getirilir.
        /// </summary>
        [Display(Name = "Marka ID")]
        public int BrandId { get; set; }

        /// <summary>
        /// Modelin bağlı olduğu markanın adıdır.
        ///
        /// AssetModel.Brand.Name üzerinden getirilir.
        /// </summary>
        [Display(Name = "Marka")]
        public string BrandName { get; set; } = string.Empty;

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
        /// Demirbaş türünün bağlı olduğu
        /// kategorinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kategori ID")]
        public int AssetCategoryId { get; set; }

        /// <summary>
        /// Demirbaş kategorisinin adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Kategorisi")]
        public string AssetCategoryName { get; set; } = string.Empty;

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
        /// Demirbaşın mevcut durumudur.
        /// </summary>
        [Display(Name = "Durum")]
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
        /// Demirbaşın açıklama bilgisidir.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

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
