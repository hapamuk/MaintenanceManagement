using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserDtos
{
    public class GetByIdUserDto
    {
        /// <summary>
        /// Kullanıcı ID bilgisi.
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
        /// Kullanıcının tam adı.
        /// </summary>
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının e-posta adresi.
        /// </summary>
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        /// <summary>
        /// Kullanıcı adı.
        /// </summary>
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        /// <summary>
        /// Telefon numarası.
        /// </summary>
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        /// <summary>
        /// Unvan bilgisi.
        /// </summary>
        [Display(Name = "Unvan")]
        public string? Title { get; set; }

        /// <summary>
        /// Kullanıcının birincil biriminin ID bilgisi.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int PrimaryUnitId { get; set; }

        /// <summary>
        /// Kullanıcının birincil organizasyon birimi.
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
        /// Kullanıcıya atanmış aktif
        /// birim-rol ilişkilerinin sayısı.
        /// </summary>
        [Display(Name = "Aktif Rol Ataması Sayısı")]
        public int ActiveRoleAssignmentCount { get; set; }

        /// <summary>
        /// Kullanıcının aktif ekip
        /// üyeliklerinin sayısı.
        /// </summary>
        [Display(Name = "Aktif Ekip Üyeliği Sayısı")]
        public int ActiveTeamMembershipCount { get; set; }

        /// <summary>
        /// Kullanıcının oluşturulma tarihi.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Son güncellenme tarihi.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
