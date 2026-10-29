using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.UnitWorkTypeDtos
{
    public class CreateUnitWorkTypeDto
    {
        /// <summary>
        /// İş türünün atanacağı organizasyon
        /// biriminin ID bilgisidir.
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
