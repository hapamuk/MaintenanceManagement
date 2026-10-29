using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.PlannedMaintenancesDtos
{
    public class GetByIdPlannedMaintenanceDto
    {
        /// <summary>
        /// Bakım planının ID bilgisidir.
        /// </summary>
        [Display(Name = "Bakım Planı ID")]
        public int Id { get; set; }

        /// <summary>
        /// Bakım planının adıdır.
        /// </summary>
        [Display(Name = "Bakım Planı Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bakımı yapılacak demirbaşın ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş ID")]
        public int AssetId { get; set; }

        /// <summary>
        /// Demirbaşın kodudur.
        /// </summary>
        [Display(Name = "Demirbaş Kodu")]
        public string AssetCode { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Adı")]
        public string AssetName { get; set; } = string.Empty;

        /// <summary>
        /// Sorumlu birimin ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Sorumlu birimin adıdır.
        /// </summary>
        [Display(Name = "Sorumlu Birim")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Sorumlu birimin kodudur.
        /// </summary>
        [Display(Name = "Birim Kodu")]
        public string UnitCode { get; set; } = string.Empty;

        /// <summary>
        /// İlgili iş alt türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Alt Türü ID")]
        public int WorkSubTypeId { get; set; }

        /// <summary>
        /// İş alt türünün adıdır.
        /// </summary>
        [Display(Name = "İş Alt Türü")]
        public string WorkSubTypeName { get; set; } = string.Empty;

        /// <summary>
        /// İş alt türünün açıklamasıdır.
        /// </summary>
        [Display(Name = "İş Alt Türü Açıklaması")]
        public string? WorkSubTypeDescription { get; set; }

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
        public string WorkTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Planı oluşturan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı ID")]
        public int? CreatedByUserId { get; set; }

        /// <summary>
        /// Planı oluşturan kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı")]
        public string? CreatedByUserFullName { get; set; }

        /// <summary>
        /// Planı oluşturan kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı E-Posta")]
        public string? CreatedByUserEmail { get; set; }

        /// <summary>
        /// Planı oluşturan kullanıcının unvanıdır.
        /// </summary>
        [Display(Name = "Oluşturan Kullanıcı Unvanı")]
        public string? CreatedByUserTitle { get; set; }

        /// <summary>
        /// Bakımın kaç günde bir tekrarlanacağını belirtir.
        /// </summary>
        [Display(Name = "Bakım Periyodu (Gün)")]
        public int PeriodDays { get; set; }

        /// <summary>
        /// Son gerçekleştirilen bakım tarihidir.
        /// </summary>
        [Display(Name = "Son Bakım Tarihi")]
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// Sonraki planlanan bakım tarihidir.
        /// </summary>
        [Display(Name = "Sonraki Bakım Tarihi")]
        public DateTime NextMaintenanceDate { get; set; }

        /// <summary>
        /// Bakım planının açıklamasıdır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Bakım planının aktiflik durumudur.
        /// </summary>
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Aktiflik durumunun Türkçe karşılığıdır.
        /// </summary>
        [Display(Name = "Durum")]
        public string IsActiveName =>
            IsActive ? "Aktif" : "Pasif";

        /// <summary>
        /// Bu bakım planından oluşturulan
        /// iş emri kayıtlarının sayısıdır.
        /// </summary>
        [Display(Name = "İş Emri Sayısı")]
        public int WorkOrderCount { get; set; }

        /// <summary>
        /// Bakım planına bağlı kontrol listesi sayısıdır.
        /// </summary>
        [Display(Name = "Kontrol Listesi Sayısı")]
        public int ChecklistCount { get; set; }

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
