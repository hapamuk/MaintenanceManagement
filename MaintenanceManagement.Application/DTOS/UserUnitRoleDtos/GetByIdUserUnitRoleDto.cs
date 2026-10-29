using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserUnitRoleDtos
{
    public class GetByIdUserUnitRoleDto
    {
        /// <summary>
        /// Rol atamasının ID bilgisidir.
        /// </summary>
        [Display(Name = "Rol Ataması ID")]
        public int Id { get; set; }

        /// <summary>
        /// İlişkili kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Kullanıcının adı ve soyadıdır.
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
        /// Kullanıcının görev veya unvanıdır.
        /// </summary>
        [Display(Name = "Unvan")]
        public string? UserTitle { get; set; }

        /// <summary>
        /// Kullanıcının birincil organizasyon
        /// biriminin ID bilgisidir.
        ///
        /// User.PrimaryUnitId üzerinden getirilir.
        /// </summary>
        [Display(Name = "Birincil Birim ID")]
        public int UserPrimaryUnitId { get; set; }

        /// <summary>
        /// Kullanıcının aktiflik durumudur.
        /// </summary>
        [Display(Name = "Kullanıcı Durumu")]
        public bool UserIsActive { get; set; }

        /// <summary>
        /// Kullanıcının sisteme erişim yeteneğidir.
        /// </summary>
        [Display(Name = "Sistem Erişimi")]
        public bool UserHasSystemAccess { get; set; }

        /// <summary>
        /// Kullanıcıya atanan rolün ID bilgisidir.
        /// </summary>
        [Display(Name = "Rol ID")]
        public int RoleId { get; set; }

        /// <summary>
        /// Atanan rolün adıdır.
        /// </summary>
        [Display(Name = "Rol Adı")]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Atanan rolün kodudur.
        /// </summary>
        [Display(Name = "Rol Kodu")]
        public string RoleCode { get; set; } = string.Empty;

        /// <summary>
        /// Atanan rolün açıklamasıdır.
        /// </summary>
        [Display(Name = "Rol Açıklaması")]
        public string? RoleDescription { get; set; }

        /// <summary>
        /// Rolün aktiflik durumudur.
        /// </summary>
        [Display(Name = "Rol Durumu")]
        public bool RoleIsActive { get; set; }

        /// <summary>
        /// Rolün bağlı olduğu organizasyon
        /// biriminin ID bilgisidir.
        ///
        /// Role.UnitId üzerinden getirilir.
        /// </summary>
        [Display(Name = "Rolün Birim ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Rolün bağlı olduğu organizasyon
        /// biriminin adıdır.
        /// </summary>
        [Display(Name = "Rolün Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Rolün bağlı olduğu organizasyon
        /// biriminin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Birim Durumu")]
        public bool UnitIsActive { get; set; }

        /// <summary>
        /// Kullanıcıya rolün atandığı UTC tarihtir.
        /// </summary>
        [Display(Name = "Rol Atanma Tarihi")]
        public DateTime AssignedAt { get; set; }

        /// <summary>
        /// Kullanıcı-rol atamasının
        /// kendi aktiflik durumudur.
        /// </summary>
        [Display(Name = "Atama Durumu")]
        public bool IsActive { get; set; }

        /// <summary>
        /// İlişki kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// İlişki kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
