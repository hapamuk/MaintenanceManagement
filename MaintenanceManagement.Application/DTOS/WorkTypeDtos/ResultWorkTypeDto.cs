using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkTypeDtos
{
    public class ResultWorkTypeDto
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
