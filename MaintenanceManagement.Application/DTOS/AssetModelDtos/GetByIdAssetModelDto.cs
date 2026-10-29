using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetModelDtos
{
    public class GetByIdAssetModelDto
    {
        /// <summary>
        /// Demirbaş modelinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Model ID")]
        public int Id { get; set; }

        /// <summary>
        /// Modelin bağlı olduğu
        /// markanın ID bilgisidir.
        /// </summary>
        [Display(Name = "Marka ID")]
        public int BrandId { get; set; }

        /// <summary>
        /// Modelin bağlı olduğu markanın adıdır.
        /// </summary>
        [Display(Name = "Marka Adı")]
        public string BrandName { get; set; } = string.Empty;

        /// <summary>
        /// Modelin bağlı olduğu markanın
        /// aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Marka Durumu")]
        public bool BrandIsActive { get; set; }

        /// <summary>
        /// Demirbaş modelinin adıdır.
        /// </summary>
        [Display(Name = "Model Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş modelinin
        /// aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Model Durumu")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Bu modele bağlı toplam demirbaş
        /// kayıtlarının sayısıdır.
        ///
        /// AssetModel.Assets koleksiyonundan
        /// hesaplanır.
        /// </summary>
        [Display(Name = "Toplam Demirbaş Sayısı")]
        public int AssetCount { get; set; }

        /// <summary>
        /// Model kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Model kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
