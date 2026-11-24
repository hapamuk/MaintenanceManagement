using System;
using System.Collections.Generic;

namespace MaintenanceManagement.Application.DTOs.AssetCatalog
{
    public sealed class AssetListFilter
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public int? CategoryId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public sealed class AssetListRowVm
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Kind { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string ResponsibleUnit { get; set; } = "";
        public string? UsingUnit { get; set; }
        public string? Barcode { get; set; }
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public sealed class AssetCategoryFilterVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public sealed class AssetListPageVm
    {
        public AssetListFilter Filter { get; set; } = new();
        public IReadOnlyList<AssetListRowVm> Items { get; set; } = Array.Empty<AssetListRowVm>();
        public IReadOnlyList<AssetCategoryFilterVm> Categories { get; set; } = Array.Empty<AssetCategoryFilterVm>();
        public int TotalCount { get; set; }
        public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)Filter.PageSize);
        public int FirstRecord => TotalCount == 0 ? 0 : (Filter.Page - 1) * Filter.PageSize + 1;
        public int LastRecord => Math.Min(TotalCount, Filter.Page * Filter.PageSize);
    }
}
