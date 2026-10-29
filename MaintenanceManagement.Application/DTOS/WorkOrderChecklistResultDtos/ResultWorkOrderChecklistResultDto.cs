using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderChecklistResultDtos
{
    public class ResultWorkOrderChecklistResultDto
    {
        /// <summary>
        /// Kontrol sonucu kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kontrol Sonucu ID")]
        public int Id { get; set; }

        /// <summary>
        /// Kontrolün ait olduğu iş emrinin ID bilgisidir.
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
        /// Bakım kontrol maddesinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kontrol Maddesi ID")]
        public int MaintenanceChecklistId { get; set; }

        /// <summary>
        /// Kontrol maddesinin başlığıdır.
        /// </summary>
        [Display(Name = "Kontrol Başlığı")]
        public string MaintenanceChecklistTitle { get; set; }
            = string.Empty;

        /// <summary>
        /// Kontrol maddesinin listedeki sıra numarasıdır.
        /// </summary>
        [Display(Name = "Sıra Numarası")]
        public int MaintenanceChecklistSequenceNumber { get; set; }

        /// <summary>
        /// Kontrol maddesinin zorunlu olup
        /// olmadığını belirtir.
        /// </summary>
        [Display(Name = "Zorunlu Kontrol")]
        public bool MaintenanceChecklistIsRequired { get; set; }

        /// <summary>
        /// Kontrolün tamamlanıp tamamlanmadığını
        /// belirtir.
        /// </summary>
        [Display(Name = "Kontrol Tamamlandı")]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Kontrolün uygunluk sonucudur.
        /// </summary>
        [Display(Name = "Kontrol Uygunluk Durumu")]
        public bool? IsSuccessful { get; set; }

        /// <summary>
        /// Kontrol sonucunun Türkçe açıklamasıdır.
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
        /// Kontrolü yapan kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Kontrol Eden Kullanıcı")]
        public string? CheckedByUserFullName { get; set; }

        /// <summary>
        /// Kontrolün yapıldığı tarihtir.
        /// </summary>
        [Display(Name = "Kontrol Tarihi")]
        public DateTime? CheckedAt { get; set; }

        /// <summary>
        /// Kontrole ilişkin açıklama ve notlardır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Notes { get; set; }

        /// <summary>
        /// Kontrol kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kontrol kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
