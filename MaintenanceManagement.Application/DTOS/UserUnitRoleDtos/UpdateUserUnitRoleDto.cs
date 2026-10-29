using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UserUnitRoleDtos
{
    public class UpdateUserUnitRoleDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Rol ataması yapılacak
        /// kullanıcının ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir kullanıcı seçiniz.")]
        [Display(Name = "Kullanıcı")]
        public int UserId { get; set; }

        /// <summary>
        /// Kullanıcıya atanacak yeni rolün
        /// ID bilgisidir.
        ///
        /// Rolün organizasyon birimi
        /// Role.UnitId üzerinden belirlenir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir rol seçiniz.")]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }
    }
}
