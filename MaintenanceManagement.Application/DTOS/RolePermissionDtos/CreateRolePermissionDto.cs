using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RolePermissionDtos
{
    public class CreateRolePermissionDto
    {
        /// <summary>
        /// Yetkinin atanacağı rolün ID bilgisidir.
        /// </summary>
        [Required(
            ErrorMessage = "Rol seçimi zorunludur.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir rol seçiniz.")]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }

        /// <summary>
        /// Role atanacak yetkinin ID bilgisidir.
        /// </summary>
        [Required(
            ErrorMessage = "Yetki seçimi zorunludur.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir yetki seçiniz.")]
        [Display(Name = "Yetki")]
        public int PermissionId { get; set; }
    }
}
