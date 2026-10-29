using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.AssetMovementDtos
{
    public class ResultAssetMovementDto
    {
        /// <summary>
        /// Hareket kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Hareket ID")]
        public int Id { get; set; }

        /// <summary>
        /// Hareket yapılan demirbaşın ID bilgisidir.
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
        /// Kaynak organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Kaynak Birim ID")]
        public int? FromUnitId { get; set; }

        /// <summary>
        /// Kaynak organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Kaynak Birim")]
        public string? FromUnitName { get; set; }

        /// <summary>
        /// Hedef organizasyon biriminin ID bilgisidir.
        /// </summary>
        [Display(Name = "Hedef Birim ID")]
        public int? ToUnitId { get; set; }

        /// <summary>
        /// Hedef organizasyon biriminin adıdır.
        /// </summary>
        [Display(Name = "Hedef Birim")]
        public string? ToUnitName { get; set; }

        /// <summary>
        /// Kaynak kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Kaynak Kullanıcı ID")]
        public int? FromUserId { get; set; }

        /// <summary>
        /// Kaynak kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Kaynak Kullanıcı")]
        public string? FromUserFullName { get; set; }

        /// <summary>
        /// Hedef kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Hedef Kullanıcı ID")]
        public int? ToUserId { get; set; }

        /// <summary>
        /// Hedef kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Hedef Kullanıcı")]
        public string? ToUserFullName { get; set; }

        /// <summary>
        /// Hareketi gerçekleştiren kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "İşlemi Yapan Kullanıcı ID")]
        public int PerformedByUserId { get; set; }

        /// <summary>
        /// Hareketi gerçekleştiren kullanıcının
        /// adı ve soyadıdır.
        /// </summary>
        [Display(Name = "İşlemi Yapan")]
        public string PerformedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// Hareketin enum türüdür.
        /// </summary>
        [Display(Name = "Hareket Türü")]
        public AssetMovementTypeEnum MovementType { get; set; }

        /// <summary>
        /// Hareket türünün Türkçe karşılığıdır.
        ///
        /// Entity sınıfına eklenmez.
        /// DTO içerisinde hesaplanır.
        /// </summary>
        [Display(Name = "Hareket Türü Açıklaması")]
        public string MovementTypeName => MovementType switch
        {
            AssetMovementTypeEnum.Registration => "İlk Kayıt",
            AssetMovementTypeEnum.Assignment => "Zimmet",
            AssetMovementTypeEnum.Transfer => "Devir",
            AssetMovementTypeEnum.Return => "Zimmet İadesi",
            AssetMovementTypeEnum.Scrap => "Hurdaya Ayırma",
            _ => "Bilinmiyor"
        };

        /// <summary>
        /// Demirbaş hareketinin gerçekleştiği tarihtir.
        /// </summary>
        [Display(Name = "Hareket Tarihi")]
        public DateTime MovementDate { get; set; }

        /// <summary>
        /// Hareket kaydının açıklamasıdır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Hareket kaydının oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Hareket kaydının son güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
