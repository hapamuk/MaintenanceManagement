using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetKindDtos
{
    public class UpdateAssetKindDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Demirbaş türünün bağlı olacağı
        /// kategorinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir demirbaş kategorisi seçiniz.")]
        [Display(Name = "Demirbaş Kategorisi")]
        public int AssetCategoryId { get; set; }

        /// <summary>
        /// Demirbaş türünün güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Demirbaş türü adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Demirbaş türü adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Demirbaş Türü Adı")]
        public string Name { get; set; } = string.Empty;
    }
}
