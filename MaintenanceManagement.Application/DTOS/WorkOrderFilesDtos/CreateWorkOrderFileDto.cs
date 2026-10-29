using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderFilesDtos
{
    public class CreateWorkOrderFileDto
    {
        /// <summary>
        /// Dosyanın bağlı olacağı iş emrinin
        /// ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş emri seçiniz.")]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// Yüklenen dosyanın adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Dosya adı zorunludur.")]
        [StringLength(
            255,
            ErrorMessage = "Dosya adı en fazla 255 karakter olabilir.")]
        [Display(Name = "Dosya Adı")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Dosyanın sunucuda veya dosya depolama
        /// sisteminde bulunduğu kayıt yoludur.
        /// </summary>
        [Required(
            ErrorMessage = "Dosya yolu zorunludur.")]
        [StringLength(
            1000,
            ErrorMessage = "Dosya yolu en fazla 1000 karakter olabilir.")]
        [Display(Name = "Dosya Yolu")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Dosyanın MIME türüdür.
        /// Örnek: application/pdf, image/jpeg.
        /// </summary>
        [StringLength(
            100,
            ErrorMessage = "İçerik türü en fazla 100 karakter olabilir.")]
        [Display(Name = "İçerik Türü")]
        public string? ContentType { get; set; }

        /// <summary>
        /// Dosyanın byte cinsinden boyutudur.
        /// Negatif değer girilemez.
        /// </summary>
        [Range(
            typeof(long),
            "0",
            "9223372036854775807",
            ErrorMessage = "Dosya boyutu negatif olamaz.")]
        [Display(Name = "Dosya Boyutu (Byte)")]
        public long FileSizeBytes { get; set; }
    }
}
