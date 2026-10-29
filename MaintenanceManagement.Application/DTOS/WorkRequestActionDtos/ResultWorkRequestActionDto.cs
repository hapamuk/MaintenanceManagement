using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkRequestActionDtos
{
    public class ResultWorkRequestActionDto
    {
        /// <summary>
        /// İşlem geçmişi kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "İşlem ID")]
        public int Id { get; set; }

        /// <summary>
        /// İşlemin ait olduğu iş talebinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Talebi ID")]
        public int WorkRequestId { get; set; }

        /// <summary>
        /// İş talebinin numarasıdır.
        /// </summary>
        [Display(Name = "Talep Numarası")]
        public string WorkRequestNumber { get; set; }
            = string.Empty;

        /// <summary>
        /// İş talebinin başlığıdır.
        /// </summary>
        [Display(Name = "Talep Başlığı")]
        public string WorkRequestTitle { get; set; }
            = string.Empty;

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının ID bilgisidir.
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
        /// Gerçekleştirilen işlem türüdür.
        /// </summary>
        [Display(Name = "İşlem Türü")]
        public WorkRequestActionTypeEnum ActionType { get; set; }

        /// <summary>
        /// İşlem türünün Türkçe karşılığıdır.
        /// Mevcut enumdaki dört değer kullanılır.
        /// </summary>
        [Display(Name = "İşlem Türü Açıklaması")]
        public string ActionTypeName => ActionType switch
        {
            WorkRequestActionTypeEnum.Submitted
                => "Talep Gönderildi",

            WorkRequestActionTypeEnum.Converted
                => "İş Emrine Dönüştürüldü",

            WorkRequestActionTypeEnum.Rejected
                => "Talep Reddedildi",

            WorkRequestActionTypeEnum.Cancelled
                => "Talep İptal Edildi",

            _ => "Bilinmiyor"
        };

        /// <summary>
        /// İşlem kaydının açıklamasıdır.
        /// </summary>
        [Display(Name = "İşlem Açıklaması")]
        public string? Description { get; set; }

        /// <summary>
        /// İşlemin gerçekleştirildiği tarihtir.
        /// </summary>
        [Display(Name = "İşlem Tarihi")]
        public DateTime PerformedAt { get; set; }

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
