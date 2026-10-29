using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.MaintenanceChecklistDtos
{
    public class CreateMaintenanceChecklistDto
    {
        /// <summary>
        /// Kontrol maddesinin bağlı olduğu
        /// planlı bakımın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir bakım planı seçiniz.")]
        [Display(Name = "Planlı Bakım")]
        public int PlannedMaintenanceId { get; set; }

        /// <summary>
        /// Kontrol maddesinin başlığıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Kontrol başlığı zorunludur.")]
        [StringLength(
            200,
            ErrorMessage = "Kontrol başlığı en fazla 200 karakter olabilir.")]
        [Display(Name = "Kontrol Başlığı")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Yapılması gereken kontrol işleminin
        /// ayrıntılı açıklamasıdır.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Kontrol Açıklaması")]
        public string? Description { get; set; }

        /// <summary>
        /// Kontrol maddesinin listedeki sırasıdır.
        /// Varsayılan değer 1'dir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Sıra numarası 1 veya daha büyük olmalıdır.")]
        [Display(Name = "Sıra Numarası")]
        public int SequenceNumber { get; set; } = 1;

        /// <summary>
        /// Kontrol maddesinin tamamlanmasının
        /// zorunlu olup olmadığını belirtir.
        /// </summary>
        [Display(Name = "Zorunlu Kontrol")]
        public bool IsRequired { get; set; } = true;
    }
}
