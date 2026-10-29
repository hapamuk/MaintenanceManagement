using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserUnitRoleDtos
{
    public class ResultUserUnitRoleDto
    {
        /// <summary>
        /// Rol ataması kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Rol Ataması ID")]
        public int Id { get; set; }

        /// <summary>
        /// Rol atanan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Ad Soyad")]
        public string UserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının sisteme giriş adıdır.
        /// </summary>
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        /// <summary>
        /// Kullanıcının aktiflik durumudur.
        /// </summary>
        [Display(Name = "Kullanıcı Durumu")]
        public bool UserIsActive { get; set; }

        /// <summary>
        /// Atanan rolün ID bilgisidir.
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
        /// İlişkili rolün aktiflik durumudur.
        /// </summary>
        [Display(Name = "Rol Durumu")]
        public bool RoleIsActive { get; set; }

        /// <summary>
        /// Rolün bağlı olduğu organizasyon
        /// biriminin ID bilgisidir.
        ///
        /// Role.UnitId üzerinden getirilir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Rolün bağlı olduğu organizasyon
        /// biriminin adıdır.
        ///
        /// Role.Unit.Name üzerinden getirilir.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Rol atamasının yapıldığı UTC tarihtir.
        /// </summary>
        [Display(Name = "Atanma Tarihi")]
        public DateTime AssignedAt { get; set; }

        /// <summary>
        /// Rol atamasının aktif/pasif durumudur.
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
