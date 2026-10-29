using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkRequestDtos
{
    public class UpdateWorkRequestDto
    {
       
        public int Id { get; set; }

        /// <summary>
        /// İş talebinin güncel başlığıdır.
        /// </summary>
        [Required(ErrorMessage = "Talep başlığı zorunludur.")]
        [StringLength(200,
            ErrorMessage = "Talep başlığı en fazla 200 karakter olabilir.")]
        [Display(Name = "Talep Başlığı")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// İş talebinin güncel açıklamasıdır.
        /// </summary>
        [Required(ErrorMessage = "Talep açıklaması zorunludur.")]
        [StringLength(2000,
            ErrorMessage = "Talep açıklaması en fazla 2000 karakter olabilir.")]
        [Display(Name = "Talep Açıklaması")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// İş talebinin ilişkili olduğu
        /// demirbaşın ID bilgisidir.
        /// </summary>
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir demirbaş seçiniz.")]
        [Display(Name = "Demirbaş")]
        public int? AssetId { get; set; }

        /// <summary>
        /// Talep eden organizasyon
        /// biriminin ID bilgisidir.
        /// </summary>
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir talep eden birim seçiniz.")]
        [Display(Name = "Talep Eden Birim")]
        public int RequesterUnitId { get; set; }

        /// <summary>
        /// Sorumlu organizasyon biriminin
        /// ID bilgisidir.
        /// </summary>
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir sorumlu birim seçiniz.")]
        [Display(Name = "Sorumlu Birim")]
        public int ResponsibleUnitId { get; set; }

        /// <summary>
        /// İş talebinin alt türünün ID bilgisidir.
        /// </summary>
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir iş alt türü seçiniz.")]
        [Display(Name = "İş Alt Türü")]
        public int WorkSubTypeId { get; set; }

        /// <summary>
        /// Saha veya lokasyonun ID bilgisidir.
        /// Boş bırakılabilir.
        /// </summary>
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir lokasyon seçiniz.")]
        [Display(Name = "Lokasyon")]
        public int? SiteLocationId { get; set; }

        /// <summary>
        /// İşin gerçekleştirileceği konuma ait
        /// ek açıklama bilgisidir.
        /// </summary>
        [StringLength(500,
            ErrorMessage = "Konum açıklaması en fazla 500 karakter olabilir.")]
        [Display(Name = "Konum Açıklaması")]
        public string? LocationDescription { get; set; }

        /// <summary>
        /// İş talebinin öncelik seviyesidir.
        /// </summary>
        [EnumDataType(typeof(WorkPriorityEnum),
            ErrorMessage = "Geçerli bir öncelik seçiniz.")]
        [Display(Name = "Öncelik")]
        public WorkPriorityEnum Priority { get; set; }
            = WorkPriorityEnum.Normal;
    }
}

