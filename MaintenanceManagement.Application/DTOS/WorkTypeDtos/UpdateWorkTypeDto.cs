using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkTypeDtos
{
    public class UpdateWorkTypeDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// İş türünün güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "İş türü adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "İş türü adı en fazla 150 karakter olabilir.")]
        [Display(Name = "İş Türü Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// İş türünün güncel açıklamasıdır.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
