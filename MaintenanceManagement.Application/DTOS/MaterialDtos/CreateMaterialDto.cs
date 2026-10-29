using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaterialDtos
{
    public class CreateMaterialDto
    {
        /// <summary>
        /// Malzemenin benzersiz kodudur.
        /// </summary>
        [Required(
            ErrorMessage = "Malzeme kodu zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Malzeme kodu en fazla 100 karakter olabilir.")]
        [Display(Name = "Malzeme Kodu")]
        public string MaterialCode { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Malzeme adı zorunludur.")]
        [StringLength(
            200,
            ErrorMessage = "Malzeme adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Malzeme Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin ölçü biriminin ID bilgisidir.
        /// Örnek: Adet, Kilogram, Metre.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir ölçü birimi seçiniz.")]
        [Display(Name = "Ölçü Birimi")]
        public int MeasurementUnitId { get; set; }

        /// <summary>
        /// Malzemenin bağlı olduğu kategorinin
        /// ID bilgisidir. İsteğe bağlıdır.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir malzeme kategorisi seçiniz.")]
        [Display(Name = "Malzeme Kategorisi")]
        public int? MaterialCategoryId { get; set; }

        /// <summary>
        /// Malzemenin ilişkili olduğu
        /// organizasyon biriminin ID bilgisidir.
        /// İsteğe bağlıdır.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir birim seçiniz.")]
        [Display(Name = "İlgili Birim")]
        public int? UnitId { get; set; }

        /// <summary>
        /// Malzemenin minimum stok seviyesidir.
        /// Stok kritik seviyeye indiğinde
        /// uyarı oluşturulması için kullanılır.
        /// </summary>
        [Range(
            typeof(decimal),
            "0",
            "999999999.999",
            ErrorMessage = "Minimum stok seviyesi geçerli aralıkta olmalıdır.")]
        [Display(Name = "Minimum Stok Seviyesi")]
        public decimal MinimumStockLevel { get; set; } = 0;

        /// <summary>
        /// Malzemeye ait açıklama bilgisidir.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
