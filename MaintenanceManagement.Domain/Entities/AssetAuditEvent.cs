using System;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceManagement.Domain.Entities
{
    // Zimmet ve iş emirleri burada saklanmaz; onlar ilgili kaynak tablolardan raporlanır.
    public sealed class AssetAuditEvent : BaseEntity
    {
        public int AssetId { get; set; }

        [Required, StringLength(120)]
        public string EventType { get; set; } = string.Empty;

        [Required, StringLength(80)]
        public string FieldName { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? OldValue { get; set; }

        [StringLength(2000)]
        public string? NewValue { get; set; }

        [Required, StringLength(4000)]
        public string Description { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public int PerformedByUserId { get; set; }
        public Asset Asset { get; set; } = null!;
    }
}
