using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UnitWorkTypeDtos
{
    public class UpdateUnitWorkTypeDto
    {
        /// <summary>
        /// Güncellenecek birim-iş türü
        /// ilişkisinin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir ilişki kaydı seçiniz.")]
        [Display(Name = "İlişki ID")]
        public int Id { get; set; }

        /// <summary>
        /// İş türünün bağlı olduğu
        /// organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir organizasyon birimi seçiniz.")]
        [Display(Name = "Organizasyon Birimi")]
        public int UnitId { get; set; }

        /// <summary>
        /// Organizasyon birimine atanacak
        /// iş türünün ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir iş türü seçiniz.")]
        [Display(Name = "İş Türü")]
        public int WorkTypeId { get; set; }
    }
}
