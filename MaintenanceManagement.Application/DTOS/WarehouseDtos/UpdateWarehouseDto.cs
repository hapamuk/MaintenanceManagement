using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WarehouseDtos
{
    public class UpdateWarehouseDto
    {
        /// <summary>
        /// Güncellenecek ambarın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir ambar seçiniz.")]
        [Display(Name = "Ambar ID")]
        public int Id { get; set; }

        /// <summary>
        /// Ambarın güncel kodudur.
        /// </summary>
        [Required(
            ErrorMessage = "Ambar kodu zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Ambar kodu en fazla 100 karakter olabilir.")]
        [Display(Name = "Ambar Kodu")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Ambarın güncel adıdır.
        /// </summary>
        [Required(
            ErrorMessage = "Ambar adı zorunludur.")]
        [StringLength(
            150,
            ErrorMessage = "Ambar adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Ambar Adı")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ambarın bağlı olduğu organizasyon
        /// biriminin ID bilgisidir.
        ///
        /// İsteğe bağlıdır.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir birim seçiniz.")]
        [Display(Name = "Bağlı Birim")]
        public int? UnitId { get; set; }

        /// <summary>
        /// Ambarın güncel konum bilgisidir.
        /// </summary>
        [StringLength(
            250,
            ErrorMessage = "Ambar konumu en fazla 250 karakter olabilir.")]
        [Display(Name = "Ambar Konumu")]
        public string? Location { get; set; }

        /// <summary>
        /// Ambarın güncel açıklamasıdır.
        /// </summary>
        [StringLength(
            500,
            ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Ambara görevlendirilecek personellerin ID listesi.
        /// </summary>
        [Display(Name = "Görevli Personeller")]
        public List<int> SelectedPersonnelIds { get; set; } = new();
    }
}
