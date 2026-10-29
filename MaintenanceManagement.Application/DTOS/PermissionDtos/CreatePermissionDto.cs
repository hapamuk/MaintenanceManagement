using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.PermissionDtos
{
    public class CreatePermissionDto
    {
        /// <summary>
        /// Yetkinin görünen adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Yetki adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Yetki adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Yetki Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Yetkiyi sistem içerisinde
        /// tanımlayan benzersiz koddur.
        /// </summary>
        [Required(
            ErrorMessage = "Yetki kodu zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Yetki kodu en fazla 150 karakter olabilir.")]
        [Display(Name = "Yetki Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin ait olduğu modül bilgisidir.
        /// Örneğin Asset, WorkOrder veya Stock.
        /// </summary>
        [StringLength(
            100,
            ErrorMessage = "Modül adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Modül")]
        public string Module { get; set; } = string.Empty;

        /// <summary>
        /// Yetki hakkında açıklama bilgisidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
