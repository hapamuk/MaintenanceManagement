using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkSubTypeDtos
{
    public class GetByIdWorkSubTypeDto
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
        /// Bağlı olduğu iş türünün adıdır.
        /// </summary>
        [Display(Name = "İş Türü")]
        public string WorkTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Bağlı olduğu iş türünün açıklamasıdır.
        /// </summary>
        [Display(Name = "İş Türü Açıklaması")]
        public string? WorkTypeDescription { get; set; }

        /// <summary>
        /// Bağlı olduğu iş türünün
        /// aktiflik durumudur.
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
        /// İş türü ve iş alt türünün
        /// birlikte aktif olup olmadığını gösterir.
        /// </summary>
        [Display(Name = "Etkin Durum")]
        public bool IsEffectivelyActive =>
            IsActive && WorkTypeIsActive;

        /// <summary>
        /// İş alt türüne bağlı toplam iş
        /// talebi sayısıdır.
        ///
        /// Bütün talep durumları dahildir.
        /// </summary>
        [Display(Name = "Toplam İş Talebi Sayısı")]
        public int WorkRequestCount { get; set; }

        /// <summary>
        /// İş alt türüne bağlı toplam
        /// iş emri sayısıdır.
        ///
        /// Açık ve tamamlanmış iş emirleri dahildir.
        /// </summary>
        [Display(Name = "Toplam İş Emri Sayısı")]
        public int WorkOrderCount { get; set; }

        /// <summary>
        /// İş alt türüne bağlı toplam
        /// planlı bakım tanımı sayısıdır.
        /// </summary>
        [Display(Name = "Planlı Bakım Sayısı")]
        public int PlannedMaintenanceCount { get; set; }

        /// <summary>
        /// İş alt türü için tanımlanmış toplam
        /// rol-yetki ilişkisi sayısıdır.
        ///
        /// Aktif ve pasif ilişkiler dahildir.
        /// </summary>
        [Display(Name = "Rol Yetki İlişkisi Sayısı")]
        public int RolePermissionCount { get; set; }

        /// <summary>
        /// İş alt türündeki aktif
        /// rol-yetki ilişkisi sayısıdır.
        ///
        /// Yalnızca ilişki kaydının IsActive
        /// değeri üzerinden hesaplanır.
        /// </summary>
        [Display(Name = "Aktif Rol Yetki İlişkisi Sayısı")]
        public int ActiveRolePermissionCount { get; set; }

        /// <summary>
        /// İş alt türünün oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// İş alt türünün son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
