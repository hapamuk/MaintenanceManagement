using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkSubTypeDtos
{
    public class CreateWorkSubTypeDto
    {
        /// <summary>
        /// İş alt türünün bağlı olduğu
        /// iş türünün ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş türü seçiniz.")]
        [Display(Name = "İş Türü")]
        public int WorkTypeId { get; set; }

        /// <summary>
        /// Oluşturulacak iş alt türünün adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "İş alt türü adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "İş alt türü adı en fazla 150 karakter olabilir.")]
        [Display(Name = "İş Alt Türü Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// İş alt türünün açıklama bilgisidir.
        /// Doldurulması zorunlu değildir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
