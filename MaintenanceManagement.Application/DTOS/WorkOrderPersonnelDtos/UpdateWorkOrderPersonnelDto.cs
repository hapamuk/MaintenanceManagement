using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderPersonnelDtos
{
    public class UpdateWorkOrderPersonnelDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Personelin iş emrindeki görev rolüdür.
        /// </summary>
        [EnumDataType(
            typeof(AssignmentRoleEnum),
            ErrorMessage = "Geçerli bir görev rolü seçiniz.")]
        [Display(Name = "Görev Rolü")]
        public AssignmentRoleEnum AssignmentRole { get; set; }
            = AssignmentRoleEnum.Responsible;

        /// <summary>
        /// Personel atamasına ilişkin
        /// güncel açıklama bilgisidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
