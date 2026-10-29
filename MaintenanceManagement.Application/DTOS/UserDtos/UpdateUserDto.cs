using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class UpdateUserDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Kullanıcının adı.
        /// </summary>
        [Required(ErrorMessage = "Ad zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının soyadı.
        /// </summary>
        [Required(ErrorMessage = "Soyad zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının e-posta adresi.
        /// </summary>
        [EmailAddress(
            ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        /// <summary>
        /// Kullanıcının giriş adı.
        /// </summary>
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        /// <summary>
        /// Kullanıcının telefon bilgisi.
        /// </summary>
        [Phone(
            ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        /// <summary>
        /// Kullanıcının unvan bilgisi.
        /// </summary>
        [Display(Name = "Unvan")]
        public string? Title { get; set; }

        /// <summary>
        /// Kullanıcının birincil
        /// organizasyon birimi.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon birimi seçiniz.")]
        [Display(Name = "Birincil Organizasyon Birimi")]
        public int PrimaryUnitId { get; set; }
    }
}
