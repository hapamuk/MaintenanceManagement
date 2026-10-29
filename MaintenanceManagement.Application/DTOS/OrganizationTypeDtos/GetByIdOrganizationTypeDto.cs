using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.OrganizationTypeDtos
{
    public class GetByIdOrganizationTypeDto
    {
        /// <summary>
        /// Organizasyon türünün ID bilgisi.
        /// </summary>
        [Display(Name = "Organizasyon Türü ID")]
        public int Id { get; set; }

        /// <summary>
        /// Organizasyon türünün adı.
        /// </summary>
        [Display(Name = "Organizasyon Türü Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon türünün kodu.
        /// </summary>
        [Display(Name = "Organizasyon Türü Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon türünün açıklaması.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Organizasyon türünün aktif
        /// veya pasif olma durumu.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Bu organizasyon türüne bağlı
        /// toplam organizasyon birimi sayısı.
        ///
        /// Aktif ve pasif birimler dahildir.
        /// </summary>
        [Display(Name = "Toplam Birim Sayısı")]
        public int UnitCount { get; set; }

        /// <summary>
        /// Bu organizasyon türüne bağlı
        /// aktif organizasyon birimi sayısı.
        /// </summary>
        [Display(Name = "Aktif Birim Sayısı")]
        public int ActiveUnitCount { get; set; }

        /// <summary>
        /// Bu organizasyon türüne bağlı
        /// pasif organizasyon birimi sayısı.
        /// </summary>
        [Display(Name = "Pasif Birim Sayısı")]
        public int InactiveUnitCount { get; set; }

        /// <summary>
        /// Organizasyon türünün
        /// oluşturulma tarihi.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Organizasyon türünün
        /// son güncellenme tarihi.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
