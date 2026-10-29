using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DTOS.WorkOrderPersonnelDtos
{
    public class GetByIdWorkOrderPersonnelDto
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
        /// İş emrinin benzersiz numarasıdır.
        /// </summary>
        [Display(Name = "İş Emri Numarası")]
        public string WorkOrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// İş emrinin başlığıdır.
        /// </summary>
        [Display(Name = "İş Emri Başlığı")]
        public string WorkOrderTitle { get; set; } = string.Empty;

        /// <summary>
        /// Ekip üyeliği kaydının ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip Üyesi ID")]
        public int TeamMemberId { get; set; }

        /// <summary>
        /// Ekip ID bilgisidir.
        /// </summary>
        [Display(Name = "Ekip ID")]
        public int TeamId { get; set; }

        /// <summary>
        /// Ekip adıdır.
        /// </summary>
        [Display(Name = "Ekip Adı")]
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// İlgili kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Personel ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Personelin adı ve soyadıdır.
        /// </summary>
        [Display(Name = "Personel Adı Soyadı")]
        public string UserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Personelin e-posta adresidir.
        /// </summary>
        [Display(Name = "Personel E-Posta")]
        public string? UserEmail { get; set; }

        /// <summary>
        /// Personelin unvanıdır.
        /// </summary>
        [Display(Name = "Personel Unvanı")]
        public string? UserTitle { get; set; }

        /// <summary>
        /// Kullanıcının aktiflik durumudur.
        /// </summary>
        [Display(Name = "Personel Durumu")]
        public bool UserIsActive { get; set; }

        /// <summary>
        /// Ekip üyeliğinin aktiflik durumudur.
        /// </summary>
        [Display(Name = "Ekip Üyeliği Aktif")]
        public bool TeamMemberIsActive { get; set; }

        /// <summary>
        /// Personelin ekibe katılma tarihidir.
        /// </summary>
        [Display(Name = "Ekibe Katılma Tarihi")]
        public DateTime TeamMemberJoinedAt { get; set; }

        /// <summary>
        /// Personelin ekipten ayrılma tarihidir.
        /// </summary>
        [Display(Name = "Ekipten Ayrılma Tarihi")]
        public DateTime? TeamMemberLeftAt { get; set; }

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
        /// Atamayı yapan kullanıcının ID bilgisidir.
        /// </summary>
        [Display(Name = "Atayan Kullanıcı ID")]
        public int AssignedByUserId { get; set; }

        /// <summary>
        /// Atamayı yapan kullanıcının adıdır.
        /// </summary>
        [Display(Name = "Atayan Kullanıcı")]
        public string AssignedByUserFullName { get; set; }
            = string.Empty;

        /// <summary>
        /// Atamayı yapan kullanıcının e-posta adresidir.
        /// </summary>
        [Display(Name = "Atayan Kullanıcı E-Posta")]
        public string? AssignedByUserEmail { get; set; }

        /// <summary>
        /// Personelin iş emrine atanma tarihidir.
        /// </summary>
        [Display(Name = "Atanma Tarihi")]
        public DateTime AssignedAt { get; set; }

        /// <summary>
        /// Personelin görevden ayrılma tarihidir.
        /// </summary>
        [Display(Name = "Görevden Ayrılma Tarihi")]
        public DateTime? ReleasedAt { get; set; }

        /// <summary>
        /// Atama kaydının aktiflik durumudur.
        /// </summary>
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Personelin iş emrindeki görevinin
        /// devam edip etmediğini gösterir.
        /// </summary>
        [Display(Name = "Görevi Devam Ediyor")]
        public bool IsOngoing => IsActive && !ReleasedAt.HasValue;

        /// <summary>
        /// Atamayla ilgili açıklamadır.
        /// </summary>
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        /// <summary>
        /// Kaydın oluşturulma tarihidir.
        /// </summary>
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kaydın güncellenme tarihidir.
        /// </summary>
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
    }
}
