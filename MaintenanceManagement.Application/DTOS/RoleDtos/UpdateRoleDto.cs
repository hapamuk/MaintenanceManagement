using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RoleDtos
{
    public class UpdateRoleDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Güncellenecek rol adı.
        /// </summary>
        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [Display(Name = "Rol Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Güncellenecek rol kodu.
        /// </summary>
        [Required(ErrorMessage = "Rol kodu zorunludur.")]
        [Display(Name = "Rol Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Rolün bağlı olduğu organizasyon birimi.
        /// </summary>
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon birimi seçiniz.")]
        [Display(Name = "Organizasyon Birimi")]
        public int UnitId { get; set; }

        /// <summary>
        /// Rolün açıklama bilgisi.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
