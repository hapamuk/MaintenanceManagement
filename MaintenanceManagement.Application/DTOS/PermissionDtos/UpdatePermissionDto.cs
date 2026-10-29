using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.PermissionDtos
{
    public class UpdatePermissionDto
    {
        
        public int Id { get; set; }

        /// <summary>
        /// Yetkinin adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Yetki adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Yetki adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Yetki Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin benzersiz kodudur.
        /// </summary>
        [Required(
            ErrorMessage = "Yetki kodu zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Yetki kodu en fazla 150 karakter olabilir.")]
        [Display(Name = "Yetki Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin bağlı olduğu modüldür.
        /// </summary>
        [StringLength(
            100,
            ErrorMessage = "Modül adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Modül")]
        public string Module { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin açıklama bilgisidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
