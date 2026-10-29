using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.PlannedMaintenancesDtos
{
    public class CreatePlannedMaintenanceDto
    {
        /// <summary>
        /// Planlı bakımın adıdır.
        /// </summary>
        [Required(ErrorMessage = "Bakım planı adı zorunludur.")]
        [StringLength(
            200,
            ErrorMessage = "Bakım planı adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Bakım Planı Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bakımı yapılacak demirbaşın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir demirbaş seçiniz.")]
        [Display(Name = "Demirbaş")]
        public int AssetId { get; set; }

        /// <summary>
        /// Bakımdan sorumlu birimin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir birim seçiniz.")]
        [Display(Name = "Sorumlu Birim")]
        public int UnitId { get; set; }

        /// <summary>
        /// Planlı bakımın ilişkili olduğu
        /// iş alt türünün ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş alt türü seçiniz.")]
        [Display(Name = "İş Alt Türü")]
        public int WorkSubTypeId { get; set; }

        /// <summary>
        /// Bakımın kaç günde bir tekrarlanacağını belirtir.
        /// </summary>
        [Range(
            1,
            3650,
            ErrorMessage = "Bakım periyodu 1 ile 3650 gün arasında olmalıdır.")]
        [Display(Name = "Bakım Periyodu (Gün)")]
        public int PeriodDays { get; set; }

        /// <summary>
        /// Demirbaşa en son bakım yapılan tarihtir.
        /// İlk bakım planı oluşturulurken boş olabilir.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Son Bakım Tarihi")]
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// Bir sonraki bakımın planlandığı tarihtir.
        /// </summary>
        [Required(ErrorMessage = "Sonraki bakım tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Sonraki Bakım Tarihi")]
        public DateTime NextMaintenanceDate { get; set; }

        /// <summary>
        /// Planlı bakımla ilgili açıklamadır.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
