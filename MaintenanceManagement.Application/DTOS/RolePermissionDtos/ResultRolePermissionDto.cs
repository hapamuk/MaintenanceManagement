using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RolePermissionDtos
{
    public class ResultRolePermissionDto
    {
        /// <summary>
        /// Rol-yetki ilişkisinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Rol Yetki ID")]
        public int Id { get; set; }

        /// <summary>
        /// İlişkili rolün ID bilgisidir.
        /// </summary>
        [Display(Name = "Rol ID")]
        public int RoleId { get; set; }

        /// <summary>
        /// Yetkinin atandığı rolün adıdır.
        /// </summary>
        [Display(Name = "Rol Adı")]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin atandığı rolün kodudur.
        /// </summary>
        [Display(Name = "Rol Kodu")]
        public string RoleCode { get; set; } = string.Empty;

        /// <summary>
        /// İlişkili yetkinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Yetki ID")]
        public int PermissionId { get; set; }

        /// <summary>
        /// Role atanan yetkinin adıdır.
        /// </summary>
        [Display(Name = "Yetki Adı")]
        public string PermissionName { get; set; } = string.Empty;

        /// <summary>
        /// Role atanan yetkinin kodudur.
        /// </summary>
        [Display(Name = "Yetki Kodu")]
        public string PermissionCode { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin ait olduğu modüldür.
        /// </summary>
        [Display(Name = "Modül")]
        public string PermissionModule { get; set; } = string.Empty;

        /// <summary>
        /// İlişkili rolün aktiflik durumudur.
        ///
        /// RolePermission durumunu değil,
        /// Role.IsActive değerini gösterir.
        /// </summary>
        [Display(Name = "Rol Durumu")]
        public bool RoleIsActive { get; set; }

        /// <summary>
        /// İlişkili yetkinin aktiflik durumudur.
        ///
        /// RolePermission durumunu değil,
        /// Permission.IsActive değerini gösterir.
        /// </summary>
        [Display(Name = "Yetki Durumu")]
        public bool PermissionIsActive { get; set; }

        /// <summary>
        /// İlişkinin oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// İlişkinin son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
