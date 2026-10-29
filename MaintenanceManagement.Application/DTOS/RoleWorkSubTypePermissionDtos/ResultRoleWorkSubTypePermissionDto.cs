using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RoleWorkSubTypePermissionDtos
{
    public class ResultRoleWorkSubTypePermissionDto
    {
        /// <summary>
        /// İlişki kaydının ID bilgisidir.
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
        /// İş alt türünün bağlı olduğu
        /// ana iş türünün adıdır.
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
        /// Yetkinin ait olduğu modüldür.
        /// </summary>
        [Display(Name = "Modül")]
        public string PermissionModule { get; set; } = string.Empty;

        /// <summary>
        /// Rol-iş alt türü-yetki ilişkisinin
        /// aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
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
