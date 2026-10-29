using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetCategoryDtos
{
    public class CreateAssetCategoryDto
    {
        /// <summary>
        /// Oluşturulacak demirbaş kategorisinin adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Kategori adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş kategorisinin açıklama bilgisidir.
        ///
        /// Doldurulması zorunlu değildir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
