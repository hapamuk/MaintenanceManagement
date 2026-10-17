using MaintenanceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region Organizasyon, Kullanıcı, Rol ve Yetki Yönetimi

        public DbSet<OrganizationType> OrganizationTypes
        { get; set; } = null!;

        public DbSet<Unit> Units
        { get; set; } = null!;

        public DbSet<User> Users
        { get; set; } = null!;

        public DbSet<Role> Roles
        { get; set; } = null!;

        public DbSet<Permission> Permissions
        { get; set; } = null!;

        public DbSet<RolePermission> RolePermissions
        { get; set; } = null!;

        public DbSet<RoleWorkSubTypePermission> RoleWorkSubTypePermissions
        { get; set; } = null!;

        public DbSet<UserUnitRole> UserUnitRoles
        { get; set; } = null!;

        public DbSet<Team> Teams
        { get; set; } = null!;

        public DbSet<TeamMember> TeamMembers
        { get; set; } = null!;

        #endregion


        #region Demirbaş Yönetimi

        public DbSet<Brand> Brands
        { get; set; } = null!;

        public DbSet<AssetModel> AssetModels
        { get; set; } = null!;

        public DbSet<AssetCategory> AssetCategories
        { get; set; } = null!;

        public DbSet<AssetKind> AssetKinds
        { get; set; } = null!;

        public DbSet<Asset> Assets
        { get; set; } = null!;

        public DbSet<AssetUser> AssetUsers
        { get; set; } = null!;

        public DbSet<AssetMovement> AssetMovements
        { get; set; } = null!;

        #endregion


        #region İş Türü ve İş Talebi Yönetimi

        public DbSet<WorkType> WorkTypes
        { get; set; } = null!;

        public DbSet<UnitWorkType> UnitWorkTypes
        { get; set; } = null!;

        public DbSet<WorkSubType> WorkSubTypes
        { get; set; } = null!;

        public DbSet<SiteLocation> SiteLocations
        { get; set; } = null!;

        public DbSet<WorkRequest> WorkRequests
        { get; set; } = null!;

        public DbSet<WorkRequestFile> WorkRequestFiles
        { get; set; } = null!;

        public DbSet<WorkRequestAction> WorkRequestActions
        { get; set; } = null!;

        #endregion


        #region İş Emri ve Bakım Yönetimi

        public DbSet<WorkOrder> WorkOrders
        { get; set; } = null!;

        public DbSet<WorkOrderPersonnel> WorkOrderPersonnels
        { get; set; } = null!;

        public DbSet<WorkOrderAction> WorkOrderActions
        { get; set; } = null!;

        public DbSet<WorkOrderStatusHistory> WorkOrderStatusHistories
        { get; set; } = null!;

        public DbSet<WorkOrderFile> WorkOrderFiles
        { get; set; } = null!;

        public DbSet<PlannedMaintenance> PlannedMaintenances
        { get; set; } = null!;

        public DbSet<MaintenanceChecklist> MaintenanceChecklists
        { get; set; } = null!;

        public DbSet<WorkOrderChecklistResult> WorkOrderChecklistResults
        { get; set; } = null!;

        public DbSet<WorkOrderMaterial> WorkOrderMaterials
        { get; set; } = null!;

        #endregion


        #region Malzeme, Ambar ve Stok Yönetimi

        public DbSet<MeasurementUnit> MeasurementUnits
        { get; set; } = null!;

        public DbSet<MaterialCategory> MaterialCategories
        { get; set; } = null!;

        public DbSet<Material> Materials
        { get; set; } = null!;

        public DbSet<Warehouse> Warehouses
        { get; set; } = null!;

        public DbSet<WarehouseStock> WarehouseStocks
        { get; set; } = null!;

        public DbSet<StockMovement> StockMovements
        { get; set; } = null!;

        #endregion
    }
}

