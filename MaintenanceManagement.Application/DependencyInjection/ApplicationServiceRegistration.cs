using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Application.Mapping;
using MaintenanceManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(GeneralMapping));

            //Role Services

            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<
                IOrganizationTypeService,
                OrganizationTypeService>();

            services.AddScoped<IUnitService, UnitService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IPermissionService, PermissionService>();

            services.AddScoped<IRolePermissionService, RolePermissionService>();

            services.AddScoped<IRoleWorkSubTypePermissionService, RoleWorkSubTypePermissionService>();

            services.AddScoped<IUserUnitRoleService, UserUnitRoleService>();

            services.AddScoped<ITeamService, TeamService>();

            services.AddScoped<ITeamMemberService, TeamMemberService>();

            services.AddScoped<IBrandService, BrandService>();

            services.AddScoped<
                IAssetModelService,
                AssetModelService>();

            services.AddScoped<IAssetCategoryService, AssetCategoryService>();

            services.AddScoped<IAssetKindService, AssetKindService>();

            services.AddScoped<IAssetService, AssetService>();

            services.AddScoped<IAssetUserService, AssetUserService>();

            services.AddScoped<IAssetMovementService, AssetMovementService>();

            services.AddScoped<IWorkTypeService, WorkTypeService>();

            services.AddScoped<IUnitWorkTypeService, UnitWorkTypeService>();

            services.AddScoped<IWorkSubTypeService, WorkSubTypeService>();

            services.AddScoped<ISiteLocationService, SiteLocationService>();

            services.AddScoped<IWorkRequestService, WorkRequestService>();

            services.AddScoped<IWorkRequestFileService, WorkRequestFileService>();

            services.AddScoped<IWorkRequestActionService, WorkRequestActionService>();


            services.AddScoped<IWorkOrderService, WorkOrderService>();

            services.AddScoped<IWorkOrderPersonnelService, WorkOrderPersonnelService>();

            services.AddScoped<IWorkOrderActionService, WorkOrderActionService>();

            services.AddScoped<IWorkOrderStatusHistoryService, WorkOrderStatusHistoryService>();

            services.AddScoped<IWorkOrderFileService, WorkOrderFileService>();

            services.AddScoped<IPlannedMaintenanceService, PlannedMaintenanceService>();

            services.AddScoped<IMaintenanceChecklistService, MaintenanceChecklistService>();

            services.AddScoped<IWorkOrderChecklistResultService, WorkOrderChecklistResultService>();

            services.AddScoped<IWorkOrderMaterialService, WorkOrderMaterialService>();

            services.AddScoped<IMeasurementUnitService, MeasurementUnitService>();

            services.AddScoped<IMaterialCategoryService, MaterialCategoryService>();

            services.AddScoped<IMaterialService, MaterialService>();

            services.AddScoped<IWarehouseService, WarehouseService>();

            services.AddScoped<IWarehouseStockService, WarehouseStockService>();

            services.AddScoped<IStockMovementService, StockMovementService>();

            services.AddScoped<
    IPasswordResetService,
    PasswordResetService>();

            return services;
        }
    }
}
