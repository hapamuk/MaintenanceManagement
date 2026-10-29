using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.PermissionDtos
{
    public class ResultPermissionDto
    {
        /// <summary>
        /// Yetkinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Yetki ID")]
        public int Id { get; set; }

        /// <summary>
        /// Yetkinin görünen adıdır.
        /// </summary>
        [Display(Name = "Yetki Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin sistemde kullanılan kodudur.
        /// </summary>
        [Display(Name = "Yetki Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Yetkinin ait olduğu modüldür.
        /// </summary>
        [Display(Name = "Modül")]
        public string Module { get; set; } = string.Empty;

        /// <summary>
        /// Yetkiye ait açıklamadır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Yetkinin aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Yetkinin oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Yetkinin son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
