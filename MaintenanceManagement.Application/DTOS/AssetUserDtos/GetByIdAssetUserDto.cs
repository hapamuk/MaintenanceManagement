using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetUserDtos
{
    public class GetByIdAssetUserDto
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
        /// Demirbaşın sistem kodudur.
        /// </summary>
        [Display(Name = "Demirbaş Kodu")]
        public string AssetCode { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Adı")]
        public string AssetName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın seri numarasıdır.
        /// </summary>
        [Display(Name = "Seri Numarası")]
        public string? AssetSerialNumber { get; set; }

        /// <summary>
        /// Demirbaşın envanter numarasıdır.
        /// </summary>
        [Display(Name = "Envanter Numarası")]
        public string? AssetInventoryNumber { get; set; }

        /// <summary>
        /// Demirbaşın mevcut durumudur.
        /// </summary>
        [Display(Name = "Demirbaş Durumu")]
        public AssetStatusEnum AssetStatus { get; set; }

        /// <summary>
        /// Demirbaşın model adıdır.
        /// </summary>
        [Display(Name = "Model")]
        public string AssetModelName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaş modelinin bağlı olduğu markadır.
        /// </summary>
        [Display(Name = "Marka")]
        public string BrandName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın bağlı olduğu organizasyon
        /// biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Demirbaş Birim ID")]
        public int AssetUnitId { get; set; }

        /// <summary>
        /// Demirbaşın bağlı olduğu
        /// organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Demirbaş Birimi")]
        public string AssetUnitName { get; set; } = string.Empty;

        /// <summary>
        /// Demirbaşın zimmetlendiği
        /// kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Zimmetli kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Zimmetli Kullanıcı")]
        public string UserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Zimmetli kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "E-Posta")]
        public string? UserEmail { get; set; }

        /// <summary>
        /// Zimmetli kullanıcının unvanıdır.
        /// </summary>
        [Display(Name = "Unvan")]
        public string? UserTitle { get; set; }

        /// <summary>
        /// Zimmetli kullanıcının birincil
        /// organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kullanıcı Birim ID")]
        public int UserPrimaryUnitId { get; set; }

        /// <summary>
        /// Zimmetli kullanıcının birincil
        /// organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Kullanıcının Birimi")]
        public string UserPrimaryUnitName { get; set; } = string.Empty;

        /// <summary>
        /// Zimmetli kullanıcının aktiflik durumudur.
        /// </summary>
        [Display(Name = "Kullanıcı Durumu")]
        public bool UserIsActive { get; set; }

        /// <summary>
        /// Zimmet işlemini gerçekleştiren
        /// kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Zimmet Veren ID")]
        public int? AssignedByUserId { get; set; }

        /// <summary>
        /// Zimmet işlemini gerçekleştiren
        /// kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Zimmet Veren")]
        public string? AssignedByFullName { get; set; }

        /// <summary>
        /// Zimmet işlemini gerçekleştiren
        /// kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "Zimmet Veren E-Posta")]
        public string? AssignedByUserEmail { get; set; }

        /// <summary>
        /// Demirbaşın zimmet başlangıç tarihidir.
        /// </summary>
        [Display(Name = "Zimmet Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Demirbaşın zimmet bitiş veya
        /// iade tarihidir.
        ///
        /// Açık zimmetlerde null olur.
        /// </summary>
        [Display(Name = "Zimmet Bitiş Tarihi")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Zimmet kaydının açık olup olmadığını
        /// EndDate üzerinden hesaplar.
        /// </summary>
        [Display(Name = "Açık Zimmet")]
        public bool IsOpen => !EndDate.HasValue;

        /// <summary>
        /// Zimmet işlemiyle ilgili açıklamadır.
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
