using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.TeamDtos
{
    public class GetByIdTeamDto
    {
        /// <summary>
        /// Ekibin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip ID")]
        public int Id { get; set; }

        /// <summary>
        /// Bağlı olduğu organizasyon
        /// biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Organizasyon Birimi ID")]
        public int UnitId { get; set; }

        /// <summary>
        /// Bağlı olduğu organizasyon
        /// biriminin adıdır.
        /// </summary>
        [Display(Name = "Organizasyon Birimi")]
        public string UnitName { get; set; } = string.Empty;

        /// <summary>
        /// Organizasyon biriminin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Organizasyon Birimi Durumu")]
        public bool UnitIsActive { get; set; }

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
        /// Ekibin görev ve sorumluluk
        /// açıklamasıdır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Ekibin aktif/pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Ekibe ait toplam üyelik kaydı sayısıdır.
        ///
        /// Aktif ve pasif üyelikler dahildir.
        /// </summary>
        [Display(Name = "Toplam Üyelik Sayısı")]
        public int TotalMemberCount { get; set; }

        /// <summary>
        /// Ekibin aktif üyelik sayısıdır.
        ///
        /// IsActive true olan ve ayrılış
        /// tarihi bulunmayan üyelikler sayılır.
        /// </summary>
        [Display(Name = "Aktif Üye Sayısı")]
        public int ActiveMemberCount { get; set; }

        /// <summary>
        /// Ekibe atanmış toplam iş
        /// emri sayısıdır.
        ///
        /// Bütün iş emri durumları dahildir.
        /// </summary>
        [Display(Name = "Toplam İş Emri Sayısı")]
        public int WorkOrderCount { get; set; }

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
