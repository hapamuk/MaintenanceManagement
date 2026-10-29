using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UnitWorkTypeDtos
{
    public class ResultUnitWorkTypeDto
    {
        /// <summary>
        /// Birim-iş türü ilişkisinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İlişki ID")]
        public int Id { get; set; }

        /// <summary>
        /// Organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin kodudur.
        /// </summary>
        [Display(Name = "Birim Kodu")]
        public string UnitCode { get; set; } = string.Empty;

        /// <summary>
        /// İş türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Türü ID")]
        public int WorkTypeId { get; set; }

        /// <summary>
        /// İş türünün adıdır.
        /// </summary>
        [Display(Name = "İş Türü")]
        public string WorkTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Birim Durumu")]
        public bool UnitIsActive { get; set; }

        /// <summary>
        /// İş türünün aktiflik durumudur.
        /// </summary>
        [Display(Name = "İş Türü Durumu")]
        public bool WorkTypeIsActive { get; set; }

        /// <summary>
        /// Birim-iş türü ilişkisinin
        /// aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "İlişki Durumu")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Birim, iş türü ve ilişki kaydının
        /// birlikte aktif olup olmadığını belirtir.
        /// </summary>
        [Display(Name = "Etkin İlişki Durumu")]
        public bool IsEffectivelyActive =>
            IsActive &&
            UnitIsActive &&
            WorkTypeIsActive;

        /// <summary>
        /// İlişki kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// İlişki kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
