using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaintenanceChecklistDtos
{
    public class ResultMaintenanceChecklistDto
    {
        /// <summary>
        /// Kontrol maddesinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kontrol Maddesi ID")]
        public int Id { get; set; }

        /// <summary>
        /// Kontrol maddesinin bağlı olduğu
        /// bakım planının ID bilgisidir.
        /// </summary>
        [Display(Name = "Planlı Bakım ID")]
        public int PlannedMaintenanceId { get; set; }

        /// <summary>
        /// Kontrol maddesinin bağlı olduğu
        /// bakım planının adıdır.
        /// </summary>
        [Display(Name = "Bakım Planı Adı")]
        public string PlannedMaintenanceName { get; set; }
            = string.Empty;

        /// <summary>
        /// Kontrol maddesinin başlığıdır.
        /// </summary>
        [Display(Name = "Kontrol Başlığı")]
        public string Title { get; set; } = string.Empty;

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
        /// Kontrol maddesinin zorunluluk durumudur.
        /// </summary>
        [Display(Name = "Zorunlu Kontrol")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Zorunluluk durumunun Türkçe açıklamasıdır.
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
        /// Aktiflik durumunun Türkçe açıklamasıdır.
        /// </summary>
        [Display(Name = "Durum")]
        public string IsActiveName =>
            IsActive ? "Aktif" : "Pasif";

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
