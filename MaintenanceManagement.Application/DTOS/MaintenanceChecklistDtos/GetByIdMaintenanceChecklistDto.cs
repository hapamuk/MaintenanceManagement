using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaintenanceChecklistDtos
{
    public class GetByIdMaintenanceChecklistDto
    {
        /// <summary>
        /// Kontrol maddesinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kontrol Maddesi ID")]
        public int Id { get; set; }

        /// <summary>
        /// Bağlı olduğu bakım planının ID bilgisidir.
        /// </summary>
        [Display(Name = "Planlı Bakım ID")]
        public int PlannedMaintenanceId { get; set; }

        /// <summary>
        /// Bağlı olduğu bakım planının adıdır.
        /// </summary>
        [Display(Name = "Bakım Planı Adı")]
        public string PlannedMaintenanceName { get; set; }
            = string.Empty;

        /// <summary>
        /// Bakım planının açıklamasıdır.
        /// </summary>
        [Display(Name = "Bakım Planı Açıklaması")]
        public string? PlannedMaintenanceDescription { get; set; }

        /// <summary>
        /// Bakım yapılacak demirbaşın ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş ID")]
        public int AssetId { get; set; }

        /// <summary>
        /// Demirbaşın kodudur.
        /// </summary>
        [Display(Name = "Demirbaş Kodu")]
        public string AssetCode { get; set; }
            = string.Empty;

        /// <summary>
        /// Demirbaşın adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Adı")]
        public string AssetName { get; set; }
            = string.Empty;

        /// <summary>
        /// Bakım planından sorumlu birimin ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Sorumlu birimin adıdır.
        /// </summary>
        [Display(Name = "Sorumlu Birim")]
        public string UnitName { get; set; }
            = string.Empty;

        /// <summary>
        /// Bakım planına ait iş alt türünün ID bilgisidir.
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
        /// Kontrol maddesinin başlığıdır.
        /// </summary>
        [Display(Name = "Kontrol Başlığı")]
        public string Title { get; set; }
            = string.Empty;

        /// <summary>
        /// Kontrol maddesinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Kontrol Açıklaması")]
        public string? Description { get; set; }

        /// <summary>
        /// Kontrol maddesinin listedeki sırasıdır.
        /// </summary>
        [Display(Name = "Sıra Numarası")]
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Kontrol maddesinin zorunlu olup
        /// olmadığını belirtir.
        /// </summary>
        [Display(Name = "Zorunlu Kontrol")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Kontrol maddesinin zorunluluk açıklamasıdır.
        /// </summary>
        [Display(Name = "Zorunluluk Açıklaması")]
        public string IsRequiredName =>
            IsRequired ? "Zorunlu" : "İsteğe Bağlı";

        /// <summary>
        /// Kontrol maddesinin aktiflik durumudur.
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
        /// Bu kontrol maddesine bağlı
        /// iş emri kontrol sonucu kayıt sayısıdır.
        /// </summary>
        [Display(Name = "Kontrol Sonucu Sayısı")]
        public int ResultCount { get; set; }

        /// <summary>
        /// Bakım planının tekrar periyodudur.
        /// Gün cinsinden gösterilir.
        /// </summary>
        [Display(Name = "Bakım Periyodu (Gün)")]
        public int PeriodDays { get; set; }

        /// <summary>
        /// Bakım planının sonraki bakım tarihidir.
        /// </summary>
        [Display(Name = "Sonraki Bakım Tarihi")]
        public DateTime NextMaintenanceDate { get; set; }

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
