using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetUserDtos
{
    public class ResultAssetUserDto
    {
        /// <summary>
        /// Zimmet kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Zimmet ID")]
        public int Id { get; set; }

        /// <summary>
        /// Zimmetlenen demirbaşın ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş ID")]
        public int AssetId { get; set; }

        /// <summary>
        /// Zimmetlenen demirbaşın sistem kodudur.
        /// </summary>
        [Display(Name = "Demirbaş Kodu")]
        public string AssetCode { get; set; } = string.Empty;

        /// <summary>
        /// Zimmetlenen demirbaşın adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Adı")]
        public string AssetName { get; set; } = string.Empty;

        /// <summary>
        /// Zimmetli kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Demirbaşın zimmetlendiği
        /// kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Zimmetli Kullanıcı")]
        public string UserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Zimmet işlemini yapan
        /// kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Zimmet Veren ID")]
        public int? AssignedByUserId { get; set; }

        /// <summary>
        /// Zimmet işlemini yapan
        /// kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Zimmet Veren")]
        public string? AssignedByFullName { get; set; }

        /// <summary>
        /// Demirbaşın kullanıcıya
        /// zimmetlendiği başlangıç tarihidir.
        /// </summary>
        [Display(Name = "Zimmet Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Demirbaşın zimmetinin sona erdiği
        /// veya iade edildiği tarihtir.
        ///
        /// Açık zimmetlerde null olabilir.
        /// </summary>
        [Display(Name = "Zimmet Bitiş Tarihi")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Zimmet kaydının açık olup olmadığını
        /// belirtir.
        ///
        /// Entity üzerinde karşılığı bulunmayan,
        /// EndDate üzerinden hesaplanan alandır.
        /// </summary>
        [Display(Name = "Açık Zimmet")]
        public bool IsOpen => !EndDate.HasValue;

        /// <summary>
        /// Zimmet işlemine ait açıklama bilgisidir.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Kaydın oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kaydın son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
