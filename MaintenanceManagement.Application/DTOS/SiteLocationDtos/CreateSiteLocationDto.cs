using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.SiteLocationDtos
{
    public class CreateSiteLocationDto
    {
        /// <summary>
        /// Oluşturulacak lokasyonun adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Lokasyon adı zorunludur.")]
        [StringLength(
            200,
            ErrorMessage = "Lokasyon adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Lokasyon Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Lokasyonun açık adres bilgisidir.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        /// <summary>
        /// Lokasyonun bulunduğu bina adıdır.
        /// </summary>
        [StringLength(
            100,
            ErrorMessage = "Bina adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Bina")]
        public string? Building { get; set; }

        /// <summary>
        /// Lokasyonun bulunduğu kat bilgisidir.
        /// </summary>
        [StringLength(
            50,
            ErrorMessage = "Kat bilgisi en fazla 50 karakter olabilir.")]
        [Display(Name = "Kat")]
        public string? Floor { get; set; }

        /// <summary>
        /// Lokasyonun coğrafi enlem bilgisidir.
        ///
        /// -90 ile 90 arasında olabilir.
        /// Girilmesi zorunlu değildir.
        /// </summary>
        [Range(
            typeof(decimal),
            "-90",
            "90",
            ErrorMessage = "Enlem -90 ile 90 arasında olmalıdır.")]
        [Display(Name = "Enlem")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Lokasyonun coğrafi boylam bilgisidir.
        ///
        /// -180 ile 180 arasında olabilir.
        /// Girilmesi zorunlu değildir.
        /// </summary>
        [Range(
            typeof(decimal),
            "-180",
            "180",
            ErrorMessage = "Boylam -180 ile 180 arasında olmalıdır.")]
        [Display(Name = "Boylam")]
        public decimal? Longitude { get; set; }
    }
}
