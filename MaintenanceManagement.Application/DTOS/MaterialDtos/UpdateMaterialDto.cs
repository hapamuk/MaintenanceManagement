using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaterialDtos
{
    public class UpdateMaterialDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Malzemenin güncel kodudur.
        /// </summary>
        [Required(ErrorMessage = "Malzeme kodu zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Malzeme kodu en fazla 100 karakter olabilir.")]
        [Display(Name = "Malzeme Kodu")]
        public string MaterialCode { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Malzeme adı zorunludur.")]
        [StringLength(
            200,
            ErrorMessage = "Malzeme adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Malzeme Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Malzemenin ölçü birimi ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir ölçü birimi seçiniz.")]
        [Display(Name = "Ölçü Birimi")]
        public int MeasurementUnitId { get; set; }

        /// <summary>
        /// Malzeme kategorisinin ID bilgisidir.
        /// Boş bırakılabilir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir malzeme kategorisi seçiniz.")]
        [Display(Name = "Malzeme Kategorisi")]
        public int? MaterialCategoryId { get; set; }

        /// <summary>
        /// Malzemenin ilişkili olduğu birimin
        /// ID bilgisidir. Boş bırakılabilir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir birim seçiniz.")]
        [Display(Name = "İlgili Birim")]
        public int? UnitId { get; set; }

        /// <summary>
        /// Malzemenin güncel minimum stok seviyesidir.
        /// </summary>
        [Range(
    typeof(decimal),
    "0",
    "999999999.999",
    ParseLimitsInInvariantCulture = true,
    ErrorMessage = "Minimum stok seviyesi geçerli aralıkta olmalıdır.")]
        [Display(Name = "Minimum Stok Seviyesi")]
        public decimal MinimumStockLevel { get; set; }

        /// <summary>
        /// Malzemeye ait güncel açıklamadır.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }


    }
}
