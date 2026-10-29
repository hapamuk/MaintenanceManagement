using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RoleDtos
{
    public class CreateRoleDto
    {
        /// <summary>
        /// Rolün adını belirtir.
        /// </summary>
        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [Display(Name = "Rol Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Rolü sistem genelinde tanımlayan kod.
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
        /// Role ait isteğe bağlı açıklama.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
