using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderStatusHistoryDtos
{
    public class GetByIdWorkOrderStatusHistoryDto
    {
        /// <summary>
        /// Durum geçmişi kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Durum Geçmişi ID")]
        public int Id { get; set; }

        /// <summary>
        /// İlgili iş emrinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Emri ID")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// İş emrinin numarasıdır.
        /// </summary>
        [Display(Name = "İş Emri Numarası")]
        public string WorkOrderNumber { get; set; }
            = string.Empty;

        /// <summary>
        /// İş emrinin başlığıdır.
        /// </summary>
        [Display(Name = "İş Emri Başlığı")]
        public string WorkOrderTitle { get; set; }
            = string.Empty;

        /// <summary>
        /// İş emrinin açıklamasıdır.
        /// </summary>
        [Display(Name = "İş Emri Açıklaması")]
        public string? WorkOrderDescription { get; set; }

        /// <summary>
        /// İş emrinin mevcut durumudur.
        /// Geçmiş kaydındaki yeni durumdan farklı olabilir.
        /// </summary>
        [Display(Name = "İş Emrinin Güncel Durumu")]
        public WorkStatusEnum WorkOrderCurrentStatus { get; set; }

        /// <summary>
        /// Güncel iş emri durumunun Türkçe açıklamasıdır.
        /// </summary>
        [Display(Name = "Güncel Durum Açıklaması")]
        public string WorkOrderCurrentStatusName =>
            WorkOrderCurrentStatus switch
            {
                WorkStatusEnum.New => "Yeni",
                WorkStatusEnum.Cancelled => "İptal Edildi",
                WorkStatusEnum.InProgress => "İşe Başlandı",
                WorkStatusEnum.PendingApproval => "Onaya Gönderildi",
                WorkStatusEnum.Completed => "Tamamlandı",
                _ => "Bilinmiyor"
            };

        /// <summary>
        /// Durum değişikliğinden önceki durumdur.
        /// İlk kayıtta null olabilir.
        /// </summary>
        [Display(Name = "Önceki Durum")]
        public WorkStatusEnum? PreviousStatus { get; set; }

        /// <summary>
        /// Önceki durumun Türkçe karşılığıdır.
        /// </summary>
        [Display(Name = "Önceki Durum Açıklaması")]
        public string PreviousStatusName => PreviousStatus switch
        {
            WorkStatusEnum.New => "Yeni",
            WorkStatusEnum.Cancelled => "İptal Edildi",
            WorkStatusEnum.InProgress => "İşe Başlandı",
            WorkStatusEnum.PendingApproval => "Onaya Gönderildi",
            WorkStatusEnum.Completed => "Tamamlandı",
            null => "İlk Durum",
            _ => "Bilinmiyor"
        };

        /// <summary>
        /// Durum değişikliğinden sonraki yeni durumdur.
        /// </summary>
        [Display(Name = "Yeni Durum")]
        public WorkStatusEnum NewStatus { get; set; }

        /// <summary>
        /// Yeni durumun Türkçe karşılığıdır.
        /// </summary>
        [Display(Name = "Yeni Durum Açıklaması")]
        public string NewStatusName => NewStatus switch
        {
            WorkStatusEnum.New => "Yeni",
            WorkStatusEnum.Cancelled => "İptal Edildi",
            WorkStatusEnum.InProgress => "İşe Başlandı",
            WorkStatusEnum.PendingApproval => "Onaya Gönderildi",
            WorkStatusEnum.Completed => "Tamamlandı",
            _ => "Bilinmiyor"
        };

        /// <summary>
        /// Durum değişikliğini yapan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "İşlemi Yapan Kullanıcı ID")]
        public int ChangedByUserId { get; set; }

        /// <summary>
        /// Durum değişikliğini yapan kullanıcının
        /// adı ve soyadıdır.
        /// </summary>
        [Display(Name = "İşlemi Yapan")]
        public string ChangedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// Durum değişikliğini yapan kullanıcının
        /// e-posta adresidir.
        /// </summary>
        [Display(Name = "Kullanıcı E-Posta")]
        public string? ChangedByUserEmail { get; set; }

        /// <summary>
        /// Durum değişikliğini yapan kullanıcının unvanıdır.
        /// </summary>
        [Display(Name = "Kullanıcı Unvanı")]
        public string? ChangedByUserTitle { get; set; }

        /// <summary>
        /// Durum değişikliğini yapan kullanıcının
        /// aktiflik durumudur.
        /// </summary>
        [Display(Name = "Kullanıcı Aktif")]
        public bool ChangedByUserIsActive { get; set; }

        /// <summary>
        /// Durum değişikliğinin yapıldığı tarihtir.
        /// </summary>
        [Display(Name = "Değişiklik Tarihi")]
        public DateTime ChangedAt { get; set; }

        /// <summary>
        /// Durum değişikliğiyle ilgili açıklamadır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

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
