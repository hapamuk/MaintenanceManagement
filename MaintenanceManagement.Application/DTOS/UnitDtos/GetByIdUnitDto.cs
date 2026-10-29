using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UnitDtos
{
    public class GetByIdUnitDto
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
        /// Organizasyon türünün ID bilgisidir.
        /// </summary>
        [Display(Name = "Organizasyon Türü ID")]
        public int OrganizationTypeId { get; set; }

        /// <summary>
        /// Organizasyon türünün adıdır.
        /// </summary>
        [Display(Name = "Organizasyon Türü")]
        public string OrganizationTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Bağlı olunan üst birimin ID bilgisidir.
        /// </summary>
        [Display(Name = "Üst Birim ID")]
        public int? ParentUnitId { get; set; }

        /// <summary>
        /// Bağlı olunan üst birimin adıdır.
        /// </summary>
        [Display(Name = "Üst Birim")]
        public string? ParentUnitName { get; set; }

        /// <summary>
        /// Birimin aktif veya pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Birime doğrudan bağlı toplam
        /// alt organizasyon birimi sayısıdır.
        ///
        /// Aktif ve pasif alt birimler dahildir.
        /// </summary>
        [Display(Name = "Alt Birim Sayısı")]
        public int ChildUnitCount { get; set; }

        /// <summary>
        /// Birime doğrudan bağlı aktif
        /// alt organizasyon birimi sayısıdır.
        /// </summary>
        [Display(Name = "Aktif Alt Birim Sayısı")]
        public int ActiveChildUnitCount { get; set; }

        /// <summary>
        /// Birimi birincil organizasyonu
        /// olarak kullanan kullanıcı sayısıdır.
        ///
        /// Aktif ve pasif kullanıcılar dahildir.
        /// </summary>
        [Display(Name = "Kullanıcı Sayısı")]
        public int UserCount { get; set; }

        /// <summary>
        /// Organizasyon birimine bağlı
        /// rol tanımlarının sayısıdır.
        /// </summary>
        [Display(Name = "Rol Sayısı")]
        public int RoleCount { get; set; }

        /// <summary>
        /// Organizasyon birimine bağlı
        /// ekiplerin sayısıdır.
        /// </summary>
        [Display(Name = "Ekip Sayısı")]
        public int TeamCount { get; set; }

        /// <summary>
        /// Organizasyon birimine bağlı
        /// demirbaşların sayısıdır.
        /// </summary>
        [Display(Name = "Demirbaş Sayısı")]
        public int AssetCount { get; set; }

        /// <summary>
        /// Organizasyon biriminin
        /// oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Organizasyon biriminin
        /// son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
