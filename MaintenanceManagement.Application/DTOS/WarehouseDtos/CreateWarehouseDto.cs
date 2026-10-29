using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WarehouseDtos
{
    public class CreateWarehouseDto
    {
        /// <summary>
        /// Ambarın benzersiz kodudur.
        ///
        /// Örnek: AMB-001, MRK-AMB-01.
        /// </summary>
        [Required(
            ErrorMessage = "Ambar kodu zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Ambar kodu en fazla 100 karakter olabilir.")]
        [Display(Name = "Ambar Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Ambarın adıdır.
        ///
        /// Örnek: Merkez Ambar, Teknik Servis Ambarı.
        /// </summary>
        [Required(
            ErrorMessage = "Ambar adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Ambar adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Ambar Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ambarın bağlı olduğu organizasyon
        /// biriminin ID bilgisidir.
        ///
        /// İsteğe bağlıdır.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir birim seçiniz.")]
        [Display(Name = "Bağlı Birim")]
        public int? UnitId { get; set; }

        /// <summary>
        /// Ambarın fiziksel konum bilgisidir.
        ///
        /// Örnek: A Blok Zemin Kat.
        /// </summary>
        [StringLength(
            250,
            ErrorMessage = "Ambar konumu en fazla 250 karakter olabilir.")]
        [Display(Name = "Ambar Konumu")]
        public string? Location { get; set; }

        /// <summary>
        /// Ambara ait açıklama bilgisidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
