using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MeasurementUnitDtos
{
    public class ResultMeasurementUnitDto
    {
        /// <summary>
        /// Ölçü biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ölçü Birimi ID")]
        public int Id { get; set; }

        /// <summary>
        /// Ölçü biriminin adıdır.
        /// </summary>
        [Display(Name = "Ölçü Birimi")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin kısaltmasıdır.
        /// </summary>
        [Display(Name = "Kısaltma")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Ölçü biriminin aktiflik durumudur.
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
        /// Bu ölçü birimini kullanan
        /// toplam malzeme kayıt sayısıdır.
        /// </summary>
        [Display(Name = "Malzeme Sayısı")]
        public int MaterialCount { get; set; }

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
