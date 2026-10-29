using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.SiteLocationDtos
{
    public class ResultSiteLocationDto
    {
        /// <summary>
        /// Lokasyonun ID bilgisidir.
        /// </summary>
        [Display(Name = "Lokasyon ID")]
        public int Id { get; set; }

        /// <summary>
        /// Lokasyonun adıdır.
        /// </summary>
        [Display(Name = "Lokasyon Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Lokasyonun adres bilgisidir.
        /// </summary>
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        /// <summary>
        /// Lokasyonun bulunduğu binadır.
        /// </summary>
        [Display(Name = "Bina")]
        public string? Building { get; set; }

        /// <summary>
        /// Lokasyonun kat bilgisidir.
        /// </summary>
        [Display(Name = "Kat")]
        public string? Floor { get; set; }

        /// <summary>
        /// Lokasyonun enlem bilgisidir.
        /// </summary>
        [Display(Name = "Enlem")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Lokasyonun boylam bilgisidir.
        /// </summary>
        [Display(Name = "Boylam")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Lokasyonun aktif veya
        /// pasif durumudur.
        /// </summary>
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Lokasyon kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Lokasyon kaydının son
        /// güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
