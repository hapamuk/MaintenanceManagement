using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.TeamMemberDtos
{
    public class GetByIdTeamMemberDto
    {
        /// <summary>
        /// Ekip üyeliğinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip Üyeliği ID")]
        public int Id { get; set; }

        /// <summary>
        /// Ekibin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip ID")]
        public int TeamId { get; set; }

        /// <summary>
        /// Ekibin adıdır.
        /// </summary>
        [Display(Name = "Ekip Adı")]
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// Ekibin kodudur.
        /// </summary>
        [Display(Name = "Ekip Kodu")]
        public string TeamCode { get; set; } = string.Empty;

        /// <summary>
        /// Ekibin açıklama bilgisidir.
        /// </summary>
        [Display(Name = "Ekip Açıklaması")]
        public string? TeamDescription { get; set; }

        /// <summary>
        /// Ekibin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Ekip Durumu")]
        public bool TeamIsActive { get; set; }

        /// <summary>
        /// Ekibin organizasyon biriminin
        /// ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Ekibin organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin
        /// aktiflik durumudur.
        /// </summary>
        [Display(Name = "Birim Durumu")]
        public bool UnitIsActive { get; set; }

        /// <summary>
        /// Ekip üyesinin kullanıcı ID bilgisidir.
        /// </summary>
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Ekip üyesinin adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Ad Soyad")]
        public string UserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının giriş adıdır.
        /// </summary>
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        /// <summary>
        /// Kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "E-Posta")]
        public string? UserEmail { get; set; }

        /// <summary>
        /// Kullanıcının telefon numarasıdır.
        /// </summary>
        [Display(Name = "Telefon")]
        public string? UserPhone { get; set; }

        /// <summary>
        /// Kullanıcının görev veya unvanıdır.
        /// </summary>
        [Display(Name = "Unvan")]
        public string? UserTitle { get; set; }

        /// <summary>
        /// Kullanıcının birincil organizasyon
        /// biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Birincil Birim ID")]
        public int UserPrimaryUnitId { get; set; }

        /// <summary>
        /// Kullanıcının aktiflik durumudur.
        /// </summary>
        [Display(Name = "Kullanıcı Durumu")]
        public bool UserIsActive { get; set; }

        /// <summary>
        /// Kullanıcının sistem erişimine
        /// sahip olup olmadığını belirtir.
        /// </summary>
        [Display(Name = "Sistem Erişimi")]
        public bool UserHasSystemAccess { get; set; }

        /// <summary>
        /// Kullanıcının ekibe katılma tarihidir.
        /// </summary>
        [Display(Name = "Katılma Tarihi")]
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Kullanıcının ekipten ayrılma tarihidir.
        ///
        /// Üyeliği devam ediyorsa null olabilir.
        /// </summary>
        [Display(Name = "Ayrılma Tarihi")]
        public DateTime? LeftAt { get; set; }

        /// <summary>
        /// Ekip üyeliğinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Üyelik Durumu")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Üyelik kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Üyelik kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
