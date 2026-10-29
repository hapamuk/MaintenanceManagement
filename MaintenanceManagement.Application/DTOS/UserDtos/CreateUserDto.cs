using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class CreateUserDto
    {
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
        /// Sisteme giriş için kullanılabilecek
        /// kullanıcı adı.
        /// </summary>
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        /// <summary>
        /// Kullanıcının telefon numarası.
        /// </summary>
        [Phone(
            ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        /// <summary>
        /// Kullanıcının görev veya unvan bilgisi.
        /// </summary>
        [Display(Name = "Unvan")]
        public string? Title { get; set; }

        /// <summary>
        /// Kullanıcının birincil organizasyon birimi.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon birimi seçiniz.")]
        [Display(Name = "Birincil Organizasyon Birimi")]
        public int PrimaryUnitId { get; set; }

        /// <summary>
        /// Kullanıcıya sisteme giriş
        /// yeteneği tanımlanıp tanımlanmayacağı.
        ///
        /// Yönetici tarafından belirlenir.
        /// </summary>
        [Display(Name = "Sistem Erişimi")]
        public bool HasSystemAccess { get; set; }

        /// <summary>
        /// Sistem hesabı oluşturulacaksa
        /// kullanılacak başlangıç şifresi.
        ///
        /// Veritabanına düz metin kaydedilmez.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string? Password { get; set; }

        /// <summary>
        /// Girilen başlangıç şifresinin tekrarı.
        /// </summary>
        [DataType(DataType.Password)]
        [Compare(
            nameof(Password),
            ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Şifre Tekrar")]
        public string? ConfirmPassword { get; set; }
    }
}
