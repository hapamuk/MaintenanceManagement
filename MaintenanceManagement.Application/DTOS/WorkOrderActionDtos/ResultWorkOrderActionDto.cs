using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderActionDtos
{
    public class ResultWorkOrderActionDto
    {
        /// <summary>
        /// İşlem kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "İşlem ID")]
        public int Id { get; set; }

        /// <summary>
        /// İşlemin bağlı olduğu iş emrinin
        /// ID bilgisidir.
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
        /// İşlemi gerçekleştiren kullanıcının
        /// ID bilgisidir.
        /// </summary>
        [Display(Name = "İşlemi Yapan Kullanıcı ID")]
        public int PerformedByUserId { get; set; }

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının
        /// adı ve soyadıdır.
        /// </summary>
        [Display(Name = "İşlemi Yapan")]
        public string PerformedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// İş emrinde gerçekleştirilen
        /// işlemin açıklamasıdır.
        /// </summary>
        [Display(Name = "Yapılan İşlem")]
        public string Description { get; set; }
            = string.Empty;

        /// <summary>
        /// İşlemin gerçekleştirildiği tarihtir.
        /// </summary>
        [Display(Name = "İşlem Tarihi")]
        public DateTime PerformedAt { get; set; }

        /// <summary>
        /// Yapılan işlem için harcanan
        /// çalışma süresidir (dakika).
        /// </summary>
        [Display(Name = "Çalışma Süresi (Dakika)")]
        public int LaborMinutes { get; set; }

        /// <summary>
        /// İşleme ait ek açıklama ve notlardır.
        /// </summary>
        [Display(Name = "Açıklama / Not")]
        public string? Notes { get; set; }

        /// <summary>
        /// İşlem kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// İşlem kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
