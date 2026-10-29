using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class LoginResultUserDto
    {
        /// <summary>
        /// Giriş yapan kullanıcının ID bilgisi.
        /// </summary>
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Kullanıcının adı ve soyadı.
        /// </summary>
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının giriş adı.
        /// </summary>
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        /// <summary>
        /// Kullanıcının e-posta adresi.
        /// </summary>
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        /// <summary>
        /// Kullanıcının birincil
        /// organizasyon biriminin ID bilgisi.
        /// </summary>
        [Display(Name = "Organizasyon Birimi ID")]
        public int PrimaryUnitId { get; set; }

        /// <summary>
        /// Kimlik doğrulama sonucunda
        /// üretilen JWT erişim tokenı.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Token türü.
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Erişim tokenının UTC
        /// bitiş tarihi.
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }
    }
}
