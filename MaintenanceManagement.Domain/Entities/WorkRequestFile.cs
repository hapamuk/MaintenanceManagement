using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkRequestFile : BaseEntity
    {
        [Required]
        [Display(Name = "İş Talebi")]
        public int WorkRequestId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Dosya Adı")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        [Display(Name = "Dosya Yolu")]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Dosya Türü")]
        public string? ContentType { get; set; }

        [Range(0, long.MaxValue)]
        [Display(Name = "Dosya Boyutu")]
        public long FileSizeBytes { get; set; }

        [Required]
        [Display(Name = "Yükleyen Kullanıcı")]
        public int UploadedByUserId { get; set; }

        public WorkRequest WorkRequest { get; set; } = null!;

        public User UploadedByUser { get; set; } = null!;
    }
}
