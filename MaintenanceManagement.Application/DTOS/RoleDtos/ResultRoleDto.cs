using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RoleDtos
{
    public class ResultRoleDto
    {
        /// <summary>
        /// Rolün ID bilgisi.
        /// </summary>
        [Display(Name = "Rol ID")]
        public int Id { get; set; }

        /// <summary>
        /// Rol adı.
        /// </summary>
        [Display(Name = "Rol Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Rol kodu.
        /// </summary>
        [Display(Name = "Rol Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Rolün açıklaması.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Rolün bağlı olduğu organizasyon biriminin ID değeri.
        /// </summary>
        [Display(Name = "Organizasyon Birimi ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Rolün bağlı olduğu organizasyon biriminin adı.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Rolün aktiflik durumu.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

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
