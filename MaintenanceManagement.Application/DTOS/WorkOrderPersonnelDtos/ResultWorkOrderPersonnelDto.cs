using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderPersonnelDtos
{
    public class ResultWorkOrderPersonnelDto
    {
        /// <summary>
        /// Personel atama kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Atama ID")]
        public int Id { get; set; }

        /// <summary>
        /// İlgili iş emrinin ID bilgisidir.
        /// </summary>
        [Display(Name = "İş Emri ID")]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// İlgili iş emrinin numarasıdır.
        /// </summary>
        [Display(Name = "İş Emri Numarası")]
        public string WorkOrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin başlığıdır.
        /// </summary>
        [Display(Name = "İş Emri Başlığı")]
        public string WorkOrderTitle { get; set; } = string.Empty;

        /// <summary>
        /// Atanan ekip üyesinin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip Üyesi ID")]
        public int TeamMemberId { get; set; }

        /// <summary>
        /// Ekip üyesinin bağlı olduğu ekibin ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip ID")]
        public int TeamId { get; set; }

        /// <summary>
        /// Ekip üyesinin bağlı olduğu ekibin adıdır.
        /// </summary>
        [Display(Name = "Ekip Adı")]
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// Ekip üyesiyle ilişkili kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Personel ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Ekip üyesinin adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Personel Adı Soyadı")]
        public string UserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Personelin iş emrindeki görev rolüdür.
        /// </summary>
        [Display(Name = "Görev Rolü")]
        public AssignmentRoleEnum AssignmentRole { get; set; }

        /// <summary>
        /// Görev rolünün Türkçe açıklamasıdır.
        /// </summary>
        [Display(Name = "Görev Rolü Açıklaması")]
        public string AssignmentRoleName => AssignmentRole switch
        {
            AssignmentRoleEnum.Responsible => "Sorumlu Personel",
            AssignmentRoleEnum.Assistant => "Yardımcı Personel",
            _ => "Bilinmiyor"
        };

        /// <summary>
        /// Atamayı gerçekleştiren kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Atayan Kullanıcı ID")]
        public int AssignedByUserId { get; set; }

        /// <summary>
        /// Atamayı yapan kullanıcının adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Atayan Kullanıcı")]
        public string AssignedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// Personelin iş emrine atandığı tarihtir.
        /// </summary>
        [Display(Name = "Atanma Tarihi")]
        public DateTime AssignedAt { get; set; }

        /// <summary>
        /// Personelin görevden ayrıldığı tarihtir.
        /// </summary>
        [Display(Name = "Görevden Ayrılma Tarihi")]
        public DateTime? ReleasedAt { get; set; }

        /// <summary>
        /// Atamanın aktiflik durumudur.
        /// </summary>
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Personelin bu iş emrindeki görevinin
        /// devam edip etmediğini gösterir.
        /// </summary>
        [Display(Name = "Görevi Devam Ediyor")]
        public bool IsOngoing => IsActive && !ReleasedAt.HasValue;

        /// <summary>
        /// Personel atamasıyla ilgili açıklamadır.
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
