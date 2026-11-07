using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaterialCategoryDtos
{
    public class CreateMaterialCategoryDto
    {
        /// <summary>
        /// Malzeme kategorisinin adıdır.
        ///
        /// Örnek:
        /// Elektrik Malzemeleri,
        /// Mekanik Malzemeler,
        /// Tesisat Malzemeleri.
        /// </summary>
        [Required(
            ErrorMessage = "Malzeme kategorisi adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Kategori adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Malzeme Kategorisi Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Malzeme kategorisi hakkında
        /// isteğe bağlı açıklama bilgisidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        // =====================================
        // ORGANİZASYON SEÇİMLERİ
        // =====================================

        /// <summary>
        /// Malzeme kategorisinin kullanılabileceği
        /// organizasyon birimlerinin ID listesidir.
        ///
        /// Hiçbir organizasyon seçilmezse
        /// kategori tüm organizasyonlara açık olur.
        ///
        /// Organizasyon seçilirse yalnızca
        /// seçilen organizasyonlarda kullanılabilir.
        /// </summary>
        [Display(Name = "Yetkili Organizasyonlar")]
        public List<int> SelectedUnitIds { get; set; }
            = new List<int>();
    }
}
