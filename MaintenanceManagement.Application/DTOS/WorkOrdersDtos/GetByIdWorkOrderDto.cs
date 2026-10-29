using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrdersDtos
{
    public class GetByIdWorkOrderDto
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
        /// Demirbaşın kodudur.
        /// </summary>
        [Display(Name = "Demirbaş Kodu")]
        public string? AssetCode { get; set; }

        /// <summary>
        /// Demirbaşın adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Adı")]
        public string? AssetName { get; set; }

        /// <summary>
        /// İlgili iş talebinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Talebi ID")]
        public int? WorkRequestId { get; set; }

        /// <summary>
        /// İlgili iş talebinin numarasıdır.
        /// </summary>
        [Display(Name = "Talep Numarası")]
        public string? WorkRequestNumber { get; set; }

        /// <summary>
        /// İlgili iş talebinin başlığıdır.
        /// </summary>
        [Display(Name = "Talep Başlığı")]
        public string? WorkRequestTitle { get; set; }

        /// <summary>
        /// İlişkili planlı bakımın ID bilgisidir.
        /// </summary>
        [Display(Name = "Planlı Bakım ID")]
        public int? PlannedMaintenanceId { get; set; }

        /// <summary>
        /// Ana iş türünün ID bilgisidir.
        /// WorkSubType üzerinden elde edilir.
        /// </summary>
        [Display(Name = "İş Türü ID")]
        public int WorkTypeId { get; set; }

        /// <summary>
        /// Ana iş türünün adıdır.
        /// </summary>
        [Display(Name = "İş Türü")]
        public string WorkTypeName { get; set; }
            = string.Empty;

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
        /// İş alt türünün açıklamasıdır.
        /// </summary>
        [Display(Name = "İş Alt Türü Açıklaması")]
        public string? WorkSubTypeDescription { get; set; }

        /// <summary>
        /// Sorumlu birimin ID bilgisidir.
        /// </summary>
        [Display(Name = "Sorumlu Birim ID")]
        public int ResponsibleUnitId { get; set; }

        /// <summary>
        /// Sorumlu birimin adıdır.
        /// </summary>
        [Display(Name = "Sorumlu Birim")]
        public string ResponsibleUnitName { get; set; }
            = string.Empty;

        /// <summary>
        /// Sorumlu birimin kodudur.
        /// </summary>
        [Display(Name = "Sorumlu Birim Kodu")]
        public string ResponsibleUnitCode { get; set; }
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
        public string TeamName { get; set; }
            = string.Empty;

        /// <summary>
        /// İş emrini oluşturan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı ID")]
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// İş emrini oluşturan kullanıcının
        /// adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı")]
        public string CreatedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// İş emrini oluşturan kullanıcının
        /// e-posta adresidir.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı E-Posta")]
        public string? CreatedByUserEmail { get; set; }

        /// <summary>
        /// Onaylayan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Onaylayan Kullanıcı ID")]
        public int? ApprovedByUserId { get; set; }

        /// <summary>
        /// Onaylayan kullanıcının adıdır.
        /// </summary>
        [Display(Name = "Onaylayan Kullanıcı")]
        public string? ApprovedByUserFullName { get; set; }

        /// <summary>
        /// Onaylayan kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "Onaylayan Kullanıcı E-Posta")]
        public string? ApprovedByUserEmail { get; set; }

        /// <summary>
        /// İş emrinin durumudur.
        /// </summary>
        [Display(Name = "İş Durumu")]
        public WorkStatusEnum Status { get; set; }

        /// <summary>
        /// İş emri durumunun Türkçe karşılığıdır.
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
        /// Önceliğin Türkçe karşılığıdır.
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
        [Display(Name = "Planlanan Başlangıç Tarihi")]
        public DateTime? PlannedStartDate { get; set; }

        /// <summary>
        /// Planlanan bitiş tarihidir.
        /// </summary>
        [Display(Name = "Planlanan Bitiş Tarihi")]
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
        /// İşin tamamlandığı tarihtir.
        /// </summary>
        [Display(Name = "Tamamlanma Tarihi")]
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// İşin iptal edildiği tarihtir.
        /// </summary>
        [Display(Name = "İptal Tarihi")]
        public DateTime? CancelledAt { get; set; }

        /// <summary>
        /// Yapılan işin sonuç açıklamasıdır.
        /// </summary>
        [Display(Name = "Sonuç Açıklaması")]
        public string? ResultDescription { get; set; }

        /// <summary>
        /// İş emrine atanmış personel kayıt sayısıdır.
        /// </summary>
        [Display(Name = "Personel Sayısı")]
        public int PersonnelCount { get; set; }

        /// <summary>
        /// İş emrine bağlı malzeme kayıt sayısıdır.
        /// </summary>
        [Display(Name = "Malzeme Kayıt Sayısı")]
        public int MaterialCount { get; set; }

        /// <summary>
        /// İş emri işlem geçmişi kayıt sayısıdır.
        /// </summary>
        [Display(Name = "İşlem Sayısı")]
        public int ActionCount { get; set; }

        /// <summary>
        /// İş emri durum geçmişi kayıt sayısıdır.
        /// </summary>
        [Display(Name = "Durum Geçmişi Sayısı")]
        public int StatusHistoryCount { get; set; }

        /// <summary>
        /// İş emrine eklenen dosya sayısıdır.
        /// </summary>
        [Display(Name = "Dosya Sayısı")]
        public int FileCount { get; set; }

        /// <summary>
        /// Kontrol listesi sonuç kayıt sayısıdır.
        /// </summary>
        [Display(Name = "Kontrol Listesi Sonuç Sayısı")]
        public int ChecklistResultCount { get; set; }

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
