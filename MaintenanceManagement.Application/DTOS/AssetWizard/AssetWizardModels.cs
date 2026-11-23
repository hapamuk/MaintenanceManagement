using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceManagement.Application.DTOs.AssetWizard
{
    public sealed class AssetWizardInput
    {
        [StringLength(50)] public string AssetCode { get; set; } = "";
        [Required, StringLength(200)] public string Name { get; set; } = "";
        public int AssetCategoryId { get; set; }
        // UI üzerinde gizlidir; kategoriye göre servis otomatik belirler.
        public int AssetKindId { get; set; }
        public int BrandId { get; set; }
        public int AssetModelId { get; set; }
        [StringLength(100)] public string? BarcodeNumber { get; set; }
        public int? SiteLocationId { get; set; }
        public int ResponsibleUnitId { get; set; }
        public int UsingUnitId { get; set; }
        [Required] public string Status { get; set; } = "Active";
        [StringLength(500)] public string? Description { get; set; }
        public int? AssigneeUserId { get; set; }
    }
    public sealed class WizardOption { public int Id { get; set; } public string Text { get; set; } = ""; public int? ParentId { get; set; } public int? GroupId { get; set; } }
    public sealed class AssetCategoryScopeRow { public int CategoryId {get;set;} public int UnitId {get;set;} }
    public sealed class AssetWizardPageVm
    {
        public AssetWizardInput Input { get; set; } = new();
        public IReadOnlyList<AssetCategoryScopeRow> CategoryScopes { get; set; } = Array.Empty<AssetCategoryScopeRow>();
        public IReadOnlyList<WizardOption> Units { get; set; } = Array.Empty<WizardOption>();
        public IReadOnlyList<WizardOption> Categories { get; set; } = Array.Empty<WizardOption>();
        public IReadOnlyList<WizardOption> Kinds { get; set; } = Array.Empty<WizardOption>();
        public IReadOnlyList<WizardOption> Brands { get; set; } = Array.Empty<WizardOption>();
        public IReadOnlyList<WizardOption> Models { get; set; } = Array.Empty<WizardOption>();
        public IReadOnlyList<WizardOption> Locations { get; set; } = Array.Empty<WizardOption>();
        public IReadOnlyList<WizardOption> Users { get; set; } = Array.Empty<WizardOption>();
    }
    public sealed class AssetHistoryRow
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public string Reference { get; set; } = "";
        public string PerformedBy { get; set; } = "";
    }
    public sealed class AssetAssignmentRow
    {
        public string User { get; set; } = "";
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public bool IsCurrent => !End.HasValue;
    }
    public sealed class AssetDetailPageVm
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string BrandModel { get; set; } = "";
        public string Barcode { get; set; } = "";
        public string Location { get; set; } = "";
        public string ResponsibleUnit { get; set; } = "";
        public string UsingUnit { get; set; } = "";
        public string Status { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public IReadOnlyList<AssetHistoryRow> History { get; set; } = Array.Empty<AssetHistoryRow>();
        public IReadOnlyList<AssetHistoryRow> WorkOrders { get; set; } = Array.Empty<AssetHistoryRow>();
        public IReadOnlyList<AssetAssignmentRow> Assignments { get; set; } = Array.Empty<AssetAssignmentRow>();
    }
}
