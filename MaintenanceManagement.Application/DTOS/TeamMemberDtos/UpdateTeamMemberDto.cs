using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.TeamMemberDtos
{
    public class UpdateTeamMemberDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Üyeliğin bağlı olduğu
        /// ekibin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir ekip seçiniz.")]
        [Display(Name = "Ekip")]
        public int TeamId { get; set; }

        /// <summary>
        /// Ekip üyesi olan
        /// kullanıcının ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir kullanıcı seçiniz.")]
        [Display(Name = "Kullanıcı")]
        public int UserId { get; set; }
    }
}
