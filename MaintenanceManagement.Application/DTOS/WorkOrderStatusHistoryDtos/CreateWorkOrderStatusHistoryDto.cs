using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderStatusHistoryDtos
{
    public class CreateWorkOrderStatusHistoryDto
    {
        /// <summary>
        /// Durumu değiştirilen iş emrinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş emri seçiniz.")]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// İş emrinin geçeceği yeni durumdur.
        /// </summary>
        [EnumDataType(
            typeof(WorkStatusEnum),
            ErrorMessage = "Geçerli bir iş durumu seçiniz.")]
        [Display(Name = "Yeni Durum")]
        public WorkStatusEnum NewStatus { get; set; }
            = WorkStatusEnum.New;

        /// <summary>
        /// Durum değişikliğinin nedenini
        /// veya açıklamasını belirtir.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Durum Değişikliği Açıklaması")]
        public string? Description { get; set; }
    }
}
