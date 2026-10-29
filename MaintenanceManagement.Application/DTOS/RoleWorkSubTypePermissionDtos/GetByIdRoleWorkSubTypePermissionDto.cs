using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RoleWorkSubTypePermissionDtos
{
    public class GetByIdRoleWorkSubTypePermissionDto
    {
        /// <summary>
        /// Yetki ataması kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Yetki Ataması ID")]
        public int Id { get; set; }

        /// <summary>
        /// İlişkili rolün ID bilgisidir.
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
        /// İlişkili rolün aktiflik durumudur.
        /// </summary>
        [Display(Name = "Rol Durumu")]
        public bool RoleIsActive { get; set; }

        /// <summary>
        /// İş alt türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Alt Türü ID")]
        public int WorkSubTypeId { get; set; }

        /// <summary>
        /// İş alt türünün adıdır.
        /// </summary>
        [Display(Name = "İş Alt Türü Adı")]
        public string WorkSubTypeName { get; set; } = string.Empty;

        /// <summary>
        /// İş alt türünün açıklamasıdır.
        /// </summary>
        [Display(Name = "İş Alt Türü Açıklaması")]
        public string? WorkSubTypeDescription { get; set; }

        /// <summary>
        /// İş alt türünün aktiflik durumudur.
        /// </summary>
        [Display(Name = "İş Alt Türü Durumu")]
        public bool WorkSubTypeIsActive { get; set; }

        /// <summary>
        /// İş alt türünün bağlı olduğu
        /// iş türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Türü ID")]
        public int WorkTypeId { get; set; }

        /// <summary>
        /// İş alt türünün bağlı olduğu
        /// iş türünün adıdır.
        /// </summary>
        [Display(Name = "İş Türü")]
        public string WorkTypeName { get; set; } = string.Empty;

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
        /// Yetkinin ait olduğu modül bilgisidir.
        /// </summary>
        [Display(Name = "Modül")]
        public string PermissionModule { get; set; } = string.Empty;

        /// <summary>
        /// Atanan yetkinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Yetki Açıklaması")]
        public string? PermissionDescription { get; set; }

        /// <summary>
        /// İlişkili yetkinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Yetki Durumu")]
        public bool PermissionIsActive { get; set; }

        /// <summary>
        /// Rol-iş alt türü-yetki atamasının
        /// kendi aktiflik durumudur.
        /// </summary>
        [Display(Name = "Atama Durumu")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Atamanın oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Atamanın son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
