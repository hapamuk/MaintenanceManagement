using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkTypeDtos
{
    public class GetByIdWorkTypeDto
    {
        /// <summary>
        /// İş türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Türü ID")]
        public int Id { get; set; }

        /// <summary>
        /// İş türünün adıdır.
        /// </summary>
        [Display(Name = "İş Türü Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// İş türüne ait açıklama bilgisidir.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// İş türünün aktif veya
        /// pasif olma durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// İş türüne bağlı toplam
        /// iş alt türü sayısıdır.
        ///
        /// Aktif ve pasif alt türler dahildir.
        /// </summary>
        [Display(Name = "Toplam İş Alt Türü Sayısı")]
        public int WorkSubTypeCount { get; set; }

        /// <summary>
        /// İş türüne bağlı aktif
        /// iş alt türü sayısıdır.
        /// </summary>
        [Display(Name = "Aktif İş Alt Türü Sayısı")]
        public int ActiveWorkSubTypeCount { get; set; }

        /// <summary>
        /// İş türüne bağlı pasif
        /// iş alt türü sayısıdır.
        /// </summary>
        [Display(Name = "Pasif İş Alt Türü Sayısı")]
        public int InactiveWorkSubTypeCount { get; set; }

        /// <summary>
        /// İş türünün organizasyon birimleriyle
        /// oluşturulmuş toplam ilişki kaydı sayısıdır.
        ///
        /// Aktif ve pasif ilişkiler dahildir.
        /// </summary>
        [Display(Name = "Toplam Birim İş Türü İlişkisi")]
        public int UnitWorkTypeCount { get; set; }

        /// <summary>
        /// İş türünün aktif organizasyon
        /// birimi ilişki kaydı sayısıdır.
        /// </summary>
        [Display(Name = "Aktif Birim İş Türü İlişkisi")]
        public int ActiveUnitWorkTypeCount { get; set; }

        /// <summary>
        /// İş türünün pasif organizasyon
        /// birimi ilişki kaydı sayısıdır.
        /// </summary>
        [Display(Name = "Pasif Birim İş Türü İlişkisi")]
        public int InactiveUnitWorkTypeCount { get; set; }

        /// <summary>
        /// İş türünün oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// İş türünün son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
