using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RolePermissionDtos
{
    public class UpdateRolePermissionDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Güncellenecek rolün ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir rol seçiniz.")]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }

        /// <summary>
        /// Güncellenecek yetkinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir yetki seçiniz.")]
        [Display(Name = "Yetki")]
        public int PermissionId { get; set; }
    }
}
