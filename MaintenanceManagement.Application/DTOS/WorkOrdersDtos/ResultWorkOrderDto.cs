using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrdersDtos
{
    public class ResultWorkOrderDto
    {
        /// <summary>
        /// İş emrinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Emri ID")]
        public int Id { get; set; }

        /// <summary>
        /// İş emrinin benzersiz numarasıdır.
        /// </summary>
        [Display(Name = "İş Emri Numarası")]
        public string WorkOrderNumber { get; set; }
            = string.Empty;

        /// <summary>
        /// İş emrinin başlığıdır.
        /// </summary>
        [Display(Name = "İş Emri Başlığı")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// İlgili demirbaşın ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş ID")]
        public int? AssetId { get; set; }

        /// <summary>
        /// İlgili demirbaşın kodudur.
        /// </summary>
        [Display(Name = "Demirbaş Kodu")]
        public string? AssetCode { get; set; }

        /// <summary>
        /// İlgili demirbaşın adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Adı")]
        public string? AssetName { get; set; }

        /// <summary>
        /// İş emrinin bağlı olduğu iş talebidir.
        /// </summary>
        [Display(Name = "İş Talebi ID")]
        public int? WorkRequestId { get; set; }

        /// <summary>
        /// İlgili iş talebinin numarasıdır.
        /// </summary>
        [Display(Name = "Talep Numarası")]
        public string? WorkRequestNumber { get; set; }

        /// <summary>
        /// İş emrinin bağlı olduğu planlı bakım ID'sidir.
        /// </summary>
        [Display(Name = "Planlı Bakım ID")]
        public int? PlannedMaintenanceId { get; set; }

        /// <summary>
        /// İş alt türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Alt Türü ID")]
        public int WorkSubTypeId { get; set; }

        /// <summary>
        /// İş alt türünün adıdır.
        /// </summary>
        [Display(Name = "İş Alt Türü")]
        public string WorkSubTypeName { get; set; }
            = string.Empty;

        /// <summary>
        /// Ana iş türünün adıdır.
        /// </summary>
        [Display(Name = "İş Türü")]
        public string WorkTypeName { get; set; }
            = string.Empty;

        /// <summary>
        /// Sorumlu organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Sorumlu Birim ID")]
        public int ResponsibleUnitId { get; set; }

        /// <summary>
        /// Sorumlu organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Sorumlu Birim")]
        public string ResponsibleUnitName { get; set; }
            = string.Empty;

        /// <summary>
        /// Sorumlu ekibin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip ID")]
        public int TeamId { get; set; }

        /// <summary>
        /// Sorumlu ekibin adıdır.
        /// </summary>
        [Display(Name = "Sorumlu Ekip")]
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// İş emrini oluşturan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı ID")]
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// İş emrini oluşturan kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı")]
        public string CreatedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// İş emrini onaylayan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Onaylayan Kullanıcı ID")]
        public int? ApprovedByUserId { get; set; }

        /// <summary>
        /// İş emrini onaylayan kullanıcının adıdır.
        /// </summary>
        [Display(Name = "Onaylayan Kullanıcı")]
        public string? ApprovedByUserFullName { get; set; }

        /// <summary>
        /// İş emrinin mevcut durumudur.
        /// </summary>
        [Display(Name = "İş Durumu")]
        public WorkStatusEnum Status { get; set; }

        /// <summary>
        /// İş emri durumunun Türkçe açıklamasıdır.
        /// </summary>
        [Display(Name = "Durum Açıklaması")]
        public string StatusName => Status switch
        {
            WorkStatusEnum.New => "Yeni",
            WorkStatusEnum.Cancelled => "İptal Edildi",
            WorkStatusEnum.InProgress => "İşe Başlandı",
            WorkStatusEnum.PendingApproval => "Onaya Gönderildi",
            WorkStatusEnum.Completed => "Tamamlandı",
            _ => "Bilinmiyor"
        };

        /// <summary>
        /// İş emrinin öncelik seviyesidir.
        /// </summary>
        [Display(Name = "Öncelik")]
        public WorkPriorityEnum Priority { get; set; }

        /// <summary>
        /// İş emrinin öncelik açıklamasıdır.
        /// </summary>
        [Display(Name = "Öncelik Açıklaması")]
        public string PriorityName => Priority switch
        {
            WorkPriorityEnum.Low => "Düşük",
            WorkPriorityEnum.Normal => "Normal",
            WorkPriorityEnum.High => "Yüksek",
            WorkPriorityEnum.Critical => "Kritik",
            _ => "Bilinmiyor"
        };

        /// <summary>
        /// Planlanan başlangıç tarihidir.
        /// </summary>
        [Display(Name = "Planlanan Başlangıç")]
        public DateTime? PlannedStartDate { get; set; }

        /// <summary>
        /// Planlanan bitiş tarihidir.
        /// </summary>
        [Display(Name = "Planlanan Bitiş")]
        public DateTime? PlannedEndDate { get; set; }

        /// <summary>
        /// İşe fiilen başlanan tarihtir.
        /// </summary>
        [Display(Name = "İşe Başlama Tarihi")]
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// İşin onaya gönderildiği tarihtir.
        /// </summary>
        [Display(Name = "Onaya Gönderilme Tarihi")]
        public DateTime? SubmittedForApprovalAt { get; set; }

        /// <summary>
        /// İş emrinin tamamlandığı tarihtir.
        /// </summary>
        [Display(Name = "Tamamlanma Tarihi")]
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// İş emrinin iptal edildiği tarihtir.
        /// </summary>
        [Display(Name = "İptal Tarihi")]
        public DateTime? CancelledAt { get; set; }

        /// <summary>
        /// Yapılan işe ilişkin sonuç açıklamasıdır.
        /// </summary>
        [Display(Name = "Sonuç Açıklaması")]
        public string? ResultDescription { get; set; }

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
