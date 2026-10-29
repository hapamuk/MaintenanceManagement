using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.RoleWorkSubTypePermissionDtos
{
    public class CreateRoleWorkSubTypePermissionDto
    {
        /// <summary>
        /// Yetkinin atanacağı rolün ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir rol seçiniz.")]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }

        /// <summary>
        /// Yetkinin geçerli olacağı iş
        /// alt türünün ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş alt türü seçiniz.")]
        [Display(Name = "İş Alt Türü")]
        public int WorkSubTypeId { get; set; }

        /// <summary>
        /// Role, seçilen iş alt türü kapsamında
        /// atanacak yetkinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir yetki seçiniz.")]
        [Display(Name = "Yetki")]
        public int PermissionId { get; set; }
    }
}
