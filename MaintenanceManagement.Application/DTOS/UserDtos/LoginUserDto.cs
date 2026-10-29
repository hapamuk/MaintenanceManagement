using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class LoginUserDto
    {
        /// <summary>
        /// Kullanıcının e-posta adresi
        /// veya kullanıcı adı.
        /// </summary>
        [Required(
            ErrorMessage = "Kullanıcı adı veya e-posta zorunludur.")]
        [Display(Name = "Kullanıcı Adı / E-Posta")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının düz metin şifresi.
        ///
        /// Sadece kimlik doğrulama amacıyla alınır;
        /// veritabanına bu şekilde kaydedilmez.
        /// </summary>
        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;
    }
}
