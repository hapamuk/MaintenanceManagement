using MaintenanceManagement.Application.Interfaces;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Infrastructure.Repositories;
using MaintenanceManagement.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureRepositories(
            this IServiceCollection services)
        {
            services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            services.AddScoped<IRoleRepository, RoleRepository>();

            // Organization Type Repository
            services.AddScoped<
                IOrganizationTypeRepository,
                OrganizationTypeRepository>();

            // Unit Repository
            services.AddScoped<
                IUnitRepository,
                UnitRepository>();

            // User Repository
            services.AddScoped<
                IUserRepository,
                UserRepository>();

            // Permission Repository
            services.AddScoped<
                IPermissionRepository,
                PermissionRepository>();

            // Role Permission Repository
            services.AddScoped<
                IRolePermissionRepository,
                RolePermissionRepository>();

            // Role Work Sub Type Permission Repository
            services.AddScoped<
                IRoleWorkSubTypePermissionRepository,
                RoleWorkSubTypePermissionRepository>();

            // User Unit Role Repository
            services.AddScoped<
                IUserUnitRoleRepository,
                UserUnitRoleRepository>();

            // Team Repository
            services.AddScoped<
                ITeamRepository,
                TeamRepository>();

            // Team Member Repository
            services.AddScoped<
                ITeamMemberRepository,
                TeamMemberRepository>();

            // Brand Repository
            services.AddScoped<IBrandRepository, BrandRepository>();

            // Asset Model Repository
            services.AddScoped<
                IAssetModelRepository,
                AssetModelRepository>();

            // Asset Category Repository
            services.AddScoped<
                IAssetCategoryRepository,
                AssetCategoryRepository>();

            // Asset Kind Repository
            services.AddScoped<
                IAssetKindRepository,
                AssetKindRepository>();

            // Asset Repository
            services.AddScoped<IAssetRepository, AssetRepository>();

            // Asset User Repository
            services.AddScoped<
                IAssetUserRepository,
                AssetUserRepository>();

            // Asset Movement Repository
            services.AddScoped<
                IAssetMovementRepository,
                AssetMovementRepository>();

            // İş Türü ve İş Talebi Yönetimi
            services.AddScoped<IWorkTypeRepository, WorkTypeRepository>();

            services.AddScoped<
    IUnitWorkTypeRepository,
    UnitWorkTypeRepository>();

            // Work Sub Type Repository
            services.AddScoped<
                IWorkSubTypeRepository,
                WorkSubTypeRepository>();

            // Site Location Repository
            services.AddScoped<
                ISiteLocationRepository,
                SiteLocationRepository>();

            // Work Request Repository
            services.AddScoped<
                IWorkRequestRepository,
                WorkRequestRepository>();

            // Work Request File Repository
            services.AddScoped<
                IWorkRequestFileRepository,
                WorkRequestFileRepository>();

            services.AddScoped<
                IWorkRequestActionRepository,
                WorkRequestActionRepository>();

            // İş Emri Yönetimi
            services.AddScoped<
                IWorkOrderRepository,
                WorkOrderRepository>();

            // İş Emri Personel Görevlendirmeleri
            services.AddScoped<
                IWorkOrderPersonnelRepository,
                WorkOrderPersonnelRepository>();

            // İş Emri Faaliyet ve İşçilik Repository
            services.AddScoped<
                IWorkOrderActionRepository,
                WorkOrderActionRepository>();

            // İş Emri Durum Geçmişi Repository
            services.AddScoped<
                IWorkOrderStatusHistoryRepository,
                WorkOrderStatusHistoryRepository>();

            // İş Emri Dosyaları Repository
            services.AddScoped<
                IWorkOrderFileRepository,
                WorkOrderFileRepository>();

            // Planlı Bakım Repository
            services.AddScoped<
                IPlannedMaintenanceRepository,
                PlannedMaintenanceRepository>();

            // Bakım Kontrol Listesi Repository
            services.AddScoped<
                IMaintenanceChecklistRepository,
                MaintenanceChecklistRepository>();

            // İş Emri Bakım Kontrol Sonuçları Repository
            services.AddScoped<
                IWorkOrderChecklistResultRepository,
                WorkOrderChecklistResultRepository>();

            // İş Emri Malzeme Kullanım Repository
            services.AddScoped<
                IWorkOrderMaterialRepository,
                WorkOrderMaterialRepository>();

            // Ölçü Birimi Repository
            services.AddScoped<
                IMeasurementUnitRepository,
                MeasurementUnitRepository>();

            // Malzeme Kategorisi Repository
            services.AddScoped<
                IMaterialCategoryRepository,
                MaterialCategoryRepository>();

            // Malzeme Repository
            services.AddScoped<IMaterialRepository, MaterialRepository>();

            // Ambar Repository
            services.AddScoped<
            IWarehouseRepository,
            WarehouseRepository>();

            // Ambar Stokları Repository
            services.AddScoped<
                IWarehouseStockRepository,
                WarehouseStockRepository>();

            // Stok Hareketleri Repository
            services.AddScoped<
                IStockMovementRepository,
                StockMovementRepository>();

            services.AddScoped<
    IPasswordResetTokenRepository,
    PasswordResetTokenRepository>();

            services.AddScoped<
                IPasswordResetEmailSender,
                SmtpPasswordResetEmailSender>();

            services.AddScoped<IUnitCodeRepository, UnitCodeRepository>();

            return services;
        }
    }
}
