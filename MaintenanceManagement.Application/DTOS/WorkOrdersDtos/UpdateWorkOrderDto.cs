using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrdersDtos
{
    public class UpdateWorkOrderDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// İş emrinin başlığıdır.
        /// </summary>
        [Required(
            ErrorMessage = "İş emri başlığı zorunludur.")]
        [StringLength(
            200,
            ErrorMessage = "İş emri başlığı en fazla 200 karakter olabilir.")]
        [Display(Name = "İş Emri Başlığı")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin açıklamasıdır.
        /// </summary>
        [StringLength(
            2000,
            ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
        [Display(Name = "İş Emri Açıklaması")]
        public string? Description { get; set; }

        /// <summary>
        /// İlgili demirbaşın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir demirbaş seçiniz.")]
        [Display(Name = "Demirbaş")]
        public int? AssetId { get; set; }

        /// <summary>
        /// İlişkili iş talebinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş talebi seçiniz.")]
        [Display(Name = "İş Talebi")]
        public int? WorkRequestId { get; set; }

        /// <summary>
        /// İlişkili planlı bakımın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir bakım planı seçiniz.")]
        [Display(Name = "Planlı Bakım")]
        public int? PlannedMaintenanceId { get; set; }

        /// <summary>
        /// İş alt türünün ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş alt türü seçiniz.")]
        [Display(Name = "İş Alt Türü")]
        public int WorkSubTypeId { get; set; }

        /// <summary>
        /// Sorumlu organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir sorumlu birim seçiniz.")]
        [Display(Name = "Sorumlu Birim")]
        public int ResponsibleUnitId { get; set; }

        /// <summary>
        /// Sorumlu ekibin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir ekip seçiniz.")]
        [Display(Name = "Sorumlu Ekip")]
        public int TeamId { get; set; }

        /// <summary>
        /// İş emrinin öncelik seviyesidir.
        /// </summary>
        [EnumDataType(
            typeof(WorkPriorityEnum),
            ErrorMessage = "Geçerli bir öncelik seçiniz.")]
        [Display(Name = "Öncelik")]
        public WorkPriorityEnum Priority { get; set; }
            = WorkPriorityEnum.Normal;

        /// <summary>
        /// Planlanan başlangıç tarihidir.
        /// </summary>
        [Display(Name = "Planlanan Başlangıç Tarihi")]
        public DateTime? PlannedStartDate { get; set; }

        /// <summary>
        /// Planlanan bitiş tarihidir.
        /// </summary>
        [Display(Name = "Planlanan Bitiş Tarihi")]
        public DateTime? PlannedEndDate { get; set; }

        /// <summary>
        /// Yapılan işin sonuç açıklamasıdır.
        /// Yetkilendirilmiş güncelleme işlemlerinde
        /// kullanılmalıdır.
        /// </summary>
        [StringLength(
            2000,
            ErrorMessage = "Sonuç açıklaması en fazla 2000 karakter olabilir.")]
        [Display(Name = "Sonuç Açıklaması")]
        public string? ResultDescription { get; set; }
    }
}
