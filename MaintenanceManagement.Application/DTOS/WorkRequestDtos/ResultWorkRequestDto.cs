using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkRequestDtos
{
    public class ResultWorkRequestDto
    {
        /// <summary>
        /// İş talebinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Talebi ID")]
        public int Id { get; set; }

        /// <summary>
        /// İş talebinin benzersiz numarasıdır.
        /// </summary>
        [Display(Name = "Talep Numarası")]
        public string RequestNumber { get; set; }
            = string.Empty;

        /// <summary>
        /// İş talebinin başlığıdır.
        /// </summary>
        [Display(Name = "Talep Başlığı")]
        public string Title { get; set; }
            = string.Empty;

        /// <summary>
        /// İş talebinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Talep Açıklaması")]
        public string Description { get; set; }
            = string.Empty;

        /// <summary>
        /// Talebe konu olan demirbaşın ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş ID")]
        public int? AssetId { get; set; }

        /// <summary>
        /// İlgili demirbaşın kodudur.
        /// </summary>
        [Display(Name = "Demirbaş Kodu")]
        public string? AssetCode { get; set; }

        /// <summary>
        /// Talebi oluşturan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Talep Eden Kullanıcı ID")]
        public int RequestedByUserId { get; set; }

        /// <summary>
        /// Talebi oluşturan kullanıcının
        /// adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Talep Eden")]
        public string RequestedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// Talep eden organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Talep Eden Birim ID")]
        public int RequesterUnitId { get; set; }

        /// <summary>
        /// Talep eden organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Talep Eden Birim")]
        public string RequesterUnitName { get; set; }
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
        /// İş alt türünün bağlı olduğu
        /// ana iş türünün adıdır.
        /// </summary>
        [Display(Name = "İş Türü")]
        public string WorkTypeName { get; set; }
            = string.Empty;

        /// <summary>
        /// Sorumlu yöneticinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Sorumlu Yönetici ID")]
        public int? ManagerUserId { get; set; }

        /// <summary>
        /// Sorumlu yöneticinin adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Sorumlu Yönetici")]
        public string? ManagerUserFullName { get; set; }

        /// <summary>
        /// Saha veya lokasyonun ID bilgisidir.
        /// </summary>
        [Display(Name = "Lokasyon ID")]
        public int? SiteLocationId { get; set; }

        /// <summary>
        /// Saha veya lokasyonun adıdır.
        /// </summary>
        [Display(Name = "Lokasyon")]
        public string? SiteLocationName { get; set; }

        /// <summary>
        /// İşin gerçekleştirileceği yer hakkında
        /// ek açıklama bilgisidir.
        /// </summary>
        [Display(Name = "Konum Açıklaması")]
        public string? LocationDescription { get; set; }

        /// <summary>
        /// İş talebinin öncelik seviyesidir.
        /// </summary>
        [Display(Name = "Öncelik")]
        public WorkPriorityEnum Priority { get; set; }

        /// <summary>
        /// Öncelik seviyesinin Türkçe karşılığıdır.
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
        /// İş talebinin mevcut durumudur.
        /// </summary>
        [Display(Name = "Talep Durumu")]
        public WorkRequestStatusEnum Status { get; set; }

        /// <summary>
        /// İş talebi durumunun Türkçe karşılığıdır.
        /// Yalnızca mevcut enum değerleri kullanılır.
        /// </summary>
        [Display(Name = "Durum Açıklaması")]
        public string StatusName => Status switch
        {
            WorkRequestStatusEnum.Draft => "Yeni",

            WorkRequestStatusEnum.Rejected => "Reddedildi",

            WorkRequestStatusEnum.ConvertedToWorkOrder =>
                "İş Emrine Dönüştürüldü",

            WorkRequestStatusEnum.Cancelled => "İptal Edildi",

            _ => "Bilinmiyor"
        };

        /// <summary>
        /// İş talebinin açıldığı tarihtir.
        /// </summary>
        [Display(Name = "Talep Tarihi")]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// İş talebinin gönderildiği tarihtir.
        /// </summary>
        [Display(Name = "Gönderilme Tarihi")]
        public DateTime? SubmittedAt { get; set; }

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
