using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetDtos
{
    public class CreateAssetDto
    {
        /// <summary>
        /// Demirbaşın sistem içerisindeki
        /// benzersiz kodudur.
        /// </summary>
        [Required(
            ErrorMessage = "Demirbaş kodu zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Demirbaş kodu en fazla 100 karakter olabilir.")]
        [Display(Name = "Demirbaş Kodu")]
        public string AssetCode { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Demirbaş adı zorunludur.")]
        [StringLength(
            200,
            ErrorMessage = "Demirbaş adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Demirbaş Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın üretici tarafından
        /// verilen seri numarasıdır.
        /// </summary>
        [StringLength(
            150,
            ErrorMessage = "Seri numarası en fazla 150 karakter olabilir.")]
        [Display(Name = "Seri Numarası")]
        public string? SerialNumber { get; set; }

        /// <summary>
        /// Demirbaşın kurum içerisindeki
        /// envanter numarasıdır.
        /// </summary>
        [StringLength(
            100,
            ErrorMessage = "Envanter numarası en fazla 100 karakter olabilir.")]
        [Display(Name = "Envanter Numarası")]
        public string? InventoryNumber { get; set; }

        /// <summary>
        /// Demirbaşın bağlı olduğu
        /// organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon birimi seçiniz.")]
        [Display(Name = "Organizasyon Birimi")]
        public int UnitId { get; set; }

        /// <summary>
        /// Demirbaşın model ID bilgisidir.
        ///
        /// Marka bilgisi seçilen modelin
        /// AssetModel.Brand ilişkisiyle belirlenir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir demirbaş modeli seçiniz.")]
        [Display(Name = "Demirbaş Modeli")]
        public int AssetModelId { get; set; }

        /// <summary>
        /// Demirbaşın tür ID bilgisidir.
        ///
        /// Kategori bilgisi seçilen türün
        /// AssetKind.AssetCategory ilişkisiyle belirlenir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir demirbaş türü seçiniz.")]
        [Display(Name = "Demirbaş Türü")]
        public int AssetKindId { get; set; }

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
        /// Demirbaş hakkında ek açıklama bilgisidir.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
