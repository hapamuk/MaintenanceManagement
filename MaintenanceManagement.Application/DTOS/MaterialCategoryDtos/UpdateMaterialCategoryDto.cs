using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaterialCategoryDtos
{
    public class UpdateMaterialCategoryDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Malzeme kategorisinin güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Malzeme kategorisi adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Kategori adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Malzeme Kategorisi Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Malzeme kategorisine ait güncel
        /// açıklama bilgisidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
