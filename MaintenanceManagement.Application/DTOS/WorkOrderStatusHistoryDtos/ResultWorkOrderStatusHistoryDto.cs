using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderStatusHistoryDtos
{
    public class ResultWorkOrderStatusHistoryDto
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
        /// İş emrinin benzersiz numarasıdır.
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
        /// İş emrinin önceki durumudur.
        /// İlk durum kaydında null olabilir.
        /// </summary>
        [Display(Name = "Önceki Durum")]
        public WorkStatusEnum? PreviousStatus { get; set; }

        /// <summary>
        /// Önceki durumun Türkçe açıklamasıdır.
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
        /// İş emrinin geçiş yaptığı yeni durumdur.
        /// </summary>
        [Display(Name = "Yeni Durum")]
        public WorkStatusEnum NewStatus { get; set; }

        /// <summary>
        /// Yeni durumun Türkçe açıklamasıdır.
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
        /// Durum değişikliğinin gerçekleştirildiği tarihtir.
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
        /// Kaydın güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
