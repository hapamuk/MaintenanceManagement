using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.TeamDtos
{
    public class CreateTeamDto
    {
        /// <summary>
        /// Ekibin bağlı olduğu organizasyon
        /// biriminin ID bilgisidir.
        /// </summary>
        [Required(
            ErrorMessage = "Organizasyon birimi zorunludur.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon birimi seçiniz.")]
        [Display(Name = "Organizasyon Birimi")]
        public int UnitId { get; set; }

        /// <summary>
        /// Oluşturulacak ekibin adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Ekip adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Ekip adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Ekip Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ekibin sistem içerisinde kullanılan kodudur.
        /// </summary>
        [Required(
            ErrorMessage = "Ekip kodu zorunludur.")]
        [StringLength(
            50,
            ErrorMessage = "Ekip kodu en fazla 50 karakter olabilir.")]
        [Display(Name = "Ekip Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Ekibin görev ve sorumluluklarını
        /// açıklayan isteğe bağlı bilgidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
