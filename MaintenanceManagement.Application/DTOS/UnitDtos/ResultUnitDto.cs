using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UnitDtos
{
    public class ResultUnitDto
    {
        /// <summary>
        /// Organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Birim ID")]
        public int Id { get; set; }

        /// <summary>
        /// Organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Birim Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin kodudur.
        /// </summary>
        [Display(Name = "Birim Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Bağlı olduğu organizasyon türünün ID değeridir.
        /// </summary>
        [Display(Name = "Organizasyon Türü ID")]
        public int OrganizationTypeId { get; set; }

        /// <summary>
        /// Bağlı olduğu organizasyon türünün adıdır.
        /// </summary>
        [Display(Name = "Organizasyon Türü")]
        public string OrganizationTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Bağlı olduğu üst birimin ID değeridir.
        /// </summary>
        [Display(Name = "Üst Birim ID")]
        public int? ParentUnitId { get; set; }

        /// <summary>
        /// Bağlı olduğu üst birimin adıdır.
        ///
        /// Üst birimi olmayan kayıtlarda
        /// null olabilir.
        /// </summary>
        [Display(Name = "Üst Birim")]
        public string? ParentUnitName { get; set; }

        /// <summary>
        /// Organizasyon biriminin
        /// aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Organizasyon biriminin
        /// oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Organizasyon biriminin
        /// güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
