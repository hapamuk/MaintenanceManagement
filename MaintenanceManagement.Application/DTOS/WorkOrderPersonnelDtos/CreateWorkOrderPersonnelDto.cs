using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderPersonnelDtos
{
    public class CreateWorkOrderPersonnelDto
    {
        /// <summary>
        /// Personelin atanacağı iş emrinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş emri seçiniz.")]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// Atanacak ekip üyesinin ID bilgisidir.
        /// TeamMember tablosundaki kaydı temsil eder.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir ekip üyesi seçiniz.")]
        [Display(Name = "Ekip Üyesi")]
        public int TeamMemberId { get; set; }

        /// <summary>
        /// Personelin iş emrindeki görev rolüdür.
        /// Varsayılan değer Sorumlu Personel'dir.
        /// </summary>
        [EnumDataType(
            typeof(AssignmentRoleEnum),
            ErrorMessage = "Geçerli bir görev rolü seçiniz.")]
        [Display(Name = "Görev Rolü")]
        public AssignmentRoleEnum AssignmentRole { get; set; }
            = AssignmentRoleEnum.Responsible;

        /// <summary>
        /// Personel atamasına ilişkin
        /// isteğe bağlı açıklamadır.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
