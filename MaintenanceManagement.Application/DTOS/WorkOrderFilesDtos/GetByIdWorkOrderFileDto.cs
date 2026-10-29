using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderFilesDtos
{
    public class GetByIdWorkOrderFileDto
    {
        /// <summary>
        /// Dosya kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Dosya ID")]
        public int Id { get; set; }

        /// <summary>
        /// Dosyanın bağlı olduğu iş emrinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Emri ID")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// İş emrinin benzersiz numarasıdır.
        /// </summary>
        [Display(Name = "İş Emri Numarası")]
        public string WorkOrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin başlığıdır.
        /// </summary>
        [Display(Name = "İş Emri Başlığı")]
        public string WorkOrderTitle { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin açıklamasıdır.
        /// </summary>
        [Display(Name = "İş Emri Açıklaması")]
        public string? WorkOrderDescription { get; set; }

        /// <summary>
        /// Dosyanın adıdır.
        /// </summary>
        [Display(Name = "Dosya Adı")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Dosyanın depolama sistemindeki kayıt yoludur.
        /// </summary>
        [Display(Name = "Dosya Yolu")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Dosyanın MIME türüdür.
        /// </summary>
        [Display(Name = "İçerik Türü")]
        public string? ContentType { get; set; }

        /// <summary>
        /// Dosyanın byte cinsinden boyutudur.
        /// </summary>
        [Display(Name = "Dosya Boyutu (Byte)")]
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Dosyayı yükleyen kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Yükleyen Kullanıcı ID")]
        public int UploadedByUserId { get; set; }

        /// <summary>
        /// Dosyayı yükleyen kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Yükleyen Kullanıcı")]
        public string UploadedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// Dosyayı yükleyen kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "Yükleyen Kullanıcı E-Posta")]
        public string? UploadedByUserEmail { get; set; }

        /// <summary>
        /// Dosyayı yükleyen kullanıcının aktiflik durumudur.
        /// </summary>
        [Display(Name = "Yükleyen Kullanıcı Durumu")]
        public bool UploadedByUserIsActive { get; set; }

        /// <summary>
        /// Dosyanın veritabanına kaydedildiği tarihtir.
        /// </summary>
        [Display(Name = "Yüklenme Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Dosya kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
