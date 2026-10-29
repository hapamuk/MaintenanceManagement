using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.TeamDtos
{
    public class ResultTeamDto
    {
        /// <summary>
        /// Ekibin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip ID")]
        public int Id { get; set; }

        /// <summary>
        /// Ekibin bağlı olduğu organizasyon
        /// biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Organizasyon Birimi ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Ekibin bağlı olduğu organizasyon
        /// biriminin adıdır.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Ekibin adıdır.
        /// </summary>
        [Display(Name = "Ekip Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ekibin kodudur.
        /// </summary>
        [Display(Name = "Ekip Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Ekibin açıklama bilgisidir.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Ekibin aktif/pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Ekibin oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Ekibin son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
