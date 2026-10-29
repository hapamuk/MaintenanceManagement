using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkSubTypeDtos
{
    public class ResultWorkSubTypeDto
    {
        /// <summary>
        /// İş alt türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Alt Türü ID")]
        public int Id { get; set; }

        /// <summary>
        /// Bağlı olduğu iş türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Türü ID")]
        public int WorkTypeId { get; set; }

        /// <summary>
        /// İş alt türünün bağlı olduğu
        /// iş türünün adıdır.
        ///
        /// WorkSubType.WorkType.Name üzerinden getirilir.
        /// </summary>
        [Display(Name = "İş Türü")]
        public string WorkTypeName { get; set; } = string.Empty;

        /// <summary>
        /// İş türünün aktiflik durumudur.
        /// </summary>
        [Display(Name = "İş Türü Durumu")]
        public bool WorkTypeIsActive { get; set; }

        /// <summary>
        /// İş alt türünün adıdır.
        /// </summary>
        [Display(Name = "İş Alt Türü")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// İş alt türünün açıklamasıdır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// İş alt türünün aktiflik durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// İş türü ve alt türün birlikte
        /// aktif olup olmadığını gösterir.
        /// </summary>
        [Display(Name = "Etkin Durum")]
        public bool IsEffectivelyActive =>
            IsActive && WorkTypeIsActive;

        /// <summary>
        /// Kaydın oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kaydın son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
