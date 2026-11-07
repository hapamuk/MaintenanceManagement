using MaintenanceManagement.Application.DTOS.WarehouseDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public sealed class WarehouseRegistrationRepository : IWarehouseRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public WarehouseRegistrationRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateWithPersonnelAsync(
            CreateWarehouseDto dto,
            IReadOnlyCollection<int> personnelIds,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(personnelIds);

            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                ValidateWarehouseDto(dto);
                int unitId = dto.UnitId!.Value;
                var ids = ValidatePersonnelIds(personnelIds);
                await EnsureActiveUnitAsync(unitId, cancellationToken);
                await EnsureEligiblePersonnelAsync(unitId, ids, cancellationToken);
                await EnsureCodeAvailableAsync(dto.Code, null, cancellationToken);

                var warehouse = new Warehouse
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    UnitId = unitId,
                    Location = TrimOrNull(dto.Location),
                    Description = TrimOrNull(dto.Description),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Warehouses.Add(warehouse);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (int userId in ids)
                {
                    _context.WarehousePersonnels.Add(new WarehousePersonnel
                    {
                        WarehouseId = warehouse.Id,
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return warehouse.Id;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task<bool> UpdateWithPersonnelAsync(
            UpdateWarehouseDto dto,
            IReadOnlyCollection<int> personnelIds,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(personnelIds);

            if (dto.Id <= 0)
                throw new ValidationException("Geçerli bir ambar seçiniz.");

            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                var warehouse = await _context.Warehouses
                    .SingleOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
                if (warehouse == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                // Kullanıcının gönderdiği kod dikkate alınmaz.
                dto.Code = warehouse.Code;
                ValidateWarehouseDto(dto);

                int unitId = dto.UnitId!.Value;
                var ids = ValidatePersonnelIds(personnelIds);
                await EnsureActiveUnitAsync(unitId, cancellationToken);
                await EnsureEligiblePersonnelAsync(unitId, ids, cancellationToken);

                if (warehouse.UnitId != unitId)
                {
                    bool hasStock = await _context.WarehouseStocks
                        .AnyAsync(x => x.WarehouseId == warehouse.Id, cancellationToken);
                    bool hasMovement = await _context.StockMovements
                        .AnyAsync(x => x.WarehouseId == warehouse.Id, cancellationToken);
                    bool hasWorkOrderUsage = await _context.Set<WorkOrderMaterial>()
                        .AnyAsync(x => x.WarehouseId == warehouse.Id, cancellationToken);

                    if (hasStock || hasMovement || hasWorkOrderUsage)
                        throw new InvalidOperationException(
                            "Stok, stok hareketi veya iş emri malzeme geçmişi bulunan ambarın organizasyonu değiştirilemez.");
                }

                warehouse.Name = dto.Name.Trim();
                warehouse.UnitId = unitId;
                warehouse.Location = TrimOrNull(dto.Location);
                warehouse.Description = TrimOrNull(dto.Description);
                warehouse.UpdatedAt = DateTime.UtcNow;
                // Code, Id, CreatedAt, IsActive ve stok ilişkileri korunur.

                var assignments = await _context.WarehousePersonnels
                    .Where(x => x.WarehouseId == warehouse.Id)
                    .ToListAsync(cancellationToken);

                var existingIds = assignments.Select(x => x.UserId).ToHashSet();
                var requestedIds = ids.ToHashSet();

                _context.WarehousePersonnels.RemoveRange(
                    assignments.Where(x => !requestedIds.Contains(x.UserId)));

                foreach (var userId in requestedIds.Except(existingIds))
                {
                    _context.WarehousePersonnels.Add(new WarehousePersonnel
                    {
                        WarehouseId = warehouse.Id,
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task<IReadOnlyList<WarehousePersonnelCandidateDto>>
            GetEligiblePersonnelByUnitAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            if (unitId <= 0)
                throw new ValidationException("Geçerli bir organizasyon seçiniz.");

            return await _context.Users.AsNoTracking()
                .Where(x => x.PrimaryUnitId == unitId && x.IsActive)
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .Select(x => new WarehousePersonnelCandidateDto
                {
                    UserId = x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    UnitId = x.PrimaryUnitId,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<int>> GetAssignedPersonnelIdsAsync(
            int warehouseId,
            CancellationToken cancellationToken = default)
        {
            if (warehouseId <= 0)
                throw new ValidationException("Geçerli bir ambar seçiniz.");

            return await _context.WarehousePersonnels.AsNoTracking()
                .Where(x => x.WarehouseId == warehouseId)
                .Select(x => x.UserId)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(cancellationToken);
        }

        private async Task EnsureActiveUnitAsync(int unitId, CancellationToken ct)
        {
            bool exists = await _context.Units.AsNoTracking()
                .AnyAsync(x => x.Id == unitId && x.IsActive, ct);
            if (!exists)
                throw new ValidationException("Seçilen organizasyon bulunamadı veya pasif.");
        }

        private async Task EnsureEligiblePersonnelAsync(
            int unitId, IReadOnlyCollection<int> ids, CancellationToken ct)
        {
            if (ids.Count == 0)
                throw new ValidationException("En az bir görevli personel seçiniz.");

            int matched = await _context.Users.AsNoTracking()
                .CountAsync(x => ids.Contains(x.Id) &&
                    x.PrimaryUnitId == unitId && x.IsActive, ct);

            if (matched != ids.Count)
                throw new ValidationException(
                    "Seçilen personellerin tamamı aktif ve belirtilen organizasyona bağlı olmalıdır.");
        }

        private async Task EnsureCodeAvailableAsync(
            string code, int? excludedId, CancellationToken ct)
        {
            bool exists = await _context.Warehouses.AsNoTracking()
                .AnyAsync(x => x.Code == code &&
                    (!excludedId.HasValue || x.Id != excludedId.Value), ct);
            if (exists)
                throw new ValidationException("Ambar kodu daha önce kullanılmıştır.");
        }

        private static List<int> ValidatePersonnelIds(IReadOnlyCollection<int> personnelIds)
        {
            if (personnelIds.Any(x => x <= 0))
                throw new ValidationException("Geçersiz personel ID değeri.");
            return personnelIds.Distinct().ToList();
        }

        private static void ValidateWarehouseDto(object dto)
        {
            Validator.ValidateObject(dto, new ValidationContext(dto), true);
            string code;
            string name;
            int? unitId;
            if (dto is CreateWarehouseDto create)
            {
                code = create.Code;
                name = create.Name;
                unitId = create.UnitId;
            }
            else if (dto is UpdateWarehouseDto update)
            {
                code = update.Code;
                name = update.Name;
                unitId = update.UnitId;
            }
            else throw new ArgumentException("Desteklenmeyen ambar DTO.", nameof(dto));

            if (string.IsNullOrWhiteSpace(code) ||
                code.Length != 6 || code[0] < '1' || code[0] > '9' ||
                !code.All(x => x >= '0' && x <= '9'))
                throw new ValidationException("Ambar kodu 6 haneli rakamlardan oluşmalıdır.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Ambar adı zorunludur.");
            if (!unitId.HasValue || unitId.Value <= 0)
                throw new ValidationException("Organizasyon seçimi zorunludur.");
        }

        private static string? TrimOrNull(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
