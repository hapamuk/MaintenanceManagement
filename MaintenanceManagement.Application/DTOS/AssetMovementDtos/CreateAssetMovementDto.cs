using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetMovementDtos
{
    public class CreateAssetMovementDto
    {
        /// <summary>
        /// Hareket işlemi gerçekleştirilecek
        /// demirbaşın ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir demirbaş seçiniz.")]
        [Display(Name = "Demirbaş")]
        public int AssetId { get; set; }

        /// <summary>
        /// Demirbaşın hareket öncesindeki
        /// organizasyon biriminin ID bilgisidir.
        ///
        /// İlk kayıt gibi işlemlerde
        /// null olabilir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir kaynak birim seçiniz.")]
        [Display(Name = "Kaynak Birim")]
        public int? FromUnitId { get; set; }

        /// <summary>
        /// Demirbaşın hareket sonrasında
        /// bağlanacağı organizasyon
        /// biriminin ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir hedef birim seçiniz.")]
        [Display(Name = "Hedef Birim")]
        public int? ToUnitId { get; set; }

        /// <summary>
        /// Hareket öncesinde demirbaşın
        /// zimmetli olduğu kullanıcının
        /// ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir kaynak kullanıcı seçiniz.")]
        [Display(Name = "Kaynak Kullanıcı")]
        public int? FromUserId { get; set; }

        /// <summary>
        /// Hareket sonrasında demirbaşın
        /// zimmetleneceği kullanıcının
        /// ID bilgisidir.
        /// </summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir hedef kullanıcı seçiniz.")]
        [Display(Name = "Hedef Kullanıcı")]
        public int? ToUserId { get; set; }

        /// <summary>
        /// Gerçekleştirilecek hareketin türüdür.
        ///
        /// Registration, Assignment, Transfer,
        /// Return veya Scrap olabilir.
        /// </summary>
        [EnumDataType(
            typeof(AssetMovementTypeEnum),
            ErrorMessage = "Geçerli bir hareket türü seçiniz.")]
        [Display(Name = "Hareket Türü")]
        public AssetMovementTypeEnum MovementType { get; set; }

        /// <summary>
        /// Hareketle ilgili açıklama bilgisidir.
        /// </summary>
        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
