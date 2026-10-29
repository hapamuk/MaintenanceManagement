using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RolePermissionDtos
{
    public class GetByIdRolePermissionDto
    {
        /// <summary>
        /// Rol-yetki ilişkisinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Rol Yetki ID")]
        public int Id { get; set; }

        /// <summary>
        /// Yetkinin atandığı rolün ID bilgisidir.
        /// </summary>
        [Display(Name = "Rol ID")]
        public int RoleId { get; set; }

        /// <summary>
        /// İlişkili rolün adıdır.
        /// </summary>
        [Display(Name = "Rol Adı")]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// İlişkili rolün kodudur.
        /// </summary>
        [Display(Name = "Rol Kodu")]
        public string RoleCode { get; set; } = string.Empty;

        /// <summary>
        /// İlişkili rolün açıklamasıdır.
        /// </summary>
        [Display(Name = "Rol Açıklaması")]
        public string? RoleDescription { get; set; }

        /// <summary>
        /// Rolün aktiflik durumudur.
        /// </summary>
        [Display(Name = "Rol Durumu")]
        public bool RoleIsActive { get; set; }

        /// <summary>
        /// Atanan yetkinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Yetki ID")]
        public int PermissionId { get; set; }

        /// <summary>
        /// Atanan yetkinin adıdır.
        /// </summary>
        [Display(Name = "Yetki Adı")]
        public string PermissionName { get; set; } = string.Empty;

        /// <summary>
        /// Atanan yetkinin kodudur.
        /// </summary>
        [Display(Name = "Yetki Kodu")]
        public string PermissionCode { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin ait olduğu modüldür.
        /// </summary>
        [Display(Name = "Modül")]
        public string PermissionModule { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Yetki Açıklaması")]
        public string? PermissionDescription { get; set; }

        /// <summary>
        /// Yetkinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Yetki Durumu")]
        public bool PermissionIsActive { get; set; }

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
