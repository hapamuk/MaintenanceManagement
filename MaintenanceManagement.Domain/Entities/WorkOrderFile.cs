using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkOrderFile : BaseEntity
    {
        [Required]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Dosya Adı")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        [Display(Name = "Dosya Yolu")]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContentType { get; set; }

        [Range(0, long.MaxValue)]
        public long FileSizeBytes { get; set; }

        [Required]
        [Display(Name = "Yükleyen")]
        public int UploadedByUserId { get; set; }

        public WorkOrder WorkOrder { get; set; } = null!;

        public User UploadedByUser { get; set; } = null!;
    }
}
