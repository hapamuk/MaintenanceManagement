using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkRequestFileDtos
{
    public class ResultWorkRequestFileDto
    {
        /// <summary>
        /// Dosya kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Dosya ID")]
        public int Id { get; set; }

        /// <summary>
        /// Dosyanın bağlı olduğu iş talebinin
        /// ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Talebi ID")]
        public int WorkRequestId { get; set; }

        /// <summary>
        /// Bağlı olduğu iş talebinin numarasıdır.
        /// </summary>
        [Display(Name = "Talep Numarası")]
        public string WorkRequestNumber { get; set; }
            = string.Empty;

        /// <summary>
        /// Bağlı olduğu iş talebinin başlığıdır.
        /// </summary>
        [Display(Name = "Talep Başlığı")]
        public string WorkRequestTitle { get; set; }
            = string.Empty;

        /// <summary>
        /// Dosyanın adıdır.
        /// </summary>
        [Display(Name = "Dosya Adı")]
        public string FileName { get; set; }
            = string.Empty;

        /// <summary>
        /// Dosyanın depolama sistemindeki yoludur.
        /// </summary>
        [Display(Name = "Dosya Yolu")]
        public string FilePath { get; set; }
            = string.Empty;

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
        /// Dosyayı yükleyen kullanıcının
        /// ID bilgisidir.
        /// </summary>
        [Display(Name = "Yükleyen Kullanıcı ID")]
        public int UploadedByUserId { get; set; }

        /// <summary>
        /// Dosyayı yükleyen kullanıcının
        /// adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Yükleyen Kullanıcı")]
        public string UploadedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// Dosya kaydının oluşturulma tarihidir.
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
