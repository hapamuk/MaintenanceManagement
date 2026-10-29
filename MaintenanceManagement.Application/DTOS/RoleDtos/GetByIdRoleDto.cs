using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RoleDtos
{
    public class GetByIdRoleDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Rolün adı.
        /// </summary>
        [Display(Name = "Rol Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Rolün kodu.
        /// </summary>
        [Display(Name = "Rol Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Rolün açıklaması.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Rolün bağlı olduğu organizasyonun ID bilgisi.
        /// </summary>
        [Display(Name = "Organizasyon Birimi ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Rolün bağlı olduğu organizasyonun adı.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Rolün aktif veya pasif olması.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Role aktif şekilde atanmış
        /// farklı kullanıcı sayısı.
        /// </summary>
        [Display(Name = "Aktif Kullanıcı Sayısı")]
        public int ActiveUserCount { get; set; }

        /// <summary>
        /// Role tanımlanan aktif
        /// genel yetkilerin sayısı.
        /// </summary>
        [Display(Name = "Aktif Yetki Sayısı")]
        public int PermissionCount { get; set; }

        /// <summary>
        /// Role tanımlanan aktif iş alt türü
        /// yetkilerinin sayısı.
        /// </summary>
        [Display(Name = "İş Alt Türü Yetki Sayısı")]
        public int WorkSubTypePermissionCount { get; set; }

        /// <summary>
        /// Rolün oluşturulma tarihi.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Rolün son güncellenme tarihi.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
