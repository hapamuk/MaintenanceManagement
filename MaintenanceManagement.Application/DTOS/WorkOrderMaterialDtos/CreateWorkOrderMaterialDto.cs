using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderMaterialDtos
{
    public class CreateWorkOrderMaterialDto
    {
        /// <summary>
        /// Malzemenin kullanıldığı iş emrinin ID bilgisidir.
        /// </summary>
        [Required(ErrorMessage = "İş emri zorunludur.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir iş emri seçiniz.")]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// Kullanılan malzemenin ID bilgisidir.
        /// </summary>
        [Required(ErrorMessage = "Malzeme zorunludur.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir malzeme seçiniz.")]
        [Display(Name = "Malzeme")]
        public int MaterialId { get; set; }

        /// <summary>
        /// Malzemenin temin edildiği ambarın ID bilgisidir.
        /// </summary>
        [Required(ErrorMessage = "Ambar zorunludur.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Geçerli bir ambar seçiniz.")]
        [Display(Name = "Ambar")]
        public int WarehouseId { get; set; }

        /// <summary>
        /// İş emrinde kullanılan malzemenin miktarıdır.
        /// Miktar sıfırdan büyük olmalıdır.
        /// </summary>
        [Required(ErrorMessage = "Kullanılan miktar zorunludur.")]
        [Range(
            typeof(decimal),
            "0.001",
            "999999999.999",
            ErrorMessage = "Miktar 0,001 ile 999999999,999 arasında olmalıdır.")]
        [Display(Name = "Kullanılan Miktar")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Malzemenin birim maliyetidir.
        /// Bilinmiyorsa boş bırakılabilir.
        /// </summary>
        [Range(
            typeof(decimal),
            "0",
            "999999999.99",
            ErrorMessage = "Birim maliyet geçerli aralıkta olmalıdır.")]
        [Display(Name = "Birim Maliyet")]
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// Malzemenin kullanımına ilişkin açıklamadır.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
