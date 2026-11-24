using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetCatalog;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Enums;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.Infrastructure.Services
{
    public sealed class AssetCatalogService : IAssetCatalogService
    {
        private readonly ApplicationDbContext _db;
        public AssetCatalogService(ApplicationDbContext db) => _db = db;

        public async Task<AssetListPageVm> GetListAsync(AssetListFilter filter, CancellationToken ct = default)
        {
            filter ??= new AssetListFilter();
            var normalized = new AssetListFilter
            {
                Search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search.Trim(),
                Status = string.IsNullOrWhiteSpace(filter.Status) ? null : filter.Status.Trim(),
                CategoryId = filter.CategoryId > 0 ? filter.CategoryId : null,
                Page = Math.Max(1, filter.Page),
                PageSize = new[] { 10, 20, 50, 100 }.Contains(filter.PageSize) ? filter.PageSize : 20
            };
            if (normalized.Search?.Length > 100) normalized.Search = normalized.Search.Substring(0, 100);

            var query = _db.Assets.AsNoTracking().AsQueryable();
            if (normalized.Search is { Length: > 0 } term)
            {
                query = query.Where(x =>
                    x.AssetCode.Contains(term) ||
                    x.Name.Contains(term) ||
                    (x.BarcodeNumber != null && x.BarcodeNumber.Contains(term)) ||
                    (x.SerialNumber != null && x.SerialNumber.Contains(term)) ||
                    (x.InventoryNumber != null && x.InventoryNumber.Contains(term)));
            }
            if (normalized.CategoryId.HasValue)
                query = query.Where(x => x.AssetKind.AssetCategoryId == normalized.CategoryId.Value);

            if (normalized.Status is { Length: > 0 } rawStatus)
            {
                if (Enum.TryParse<AssetStatusEnum>(rawStatus, true, out var status) &&
                    Enum.IsDefined(typeof(AssetStatusEnum), status))
                    query = query.Where(x => x.Status == status);
                else normalized.Status = null;
            }

            var total = await query.CountAsync(ct);
            var pages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)normalized.PageSize);
            if (pages > 0) normalized.Page = Math.Min(normalized.Page, pages);
            else normalized.Page = 1;

            // Veritabanı seviyesinde projection + Skip/Take: tüm liste belleğe alınmaz.
            var records = await query
                .OrderByDescending(x => x.Id)
                .Skip((normalized.Page - 1) * normalized.PageSize)
                .Take(normalized.PageSize)
                .Select(x => new
                {
                    x.Id, x.AssetCode, x.Name, x.BarcodeNumber, x.Status, x.CreatedAt,
                    Category = x.AssetKind.AssetCategory.Name,
                    Kind = x.AssetKind.Name,
                    Brand = x.AssetModel.Brand.Name,
                    Model = x.AssetModel.Name,
                    ResponsibleUnit = x.Unit.Name,
                    UsingUnit = x.UsingUnit != null ? x.UsingUnit.Name : null
                })
                .ToListAsync(ct);

            var categories = await _db.AssetCategories.AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new AssetCategoryFilterVm { Id = x.Id, Name = x.Name })
                .ToListAsync(ct);

            return new AssetListPageVm
            {
                Filter = normalized,
                TotalCount = total,
                Categories = categories,
                Items = records.Select(x => new AssetListRowVm
                {
                    Id = x.Id, Code = x.AssetCode, Name = x.Name,
                    Category = x.Category, Kind = x.Kind, Brand = x.Brand,
                    Model = x.Model, ResponsibleUnit = x.ResponsibleUnit,
                    UsingUnit = x.UsingUnit, Barcode = x.BarcodeNumber,
                    Status = x.Status.ToString(), CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
