using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class ResetPasswordUserDto
    {
        /// <summary>
        /// Şifresi sıfırlanacak hesaba
        /// kayıtlı e-posta adresi.
        /// </summary>
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(
            ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-Posta")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Şifre sıfırlama için oluşturulan
        /// tek kullanımlık güvenli token.
        /// </summary>
        [Required(ErrorMessage = "Sıfırlama kodu zorunludur.")]
        [Display(Name = "Sıfırlama Kodu")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının yeni şifresi.
        /// </summary>
        [Required(ErrorMessage = "Yeni şifre zorunludur.")]
        [MinLength(
            8,
            ErrorMessage = "Yeni şifre en az 8 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Yeni şifrenin tekrar girildiği alan.
        /// </summary>
        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [Compare(
            nameof(NewPassword),
            ErrorMessage = "Şifreler eşleşmiyor.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
