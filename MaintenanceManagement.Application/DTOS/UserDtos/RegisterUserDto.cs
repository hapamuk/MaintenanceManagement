using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class RegisterUserDto
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
        /// Kayıt ve şifre sıfırlama için zorunludur.
        /// </summary>
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(
            ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-Posta")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Sisteme giriş için seçilen kullanıcı adı.
        /// </summary>
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının telefon numarası.
        /// </summary>
        [Phone(
            ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        /// <summary>
        /// Kullanıcının birincil
        /// organizasyon birimi talebi.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon birimi seçiniz.")]
        [Display(Name = "Organizasyon Birimi")]
        public int PrimaryUnitId { get; set; }

        /// <summary>
        /// Kullanıcının hesap şifresi.
        /// </summary>
        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(
            8,
            ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Şifre tekrar alanı.
        /// </summary>
        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [Compare(
            nameof(Password),
            ErrorMessage = "Şifreler eşleşmiyor.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
