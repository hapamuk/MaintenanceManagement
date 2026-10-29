using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class UserProfileDto
    {
        /// <summary>
        /// Kullanıcının ID bilgisi.
        /// </summary>
        [Display(Name = "Kullanıcı ID")]
        public int Id { get; set; }

        /// <summary>
        /// Kullanıcının adı.
        /// </summary>
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının soyadı.
        /// </summary>
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının ad ve soyadı.
        /// </summary>
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının e-posta adresi.
        /// </summary>
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        /// <summary>
        /// Kullanıcının giriş adı.
        /// </summary>
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        /// <summary>
        /// Kullanıcının telefon numarası.
        /// </summary>
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        /// <summary>
        /// Kullanıcının görev veya unvanı.
        /// </summary>
        [Display(Name = "Unvan")]
        public string? Title { get; set; }

        /// <summary>
        /// Birincil organizasyon biriminin ID bilgisi.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int PrimaryUnitId { get; set; }

        /// <summary>
        /// Birincil organizasyon biriminin adı.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string PrimaryUnitName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının sistem erişimi durumu.
        /// </summary>
        [Display(Name = "Sistem Erişimi")]
        public bool HasSystemAccess { get; set; }

        /// <summary>
        /// Kullanıcının aktiflik durumu.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Hesabın oluşturulma tarihi.
        /// </summary>
        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreatedAt { get; set; }
    }
}
