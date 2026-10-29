using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetUserDtos
{
    public class CreateAssetUserDto
    {
        /// <summary>
        /// Zimmetlenecek demirbaşın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir demirbaş seçiniz.")]
        [Display(Name = "Demirbaş")]
        public int AssetId { get; set; }

        /// <summary>
        /// Demirbaşın zimmetleneceği
        /// kullanıcının ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir kullanıcı seçiniz.")]
        [Display(Name = "Zimmetli Kullanıcı")]
        public int UserId { get; set; }

        /// <summary>
        /// Zimmet işlemiyle ilgili
        /// isteğe bağlı açıklama bilgisidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
