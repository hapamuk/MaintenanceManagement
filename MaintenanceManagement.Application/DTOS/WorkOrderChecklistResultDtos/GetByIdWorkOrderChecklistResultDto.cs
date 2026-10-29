using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderChecklistResultDtos
{
    public class GetByIdWorkOrderChecklistResultDto
    {
        /// <summary>
        /// Kontrol sonucu kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kontrol Sonucu ID")]
        public int Id { get; set; }

        /// <summary>
        /// İş emrinin ID bilgisidir.
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
        /// Bakım kontrol maddesinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kontrol Maddesi ID")]
        public int MaintenanceChecklistId { get; set; }

        /// <summary>
        /// Bakım kontrol maddesinin başlığıdır.
        /// </summary>
        [Display(Name = "Kontrol Başlığı")]
        public string MaintenanceChecklistTitle { get; set; }
            = string.Empty;

        /// <summary>
        /// Bakım kontrol maddesinin açıklamasıdır.
        /// </summary>
        [Display(Name = "Kontrol Açıklaması")]
        public string? MaintenanceChecklistDescription { get; set; }

        /// <summary>
        /// Kontrol maddesinin sıra numarasıdır.
        /// </summary>
        [Display(Name = "Sıra Numarası")]
        public int MaintenanceChecklistSequenceNumber { get; set; }

        /// <summary>
        /// Kontrol maddesinin zorunluluk durumudur.
        /// </summary>
        [Display(Name = "Zorunlu Kontrol")]
        public bool MaintenanceChecklistIsRequired { get; set; }

        /// <summary>
        /// Kontrol maddesinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Kontrol Maddesi Aktif")]
        public bool MaintenanceChecklistIsActive { get; set; }

        /// <summary>
        /// Kontrol maddesinin bağlı olduğu
        /// planlı bakımın ID bilgisidir.
        /// </summary>
        [Display(Name = "Planlı Bakım ID")]
        public int PlannedMaintenanceId { get; set; }

        /// <summary>
        /// İlgili bakım planının adıdır.
        /// </summary>
        [Display(Name = "Bakım Planı Adı")]
        public string PlannedMaintenanceName { get; set; }
            = string.Empty;

        /// <summary>
        /// Kontrolün tamamlanıp tamamlanmadığını
        /// gösterir.
        /// </summary>
        [Display(Name = "Kontrol Tamamlandı")]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Kontrolün uygunluk sonucudur.
        /// </summary>
        [Display(Name = "Kontrol Uygunluk Durumu")]
        public bool? IsSuccessful { get; set; }

        /// <summary>
        /// Kontrol sonucunun Türkçe karşılığıdır.
        /// </summary>
        [Display(Name = "Kontrol Sonucu")]
        public string ResultName
        {
            get
            {
                if (!IsCompleted)
                    return "Kontrol Edilmedi";

                if (!IsSuccessful.HasValue)
                    return "Sonuç Belirtilmedi";

                return IsSuccessful.Value
                    ? "Uygun"
                    : "Uygun Değil";
            }
        }

        /// <summary>
        /// Kontrolü yapan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kontrol Eden Kullanıcı ID")]
        public int? CheckedByUserId { get; set; }

        /// <summary>
        /// Kontrolü yapan kullanıcının
        /// adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Kontrol Eden Kullanıcı")]
        public string? CheckedByUserFullName { get; set; }

        /// <summary>
        /// Kontrolü yapan kullanıcının
        /// e-posta adresidir.
        /// </summary>
        [Display(Name = "Kullanıcı E-Posta")]
        public string? CheckedByUserEmail { get; set; }

        /// <summary>
        /// Kontrolü yapan kullanıcının unvanıdır.
        /// </summary>
        [Display(Name = "Kullanıcı Unvanı")]
        public string? CheckedByUserTitle { get; set; }

        /// <summary>
        /// Kontrolün gerçekleştirildiği tarihtir.
        /// </summary>
        [Display(Name = "Kontrol Tarihi")]
        public DateTime? CheckedAt { get; set; }

        /// <summary>
        /// Kontrole ilişkin açıklama ve notlardır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Notes { get; set; }

        /// <summary>
        /// Kaydın oluşturulduğu tarihtir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kaydın son güncellendiği tarihtir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
