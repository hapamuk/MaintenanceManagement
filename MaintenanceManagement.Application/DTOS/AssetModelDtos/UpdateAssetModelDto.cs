using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetModelDtos
{
    public class UpdateAssetModelDto
    {
       
        public int Id { get; set; }

        /// <summary>
        /// Modelin bağlı olacağı
        /// markanın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir marka seçiniz.")]
        [Display(Name = "Marka")]
        public int BrandId { get; set; }

        /// <summary>
        /// Modelin güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Model adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Model adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Model Adı")]
        public string Name { get; set; } = string.Empty;
    }
}
