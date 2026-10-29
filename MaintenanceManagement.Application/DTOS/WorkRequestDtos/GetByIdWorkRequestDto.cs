using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkRequestDtos
{
    public class GetByIdWorkRequestDto
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
        /// İş talebiyle ilişkili demirbaşın ID bilgisidir.
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
        /// Talebi oluşturan kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "Talep Eden E-Posta")]
        public string? RequestedByUserEmail { get; set; }

        /// <summary>
        /// Talebi oluşturan kullanıcının
        /// aktiflik durumudur.
        /// </summary>
        [Display(Name = "Talep Eden Kullanıcı Durumu")]
        public bool RequestedByUserIsActive { get; set; }

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
        /// Talep eden organizasyon biriminin kodudur.
        /// </summary>
        [Display(Name = "Talep Eden Birim Kodu")]
        public string RequesterUnitCode { get; set; }
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
        /// Sorumlu organizasyon biriminin kodudur.
        /// </summary>
        [Display(Name = "Sorumlu Birim Kodu")]
        public string ResponsibleUnitCode { get; set; }
            = string.Empty;

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
        /// İş alt türünün aktiflik durumudur.
        /// </summary>
        [Display(Name = "İş Alt Türü Durumu")]
        public bool WorkSubTypeIsActive { get; set; }

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
        /// Sorumlu yöneticinin e-posta adresidir.
        /// </summary>
        [Display(Name = "Sorumlu Yönetici E-Posta")]
        public string? ManagerUserEmail { get; set; }

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
        /// Lokasyonun adresidir.
        /// </summary>
        [Display(Name = "Lokasyon Adresi")]
        public string? SiteLocationAddress { get; set; }

        /// <summary>
        /// Konuma ilişkin açıklama bilgisidir.
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
        /// İş talebinin mevcut durumunun
        /// Türkçe karşılığıdır.
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
        /// İş talebinin oluşturulduğu tarihtir.
        /// </summary>
        [Display(Name = "Talep Tarihi")]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// İş talebinin gönderilme tarihidir.
        /// </summary>
        [Display(Name = "Gönderilme Tarihi")]
        public DateTime? SubmittedAt { get; set; }

        /// <summary>
        /// Talebe bağlı iş emrinin ID bilgisidir.
        /// İş emri yoksa null olabilir.
        /// </summary>
        [Display(Name = "İş Emri ID")]
        public int? WorkOrderId { get; set; }

        /// <summary>
        /// Talebe bağlı iş emri bulunup
        /// bulunmadığını gösterir.
        /// </summary>
        [Display(Name = "İş Emri Oluşturulmuş")]
        public bool HasWorkOrder => WorkOrderId.HasValue;

        /// <summary>
        /// İş talebine eklenen dosya sayısıdır.
        /// </summary>
        [Display(Name = "Dosya Sayısı")]
        public int FileCount { get; set; }

        /// <summary>
        /// İş talebiyle ilgili işlem
        /// geçmişi kayıtlarının sayısıdır.
        /// </summary>
        [Display(Name = "İşlem Geçmişi Sayısı")]
        public int ActionCount { get; set; }

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

