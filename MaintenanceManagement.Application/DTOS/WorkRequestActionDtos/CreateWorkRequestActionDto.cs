using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkRequestActionDtos
{
    public class CreateWorkRequestActionDto
    {
        /// <summary>
        /// İşlemin ait olduğu iş talebinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş talebi seçiniz.")]
        [Display(Name = "İş Talebi")]
        public int WorkRequestId { get; set; }

        /// <summary>
        /// İş talebi üzerinde gerçekleştirilen işlem türüdür.
        /// </summary>
        [EnumDataType(
            typeof(WorkRequestActionTypeEnum),
            ErrorMessage = "Geçerli bir işlem türü seçiniz.")]
        [Display(Name = "İşlem Türü")]
        public WorkRequestActionTypeEnum ActionType { get; set; }

        /// <summary>
        /// Gerçekleştirilen işlem hakkında açıklama bilgisidir.
        /// </summary>
        [StringLength(
            2000,
            ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
        [Display(Name = "İşlem Açıklaması")]
        public string? Description { get; set; }
    }
}
