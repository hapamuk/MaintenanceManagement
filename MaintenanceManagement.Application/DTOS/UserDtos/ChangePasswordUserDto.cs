using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class ChangePasswordUserDto
    {
        /// <summary>
        /// Kullanıcının mevcut şifresi.
        /// </summary>
        [Required(ErrorMessage = "Mevcut şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının belirlemek istediği yeni şifre.
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
        [Required(ErrorMessage = "Yeni şifre tekrarı zorunludur.")]
        [Compare(
            nameof(NewPassword),
            ErrorMessage = "Yeni şifreler eşleşmiyor.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
