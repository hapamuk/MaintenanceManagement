using AutoMapper;
using MaintenanceManagement.Application.DTOS.AssetCategoryDtos;
using MaintenanceManagement.Application.DTOS.AssetDtos;
using MaintenanceManagement.Application.DTOS.AssetKindDtos;
using MaintenanceManagement.Application.DTOS.AssetModelDtos;
using MaintenanceManagement.Application.DTOS.AssetMovementDtos;
using MaintenanceManagement.Application.DTOS.AssetUserDtos;
using MaintenanceManagement.Application.DTOS.BrandDtos;
using MaintenanceManagement.Application.DTOS.MaintenanceChecklistDtos;
using MaintenanceManagement.Application.DTOS.MaterialCategoryDtos;
using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.DTOS.MeasurementUnitDtos;
using MaintenanceManagement.Application.DTOS.OrganizationTypeDtos;
using MaintenanceManagement.Application.DTOS.PermissionDtos;
using MaintenanceManagement.Application.DTOS.PlannedMaintenancesDtos;
using MaintenanceManagement.Application.DTOS.RoleDtos;
using MaintenanceManagement.Application.DTOS.RolePermissionDtos;
using MaintenanceManagement.Application.DTOS.RoleWorkSubTypePermissionDtos;
using MaintenanceManagement.Application.DTOS.SiteLocationDtos;
using MaintenanceManagement.Application.DTOS.StockMovementDtos;
using MaintenanceManagement.Application.DTOS.TeamDtos;
using MaintenanceManagement.Application.DTOS.TeamMemberDtos;
using MaintenanceManagement.Application.DTOS.UnitDtos;
using MaintenanceManagement.Application.DTOS.UnitWorkTypeDtos;
using MaintenanceManagement.Application.DTOS.UserDtos;
using MaintenanceManagement.Application.DTOS.UserUnitRoleDtos;
using MaintenanceManagement.Application.DTOS.WarehouseDtos;
using MaintenanceManagement.Application.DTOS.WarehouseStockDtos;
using MaintenanceManagement.Application.DTOS.WorkOrderActionDtos;
using MaintenanceManagement.Application.DTOS.WorkOrderChecklistResultDtos;
using MaintenanceManagement.Application.DTOS.WorkOrderFilesDtos;
using MaintenanceManagement.Application.DTOS.WorkOrderMaterialDtos;
using MaintenanceManagement.Application.DTOS.WorkOrderPersonnelDtos;
using MaintenanceManagement.Application.DTOS.WorkOrdersDtos;
using MaintenanceManagement.Application.DTOS.WorkOrderStatusHistoryDtos;
using MaintenanceManagement.Application.DTOS.WorkRequestActionDtos;
using MaintenanceManagement.Application.DTOS.WorkRequestDtos;
using MaintenanceManagement.Application.DTOS.WorkRequestFileDtos;
using MaintenanceManagement.Application.DTOS.WorkSubTypeDtos;
using MaintenanceManagement.Application.DTOS.WorkTypeDtos;
using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Mapping
{
    public class GeneralMapping:Profile
    {

        public GeneralMapping()
        {
            #region Role Mapping

            CreateMap<CreateRoleDto, Role>();

            CreateMap<UpdateRoleDto, Role>();

            CreateMap<Role, ResultRoleDto>()
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(
                        src => src.Unit.Name));

            CreateMap<Role, GetByIdRoleDto>()
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(
                        src => src.Unit.Name))
                .ForMember(
                    dest => dest.ActiveUserCount,
                    opt => opt.MapFrom(src =>
                        src.UserUnitRoles
                            .Where(x => x.IsActive)
                            .Select(x => x.UserId)
                            .Distinct()
                            .Count()))
                .ForMember(
                    dest => dest.PermissionCount,
                    opt => opt.MapFrom(src =>
                        src.RolePermissions.Count(x => x.IsActive)))
                .ForMember(
                    dest => dest.WorkSubTypePermissionCount,
                    opt => opt.MapFrom(src =>
                        src.WorkSubTypePermissions.Count(x => x.IsActive)));

            #endregion


            #region OrganizationType Mapping

            // Yeni organizasyon türü oluşturma
            CreateMap<CreateOrganizationTypeDto, OrganizationType>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Units,
                    opt => opt.Ignore());

            // Organizasyon türü güncelleme
            CreateMap<UpdateOrganizationTypeDto, OrganizationType>()
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Units,
                    opt => opt.Ignore());

            // Organizasyon türü listeleme
            CreateMap<OrganizationType, ResultOrganizationTypeDto>();

            // Organizasyon türü detayları
            CreateMap<OrganizationType, GetByIdOrganizationTypeDto>()
                .ForMember(
                    dest => dest.UnitCount,
                    opt => opt.MapFrom(src =>
                        src.Units.Count))
                .ForMember(
                    dest => dest.ActiveUnitCount,
                    opt => opt.MapFrom(src =>
                        src.Units.Count(x => x.IsActive)))
                .ForMember(
                    dest => dest.InactiveUnitCount,
                    opt => opt.MapFrom(src =>
                        src.Units.Count(x => !x.IsActive)));

            #endregion

            #region Unit Mapping
            CreateMap<CreateUnitDto, Unit>()
    .ForMember(
        dest => dest.Id,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.IsActive,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.CreatedAt,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.UpdatedAt,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.OrganizationType,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.ParentUnit,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.ChildUnits,
        opt => opt.Ignore());

            // Organizasyon birimi güncelleme
            CreateMap<UpdateUnitDto, Unit>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.OrganizationType,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ParentUnit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Code,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ChildUnits,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<Unit, UpdateUnitDto>();

            // Organizasyon birimi listeleme
            CreateMap<Unit, ResultUnitDto>()
                .ForMember(
                    dest => dest.OrganizationTypeName,
                    opt => opt.MapFrom(src =>
                        src.OrganizationType != null
                            ? src.OrganizationType.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.ParentUnitName,
                    opt => opt.MapFrom(src =>
                        src.ParentUnit != null
                            ? src.ParentUnit.Name
                            : null));

            // Organizasyon birimi detayları
            CreateMap<Unit, GetByIdUnitDto>()
                .ForMember(
                    dest => dest.OrganizationTypeName,
                    opt => opt.MapFrom(src =>
                        src.OrganizationType != null
                            ? src.OrganizationType.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.ParentUnitName,
                    opt => opt.MapFrom(src =>
                        src.ParentUnit != null
                            ? src.ParentUnit.Name
                            : null))
                .ForMember(
                    dest => dest.ChildUnitCount,
                    opt => opt.MapFrom(src =>
                        src.ChildUnits.Count))
                .ForMember(
                    dest => dest.ActiveChildUnitCount,
                    opt => opt.MapFrom(src =>
                        src.ChildUnits.Count(x => x.IsActive)))
                .ForMember(
                    dest => dest.UserCount,
                    opt => opt.MapFrom(src =>
                        src.Users.Count))
                .ForMember(
                    dest => dest.RoleCount,
                    opt => opt.MapFrom(src =>
                        src.Roles.Count))
                .ForMember(
                    dest => dest.TeamCount,
                    opt => opt.MapFrom(src =>
                        src.Teams.Count))
                .ForMember(
                    dest => dest.AssetCount,
                    opt => opt.MapFrom(src =>
                        src.Assets.Count));


            #endregion

            #region User Mapping

            // Yönetici tarafından kullanıcı oluşturma
            CreateMap<CreateUserDto, User>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PrimaryUnit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UserUnitRoles,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.TeamMemberships,
                    opt => opt.Ignore());

            // Kullanıcı bilgilerini güncelleme
            CreateMap<UpdateUserDto, User>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.HasSystemAccess,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PrimaryUnit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UserUnitRoles,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.TeamMemberships,
                    opt => opt.Ignore());

            // Kullanıcı güncelleme formunu doldurma
            CreateMap<User, UpdateUserDto>();

            // Kullanıcı listeleme
            CreateMap<User, ResultUserDto>()
                .ForMember(
                    dest => dest.FullName,
                    opt => opt.MapFrom(src =>
                        src.FirstName + " " + src.LastName))
                .ForMember(
                    dest => dest.PrimaryUnitName,
                    opt => opt.MapFrom(src =>
                        src.PrimaryUnit != null
                            ? src.PrimaryUnit.Name
                            : string.Empty));

            // Kullanıcı detayları
            CreateMap<User, GetByIdUserDto>()
                .ForMember(
                    dest => dest.FullName,
                    opt => opt.MapFrom(src =>
                        src.FirstName + " " + src.LastName))
                .ForMember(
                    dest => dest.PrimaryUnitName,
                    opt => opt.MapFrom(src =>
                        src.PrimaryUnit != null
                            ? src.PrimaryUnit.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.ActiveRoleAssignmentCount,
                    opt => opt.MapFrom(src =>
                        src.UserUnitRoles.Count(x => x.IsActive)))
                .ForMember(
                    dest => dest.ActiveTeamMembershipCount,
                    opt => opt.MapFrom(src =>
                        src.TeamMemberships.Count(x => x.IsActive)));

            // Kullanıcı kayıt talebi
            CreateMap<RegisterUserDto, User>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.HasSystemAccess,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PrimaryUnit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UserUnitRoles,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.TeamMemberships,
                    opt => opt.Ignore());

            // Kullanıcı profilini görüntüleme
            CreateMap<User, UserProfileDto>()
                .ForMember(
                    dest => dest.FullName,
                    opt => opt.MapFrom(src =>
                        src.FirstName + " " + src.LastName))
                .ForMember(
                    dest => dest.PrimaryUnitName,
                    opt => opt.MapFrom(src =>
                        src.PrimaryUnit != null
                            ? src.PrimaryUnit.Name
                            : string.Empty));

            // Profil güncelleme formunu doldurma
            CreateMap<User, UpdateUserProfileDto>();

            // Profil bilgilerini güncelleme
            CreateMap<UpdateUserProfileDto, User>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Email,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Username,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Title,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PrimaryUnitId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.HasSystemAccess,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PrimaryUnit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UserUnitRoles,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.TeamMemberships,
                    opt => opt.Ignore());

            #endregion

            #region Permission Mapping

            // Permission Mapping
            // ===================================================

            // Yeni yetki oluşturma
            CreateMap<CreatePermissionDto, Permission>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.RolePermissions,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkSubTypePermissions,
                    opt => opt.Ignore());

            // Mevcut yetkiyi güncelleme
            CreateMap<UpdatePermissionDto, Permission>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.RolePermissions,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkSubTypePermissions,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<Permission, UpdatePermissionDto>();

            // Yetkileri listeleme
            CreateMap<Permission, ResultPermissionDto>();

            // Yetki detaylarını görüntüleme
            CreateMap<Permission, GetByIdPermissionDto>()
                .ForMember(
                    dest => dest.ActiveRoleCount,
                    opt => opt.MapFrom(src =>
                        src.RolePermissions
                            .Where(x => x.IsActive)
                            .Select(x => x.RoleId)
                            .Distinct()
                            .Count()))
                .ForMember(
                    dest => dest.ActiveWorkSubTypePermissionCount,
                    opt => opt.MapFrom(src =>
                        src.WorkSubTypePermissions
                            .Count(x => x.IsActive)));

            #endregion

            #region RolePermission Mapping

            // RolePermission Mapping
            // ===================================================

            // Yeni rol-yetki ilişkisi oluşturma
            CreateMap<CreateRolePermissionDto, RolePermission>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Role,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Permission,
                    opt => opt.Ignore());

            // Mevcut rol-yetki ilişkisini güncelleme
            CreateMap<UpdateRolePermissionDto, RolePermission>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Role,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Permission,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<RolePermission, UpdateRolePermissionDto>();

            // Rol-yetki ilişkilerini listeleme
            CreateMap<RolePermission, ResultRolePermissionDto>()
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(
                    dest => dest.RoleCode,
                    opt => opt.MapFrom(src => src.Role.Code))
                .ForMember(
                    dest => dest.RoleIsActive,
                    opt => opt.MapFrom(src => src.Role.IsActive))
                .ForMember(
                    dest => dest.PermissionName,
                    opt => opt.MapFrom(src => src.Permission.Name))
                .ForMember(
                    dest => dest.PermissionCode,
                    opt => opt.MapFrom(src => src.Permission.Code))
                .ForMember(
                    dest => dest.PermissionModule,
                    opt => opt.MapFrom(src => src.Permission.Module))
                .ForMember(
                    dest => dest.PermissionIsActive,
                    opt => opt.MapFrom(src => src.Permission.IsActive));

            // Rol-yetki ilişkisinin detayları
            CreateMap<RolePermission, GetByIdRolePermissionDto>()
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(
                    dest => dest.RoleCode,
                    opt => opt.MapFrom(src => src.Role.Code))
                .ForMember(
                    dest => dest.RoleDescription,
                    opt => opt.MapFrom(src => src.Role.Description))
                .ForMember(
                    dest => dest.RoleIsActive,
                    opt => opt.MapFrom(src => src.Role.IsActive))
                .ForMember(
                    dest => dest.PermissionName,
                    opt => opt.MapFrom(src => src.Permission.Name))
                .ForMember(
                    dest => dest.PermissionCode,
                    opt => opt.MapFrom(src => src.Permission.Code))
                .ForMember(
                    dest => dest.PermissionModule,
                    opt => opt.MapFrom(src => src.Permission.Module))
                .ForMember(
                    dest => dest.PermissionDescription,
                    opt => opt.MapFrom(src => src.Permission.Description))
                .ForMember(
                    dest => dest.PermissionIsActive,
                    opt => opt.MapFrom(src => src.Permission.IsActive));

            #endregion

            #region RoleWorkSubTypePermission Mapping

            // RoleWorkSubTypePermission Mapping
            // ===================================================

            // Yeni rol-iş alt türü-yetki ataması oluşturma
            CreateMap<
                CreateRoleWorkSubTypePermissionDto,
                RoleWorkSubTypePermission>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Role,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkSubType,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Permission,
                    opt => opt.Ignore());

            // Mevcut rol-iş alt türü-yetki atamasını güncelleme
            CreateMap<
                UpdateRoleWorkSubTypePermissionDto,
                RoleWorkSubTypePermission>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Role,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkSubType,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Permission,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<
                RoleWorkSubTypePermission,
                UpdateRoleWorkSubTypePermissionDto>();

            // Rol-iş alt türü-yetki atamalarını listeleme
            CreateMap<
                RoleWorkSubTypePermission,
                ResultRoleWorkSubTypePermissionDto>()
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(
                        src => src.Role.Name))
                .ForMember(
                    dest => dest.RoleCode,
                    opt => opt.MapFrom(
                        src => src.Role.Code))
                .ForMember(
                    dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(
                        src => src.WorkSubType.Name))
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(
                        src => src.WorkSubType.WorkType.Name))
                .ForMember(
                    dest => dest.PermissionName,
                    opt => opt.MapFrom(
                        src => src.Permission.Name))
                .ForMember(
                    dest => dest.PermissionCode,
                    opt => opt.MapFrom(
                        src => src.Permission.Code))
                .ForMember(
                    dest => dest.PermissionModule,
                    opt => opt.MapFrom(
                        src => src.Permission.Module));

            // Rol-iş alt türü-yetki atamasının detayları
            CreateMap<
                RoleWorkSubTypePermission,
                GetByIdRoleWorkSubTypePermissionDto>()
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(
                        src => src.Role.Name))
                .ForMember(
                    dest => dest.RoleCode,
                    opt => opt.MapFrom(
                        src => src.Role.Code))
                .ForMember(
                    dest => dest.RoleDescription,
                    opt => opt.MapFrom(
                        src => src.Role.Description))
                .ForMember(
                    dest => dest.RoleIsActive,
                    opt => opt.MapFrom(
                        src => src.Role.IsActive))
                .ForMember(
                    dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(
                        src => src.WorkSubType.Name))
                .ForMember(
                    dest => dest.WorkSubTypeDescription,
                    opt => opt.MapFrom(
                        src => src.WorkSubType.Description))
                .ForMember(
                    dest => dest.WorkSubTypeIsActive,
                    opt => opt.MapFrom(
                        src => src.WorkSubType.IsActive))
                .ForMember(
                    dest => dest.WorkTypeId,
                    opt => opt.MapFrom(
                        src => src.WorkSubType.WorkTypeId))
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(
                        src => src.WorkSubType.WorkType.Name))
                .ForMember(
                    dest => dest.PermissionName,
                    opt => opt.MapFrom(
                        src => src.Permission.Name))
                .ForMember(
                    dest => dest.PermissionCode,
                    opt => opt.MapFrom(
                        src => src.Permission.Code))
                .ForMember(
                    dest => dest.PermissionModule,
                    opt => opt.MapFrom(
                        src => src.Permission.Module))
                .ForMember(
                    dest => dest.PermissionDescription,
                    opt => opt.MapFrom(
                        src => src.Permission.Description))
                .ForMember(
                    dest => dest.PermissionIsActive,
                    opt => opt.MapFrom(
                        src => src.Permission.IsActive));

            #endregion


            #region UserUnitRole Mapping
            // UserUnitRole Mapping
            // ===================================================

            // Yeni kullanıcı-rol ataması oluşturma
            CreateMap<CreateUserUnitRoleDto, UserUnitRole>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssignedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Role,
                    opt => opt.Ignore());

            // Mevcut kullanıcı-rol atamasını güncelleme
            CreateMap<UpdateUserUnitRoleDto, UserUnitRole>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssignedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Role,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<UserUnitRole, UpdateUserUnitRoleDto>();

            // Kullanıcı-rol atamalarını listeleme
            CreateMap<UserUnitRole, ResultUserUnitRoleDto>()
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src =>
                        src.User.FirstName + " " + src.User.LastName))
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src =>
                        src.User.Username))
                .ForMember(
                    dest => dest.UserIsActive,
                    opt => opt.MapFrom(src =>
                        src.User.IsActive))
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(src =>
                        src.Role.Name))
                .ForMember(
                    dest => dest.RoleCode,
                    opt => opt.MapFrom(src =>
                        src.Role.Code))
                .ForMember(
                    dest => dest.RoleIsActive,
                    opt => opt.MapFrom(src =>
                        src.Role.IsActive))
                .ForMember(
                    dest => dest.UnitId,
                    opt => opt.MapFrom(src =>
                        src.Role.UnitId))
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Role.Unit != null
                            ? src.Role.Unit.Name
                            : string.Empty));

            // Kullanıcı-rol ataması detayları
            CreateMap<UserUnitRole, GetByIdUserUnitRoleDto>()
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src =>
                        src.User.FirstName + " " + src.User.LastName))
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src =>
                        src.User.Username))
                .ForMember(
                    dest => dest.UserEmail,
                    opt => opt.MapFrom(src =>
                        src.User.Email))
                .ForMember(
                    dest => dest.UserTitle,
                    opt => opt.MapFrom(src =>
                        src.User.Title))
                .ForMember(
                    dest => dest.UserPrimaryUnitId,
                    opt => opt.MapFrom(src =>
                        src.User.PrimaryUnitId))
                .ForMember(
                    dest => dest.UserIsActive,
                    opt => opt.MapFrom(src =>
                        src.User.IsActive))
                .ForMember(
                    dest => dest.UserHasSystemAccess,
                    opt => opt.MapFrom(src =>
                        src.User.HasSystemAccess))
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(src =>
                        src.Role.Name))
                .ForMember(
                    dest => dest.RoleCode,
                    opt => opt.MapFrom(src =>
                        src.Role.Code))
                .ForMember(
                    dest => dest.RoleDescription,
                    opt => opt.MapFrom(src =>
                        src.Role.Description))
                .ForMember(
                    dest => dest.RoleIsActive,
                    opt => opt.MapFrom(src =>
                        src.Role.IsActive))
                .ForMember(
                    dest => dest.UnitId,
                    opt => opt.MapFrom(src =>
                        src.Role.UnitId))
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Role.Unit != null
                            ? src.Role.Unit.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.UnitIsActive,
                    opt => opt.MapFrom(src =>
                        src.Role.Unit != null &&
                        src.Role.Unit.IsActive));
            #endregion

            #region Team Mapping
            // Team Mapping
            // ===================================================

            // Yeni ekip oluşturma
            CreateMap<CreateTeamDto, Team>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Unit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.TeamMembers,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrders,
                    opt => opt.Ignore());

            // Mevcut ekip bilgilerini güncelleme
            CreateMap<UpdateTeamDto, Team>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Unit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.TeamMembers,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrders,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<Team, UpdateTeamDto>();

            // Ekipleri listeleme
            CreateMap<Team, ResultTeamDto>()
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit != null
                            ? src.Unit.Name
                            : string.Empty));

            // Ekip detayları
            CreateMap<Team, GetByIdTeamDto>()
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit != null
                            ? src.Unit.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.UnitIsActive,
                    opt => opt.MapFrom(src =>
                        src.Unit != null &&
                        src.Unit.IsActive))
                .ForMember(
                    dest => dest.TotalMemberCount,
                    opt => opt.MapFrom(src =>
                        src.TeamMembers.Count))
                .ForMember(
                    dest => dest.ActiveMemberCount,
                    opt => opt.MapFrom(src =>
                        src.TeamMembers.Count(x =>
                            x.IsActive &&
                            !x.LeftAt.HasValue)))
                .ForMember(
                    dest => dest.WorkOrderCount,
                    opt => opt.MapFrom(src =>
                        src.WorkOrders.Count));
            #endregion

            #region TeamMember Mapping


            // Yeni ekip üyeliği oluşturma
            CreateMap<CreateTeamMemberDto, TeamMember>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.JoinedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.LeftAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Team,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.User,
                    opt => opt.Ignore());

            // Mevcut ekip üyeliğini güncelleme
            CreateMap<UpdateTeamMemberDto, TeamMember>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.JoinedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.LeftAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Team,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.User,
                    opt => opt.Ignore());

            // Ekip üyeliği güncelleme formunu doldurma
            CreateMap<TeamMember, UpdateTeamMemberDto>();

            // Ekip üyelerini listeleme
            CreateMap<TeamMember, ResultTeamMemberDto>()
                .ForMember(
                    dest => dest.TeamName,
                    opt => opt.MapFrom(src =>
                        src.Team.Name))
                .ForMember(
                    dest => dest.TeamCode,
                    opt => opt.MapFrom(src =>
                        src.Team.Code))
                .ForMember(
                    dest => dest.UnitId,
                    opt => opt.MapFrom(src =>
                        src.Team.UnitId))
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Team.Unit != null
                            ? src.Team.Unit.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src =>
                        src.User.FirstName + " " + src.User.LastName))
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src =>
                        src.User.Username))
                .ForMember(
                    dest => dest.UserTitle,
                    opt => opt.MapFrom(src =>
                        src.User.Title))
                .ForMember(
                    dest => dest.UserIsActive,
                    opt => opt.MapFrom(src =>
                        src.User.IsActive))
                .ForMember(
                    dest => dest.TeamIsActive,
                    opt => opt.MapFrom(src =>
                        src.Team.IsActive));

            // Ekip üyeliği detaylarını görüntüleme
            CreateMap<TeamMember, GetByIdTeamMemberDto>()
                .ForMember(
                    dest => dest.TeamName,
                    opt => opt.MapFrom(src =>
                        src.Team.Name))
                .ForMember(
                    dest => dest.TeamCode,
                    opt => opt.MapFrom(src =>
                        src.Team.Code))
                .ForMember(
                    dest => dest.TeamDescription,
                    opt => opt.MapFrom(src =>
                        src.Team.Description))
                .ForMember(
                    dest => dest.TeamIsActive,
                    opt => opt.MapFrom(src =>
                        src.Team.IsActive))
                .ForMember(
                    dest => dest.UnitId,
                    opt => opt.MapFrom(src =>
                        src.Team.UnitId))
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Team.Unit != null
                            ? src.Team.Unit.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.UnitIsActive,
                    opt => opt.MapFrom(src =>
                        src.Team.Unit != null &&
                        src.Team.Unit.IsActive))
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src =>
                        src.User.FirstName + " " + src.User.LastName))
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src =>
                        src.User.Username))
                .ForMember(
                    dest => dest.UserEmail,
                    opt => opt.MapFrom(src =>
                        src.User.Email))
                .ForMember(
                    dest => dest.UserPhone,
                    opt => opt.MapFrom(src =>
                        src.User.Phone))
                .ForMember(
                    dest => dest.UserTitle,
                    opt => opt.MapFrom(src =>
                        src.User.Title))
                .ForMember(
                    dest => dest.UserPrimaryUnitId,
                    opt => opt.MapFrom(src =>
                        src.User.PrimaryUnitId))
                .ForMember(
                    dest => dest.UserIsActive,
                    opt => opt.MapFrom(src =>
                        src.User.IsActive))
                .ForMember(
                    dest => dest.UserHasSystemAccess,
                    opt => opt.MapFrom(src =>
                        src.User.HasSystemAccess));
            #endregion


            #region Brand Mapping

            // Yeni marka oluşturma
            CreateMap<CreateBrandDto, Brand>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Models,
                    opt => opt.Ignore());

            // Mevcut markayı güncelleme
            CreateMap<UpdateBrandDto, Brand>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Models,
                    opt => opt.Ignore());

            // Marka güncelleme formunu doldurma
            CreateMap<Brand, UpdateBrandDto>();

            // Markaları listeleme
            CreateMap<Brand, ResultBrandDto>();

            // Marka detaylarını görüntüleme
            CreateMap<Brand, GetByIdBrandDto>()
                .ForMember(
                    dest => dest.ModelCount,
                    opt => opt.MapFrom(src =>
                        src.Models.Count))
                .ForMember(
                    dest => dest.ActiveModelCount,
                    opt => opt.MapFrom(src =>
                        src.Models.Count(x => x.IsActive)))
                .ForMember(
                    dest => dest.InactiveModelCount,
                    opt => opt.MapFrom(src =>
                        src.Models.Count(x => !x.IsActive)));

            #endregion

            #region AssetModel Mapping

            // Yeni demirbaş modeli oluşturma
            CreateMap<CreateAssetModelDto, AssetModel>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Brand,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Assets,
                    opt => opt.Ignore());

            // Mevcut demirbaş modelini güncelleme
            CreateMap<UpdateAssetModelDto, AssetModel>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Brand,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Assets,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<AssetModel, UpdateAssetModelDto>();

            // Demirbaş modellerini listeleme
            CreateMap<AssetModel, ResultAssetModelDto>()
                .ForMember(
                    dest => dest.BrandName,
                    opt => opt.MapFrom(src =>
                        src.Brand != null
                            ? src.Brand.Name
                            : string.Empty));

            // Demirbaş modeli detaylarını görüntüleme
            CreateMap<AssetModel, GetByIdAssetModelDto>()
                .ForMember(
                    dest => dest.BrandName,
                    opt => opt.MapFrom(src =>
                        src.Brand != null
                            ? src.Brand.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.BrandIsActive,
                    opt => opt.MapFrom(src =>
                        src.Brand != null &&
                        src.Brand.IsActive))
                .ForMember(
                    dest => dest.AssetCount,
                    opt => opt.MapFrom(src =>
                        src.Assets.Count));

            #endregion

            #region AssetCategory Mapping

            // Yeni demirbaş kategorisi oluşturma
            CreateMap<CreateAssetCategoryDto, AssetCategory>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Kinds,
                    opt => opt.Ignore());

            // Mevcut demirbaş kategorisini güncelleme
            CreateMap<UpdateAssetCategoryDto, AssetCategory>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Kinds,
                    opt => opt.Ignore());

            // Kategori güncelleme formunu doldurma
            CreateMap<AssetCategory, UpdateAssetCategoryDto>();

            // Demirbaş kategorilerini listeleme
            CreateMap<AssetCategory, ResultAssetCategoryDto>();

            // Demirbaş kategorisi detaylarını görüntüleme
            CreateMap<AssetCategory, GetByIdAssetCategoryDto>()
                .ForMember(
                    dest => dest.KindCount,
                    opt => opt.MapFrom(src =>
                        src.Kinds.Count))
                .ForMember(
                    dest => dest.ActiveKindCount,
                    opt => opt.MapFrom(src =>
                        src.Kinds.Count(x => x.IsActive)))
                .ForMember(
                    dest => dest.InactiveKindCount,
                    opt => opt.MapFrom(src =>
                        src.Kinds.Count(x => !x.IsActive)));

            #endregion

            #region AssetKind Mapping

            // Yeni demirbaş türü oluşturma
            CreateMap<CreateAssetKindDto, AssetKind>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetCategory,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Assets,
                    opt => opt.Ignore());

            // Mevcut demirbaş türünü güncelleme
            CreateMap<UpdateAssetKindDto, AssetKind>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetCategory,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Assets,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<AssetKind, UpdateAssetKindDto>();

            // Demirbaş türlerini listeleme
            CreateMap<AssetKind, ResultAssetKindDto>()
                .ForMember(
                    dest => dest.AssetCategoryName,
                    opt => opt.MapFrom(src =>
                        src.AssetCategory != null
                            ? src.AssetCategory.Name
                            : string.Empty));

            // Demirbaş türü detaylarını görüntüleme
            CreateMap<AssetKind, GetByIdAssetKindDto>()
                .ForMember(
                    dest => dest.AssetCategoryName,
                    opt => opt.MapFrom(src =>
                        src.AssetCategory != null
                            ? src.AssetCategory.Name
                            : string.Empty))
                .ForMember(
                    dest => dest.AssetCategoryIsActive,
                    opt => opt.MapFrom(src =>
                        src.AssetCategory != null &&
                        src.AssetCategory.IsActive))
                .ForMember(
                    dest => dest.AssetCount,
                    opt => opt.MapFrom(src =>
                        src.Assets.Count));

            #endregion


            #region Asset Mapping

            // Yeni demirbaş oluşturma
            CreateMap<CreateAssetDto, Asset>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Status,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Unit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetModel,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetKind,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetUsers,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetMovements,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequests,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrders,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PlannedMaintenances,
                    opt => opt.Ignore());

            // Mevcut demirbaş bilgilerini güncelleme
            CreateMap<UpdateAssetDto, Asset>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Status,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Unit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetModel,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetKind,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetUsers,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetMovements,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequests,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrders,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PlannedMaintenances,
                    opt => opt.Ignore());

            // Demirbaş güncelleme formunu doldurma
            CreateMap<Asset, UpdateAssetDto>();

            // Demirbaşları listeleme
            CreateMap<Asset, ResultAssetDto>()
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit.Name))
                .ForMember(
                    dest => dest.AssetModelName,
                    opt => opt.MapFrom(src =>
                        src.AssetModel.Name))
                .ForMember(
                    dest => dest.BrandId,
                    opt => opt.MapFrom(src =>
                        src.AssetModel.BrandId))
                .ForMember(
                    dest => dest.BrandName,
                    opt => opt.MapFrom(src =>
                        src.AssetModel.Brand.Name))
                .ForMember(
                    dest => dest.AssetKindName,
                    opt => opt.MapFrom(src =>
                        src.AssetKind.Name))
                .ForMember(
                    dest => dest.AssetCategoryId,
                    opt => opt.MapFrom(src =>
                        src.AssetKind.AssetCategoryId))
                .ForMember(
                    dest => dest.AssetCategoryName,
                    opt => opt.MapFrom(src =>
                        src.AssetKind.AssetCategory.Name));

            // Demirbaş detaylarını görüntüleme
            CreateMap<Asset, GetByIdAssetDto>()
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit.Name))
                .ForMember(
                    dest => dest.UnitIsActive,
                    opt => opt.MapFrom(src =>
                        src.Unit.IsActive))
                .ForMember(
                    dest => dest.AssetModelName,
                    opt => opt.MapFrom(src =>
                        src.AssetModel.Name))
                .ForMember(
                    dest => dest.AssetModelIsActive,
                    opt => opt.MapFrom(src =>
                        src.AssetModel.IsActive))
                .ForMember(
                    dest => dest.BrandId,
                    opt => opt.MapFrom(src =>
                        src.AssetModel.BrandId))
                .ForMember(
                    dest => dest.BrandName,
                    opt => opt.MapFrom(src =>
                        src.AssetModel.Brand.Name))
                .ForMember(
                    dest => dest.BrandIsActive,
                    opt => opt.MapFrom(src =>
                        src.AssetModel.Brand.IsActive))
                .ForMember(
                    dest => dest.AssetKindName,
                    opt => opt.MapFrom(src =>
                        src.AssetKind.Name))
                .ForMember(
                    dest => dest.AssetKindIsActive,
                    opt => opt.MapFrom(src =>
                        src.AssetKind.IsActive))
                .ForMember(
                    dest => dest.AssetCategoryId,
                    opt => opt.MapFrom(src =>
                        src.AssetKind.AssetCategoryId))
                .ForMember(
                    dest => dest.AssetCategoryName,
                    opt => opt.MapFrom(src =>
                        src.AssetKind.AssetCategory.Name))
                .ForMember(
                    dest => dest.AssetCategoryIsActive,
                    opt => opt.MapFrom(src =>
                        src.AssetKind.AssetCategory.IsActive))
                .ForMember(
                    dest => dest.AssetUserCount,
                    opt => opt.MapFrom(src =>
                        src.AssetUsers.Count))
                .ForMember(
                    dest => dest.AssetMovementCount,
                    opt => opt.MapFrom(src =>
                        src.AssetMovements.Count))
                .ForMember(
                    dest => dest.WorkRequestCount,
                    opt => opt.MapFrom(src =>
                        src.WorkRequests.Count))
                .ForMember(
                    dest => dest.WorkOrderCount,
                    opt => opt.MapFrom(src =>
                        src.WorkOrders.Count))
                .ForMember(
                    dest => dest.PlannedMaintenanceCount,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenances.Count));

            #endregion


            #region AssetUser Mapping
            // Yeni demirbaş zimmet kaydı oluşturma
            CreateMap<CreateAssetUserDto, AssetUser>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssignedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.StartDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.EndDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Asset,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssignedByUser,
                    opt => opt.Ignore());

            // Mevcut zimmet kaydının açıklamasını güncelleme
            CreateMap<UpdateAssetUserDto, AssetUser>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssetId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssignedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.StartDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.EndDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Asset,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.AssignedByUser,
                    opt => opt.Ignore());

            // Zimmet güncelleme formunu doldurma
            CreateMap<AssetUser, UpdateAssetUserDto>();

            // Zimmet kayıtlarını listeleme
            CreateMap<AssetUser, ResultAssetUserDto>()
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset.AssetCode))
                .ForMember(
                    dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset.Name))
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src =>
                        src.User.FirstName + " " + src.User.LastName))
                .ForMember(
                    dest => dest.AssignedByFullName,
                    opt => opt.MapFrom(src =>
                        src.AssignedByUser != null
                            ? src.AssignedByUser.FirstName + " " +
                              src.AssignedByUser.LastName
                            : null));

            // Zimmet kaydının detaylarını görüntüleme
            CreateMap<AssetUser, GetByIdAssetUserDto>()
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset.AssetCode))
                .ForMember(
                    dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset.Name))
                .ForMember(
                    dest => dest.AssetSerialNumber,
                    opt => opt.MapFrom(src =>
                        src.Asset.SerialNumber))
                .ForMember(
                    dest => dest.AssetInventoryNumber,
                    opt => opt.MapFrom(src =>
                        src.Asset.InventoryNumber))
                .ForMember(
                    dest => dest.AssetStatus,
                    opt => opt.MapFrom(src =>
                        src.Asset.Status))
                .ForMember(
                    dest => dest.AssetModelName,
                    opt => opt.MapFrom(src =>
                        src.Asset.AssetModel.Name))
                .ForMember(
                    dest => dest.BrandName,
                    opt => opt.MapFrom(src =>
                        src.Asset.AssetModel.Brand.Name))
                .ForMember(
                    dest => dest.AssetUnitId,
                    opt => opt.MapFrom(src =>
                        src.Asset.UnitId))
                .ForMember(
                    dest => dest.AssetUnitName,
                    opt => opt.MapFrom(src =>
                        src.Asset.Unit.Name))
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src =>
                        src.User.FirstName + " " + src.User.LastName))
                .ForMember(
                    dest => dest.UserEmail,
                    opt => opt.MapFrom(src =>
                        src.User.Email))
                .ForMember(
                    dest => dest.UserTitle,
                    opt => opt.MapFrom(src =>
                        src.User.Title))
                .ForMember(
                    dest => dest.UserPrimaryUnitId,
                    opt => opt.MapFrom(src =>
                        src.User.PrimaryUnitId))
                .ForMember(
                    dest => dest.UserPrimaryUnitName,
                    opt => opt.MapFrom(src =>
                        src.User.PrimaryUnit.Name))
                .ForMember(
                    dest => dest.UserIsActive,
                    opt => opt.MapFrom(src =>
                        src.User.IsActive))
                .ForMember(
                    dest => dest.AssignedByFullName,
                    opt => opt.MapFrom(src =>
                        src.AssignedByUser != null
                            ? src.AssignedByUser.FirstName + " " +
                              src.AssignedByUser.LastName
                            : null))
                .ForMember(
                    dest => dest.AssignedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.AssignedByUser != null
                            ? src.AssignedByUser.Email
                            : null));


            #endregion

            #region AssetMovement Mapping

            // Yeni demirbaş hareketi oluşturma
            CreateMap<CreateAssetMovementDto, AssetMovement>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PerformedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.MovementDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Asset,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.FromUnit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ToUnit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.FromUser,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ToUser,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PerformedByUser,
                    opt => opt.Ignore());

            // Mevcut hareketin açıklamasını güncelleme
            CreateMap<UpdateAssetMovementDto, AssetMovement>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore());

            // Hareket güncelleme formunu doldurma
            CreateMap<AssetMovement, UpdateAssetMovementDto>();

            // Demirbaş hareketlerini listeleme
            CreateMap<AssetMovement, ResultAssetMovementDto>()
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset.AssetCode))
                .ForMember(
                    dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset.Name))
                .ForMember(
                    dest => dest.FromUnitName,
                    opt => opt.MapFrom(src =>
                        src.FromUnit != null
                            ? src.FromUnit.Name
                            : null))
                .ForMember(
                    dest => dest.ToUnitName,
                    opt => opt.MapFrom(src =>
                        src.ToUnit != null
                            ? src.ToUnit.Name
                            : null))
                .ForMember(
                    dest => dest.FromUserFullName,
                    opt => opt.MapFrom(src =>
                        src.FromUser != null
                            ? src.FromUser.FirstName + " " +
                              src.FromUser.LastName
                            : null))
                .ForMember(
                    dest => dest.ToUserFullName,
                    opt => opt.MapFrom(src =>
                        src.ToUser != null
                            ? src.ToUser.FirstName + " " +
                              src.ToUser.LastName
                            : null))
                .ForMember(
                    dest => dest.PerformedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.FirstName + " " +
                        src.PerformedByUser.LastName));

            // Demirbaş hareketinin detaylarını görüntüleme
            CreateMap<AssetMovement, GetByIdAssetMovementDto>()
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset.AssetCode))
                .ForMember(
                    dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset.Name))
                .ForMember(
                    dest => dest.AssetSerialNumber,
                    opt => opt.MapFrom(src =>
                        src.Asset.SerialNumber))
                .ForMember(
                    dest => dest.AssetInventoryNumber,
                    opt => opt.MapFrom(src =>
                        src.Asset.InventoryNumber))
                .ForMember(
                    dest => dest.FromUnitName,
                    opt => opt.MapFrom(src =>
                        src.FromUnit != null
                            ? src.FromUnit.Name
                            : null))
                .ForMember(
                    dest => dest.FromUnitCode,
                    opt => opt.MapFrom(src =>
                        src.FromUnit != null
                            ? src.FromUnit.Code
                            : null))
                .ForMember(
                    dest => dest.ToUnitName,
                    opt => opt.MapFrom(src =>
                        src.ToUnit != null
                            ? src.ToUnit.Name
                            : null))
                .ForMember(
                    dest => dest.ToUnitCode,
                    opt => opt.MapFrom(src =>
                        src.ToUnit != null
                            ? src.ToUnit.Code
                            : null))
                .ForMember(
                    dest => dest.FromUserFullName,
                    opt => opt.MapFrom(src =>
                        src.FromUser != null
                            ? src.FromUser.FirstName + " " +
                              src.FromUser.LastName
                            : null))
                .ForMember(
                    dest => dest.FromUsername,
                    opt => opt.MapFrom(src =>
                        src.FromUser != null
                            ? src.FromUser.Username
                            : null))
                .ForMember(
                    dest => dest.ToUserFullName,
                    opt => opt.MapFrom(src =>
                        src.ToUser != null
                            ? src.ToUser.FirstName + " " +
                              src.ToUser.LastName
                            : null))
                .ForMember(
                    dest => dest.ToUsername,
                    opt => opt.MapFrom(src =>
                        src.ToUser != null
                            ? src.ToUser.Username
                            : null))
                .ForMember(
                    dest => dest.PerformedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.FirstName + " " +
                        src.PerformedByUser.LastName))
                .ForMember(
                    dest => dest.PerformedByUsername,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.Username));

            #endregion

            #region WorkType Mapping

            // Yeni iş türü oluşturma
            CreateMap<CreateWorkTypeDto, WorkType>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkSubTypes,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UnitWorkTypes,
                    opt => opt.Ignore());

            // Mevcut iş türünü güncelleme
            CreateMap<UpdateWorkTypeDto, WorkType>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkSubTypes,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UnitWorkTypes,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<WorkType, UpdateWorkTypeDto>();

            // İş türlerini listeleme
            CreateMap<WorkType, ResultWorkTypeDto>();

            // İş türü detaylarını görüntüleme
            CreateMap<WorkType, GetByIdWorkTypeDto>()
                .ForMember(
                    dest => dest.WorkSubTypeCount,
                    opt => opt.MapFrom(src =>
                        src.WorkSubTypes.Count))
                .ForMember(
                    dest => dest.ActiveWorkSubTypeCount,
                    opt => opt.MapFrom(src =>
                        src.WorkSubTypes.Count(x => x.IsActive)))
                .ForMember(
                    dest => dest.InactiveWorkSubTypeCount,
                    opt => opt.MapFrom(src =>
                        src.WorkSubTypes.Count(x => !x.IsActive)))
                .ForMember(
                    dest => dest.UnitWorkTypeCount,
                    opt => opt.MapFrom(src =>
                        src.UnitWorkTypes.Count))
                .ForMember(
                    dest => dest.ActiveUnitWorkTypeCount,
                    opt => opt.MapFrom(src =>
                        src.UnitWorkTypes.Count(x => x.IsActive)))
                .ForMember(
                    dest => dest.InactiveUnitWorkTypeCount,
                    opt => opt.MapFrom(src =>
                        src.UnitWorkTypes.Count(x => !x.IsActive)));

            #endregion

            #region UnitWorkType Mapping


            // Yeni birim-iş türü ilişkisi oluşturma
            CreateMap<CreateUnitWorkTypeDto, UnitWorkType>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Unit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkType,
                    opt => opt.Ignore());

            // Mevcut birim-iş türü ilişkisini güncelleme
            CreateMap<UpdateUnitWorkTypeDto, UnitWorkType>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Unit,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkType,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<UnitWorkType, UpdateUnitWorkTypeDto>();

            // Birim-iş türü ilişkilerini listeleme
            CreateMap<UnitWorkType, ResultUnitWorkTypeDto>()
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit.Name))
                .ForMember(
                    dest => dest.UnitCode,
                    opt => opt.MapFrom(src =>
                        src.Unit.Code))
                .ForMember(
                    dest => dest.UnitIsActive,
                    opt => opt.MapFrom(src =>
                        src.Unit.IsActive))
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkType.Name))
                .ForMember(
                    dest => dest.WorkTypeIsActive,
                    opt => opt.MapFrom(src =>
                        src.WorkType.IsActive));

            // Birim-iş türü ilişkisi detaylarını görüntüleme
            CreateMap<UnitWorkType, GetByIdUnitWorkTypeDto>()
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit.Name))
                .ForMember(
                    dest => dest.UnitCode,
                    opt => opt.MapFrom(src =>
                        src.Unit.Code))
                .ForMember(
                    dest => dest.UnitIsActive,
                    opt => opt.MapFrom(src =>
                        src.Unit.IsActive))
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkType.Name))
                .ForMember(
                    dest => dest.WorkTypeDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkType.Description))
                .ForMember(
                    dest => dest.WorkTypeIsActive,
                    opt => opt.MapFrom(src =>
                        src.WorkType.IsActive));

            #endregion

            #region WorkSubType Mapping

            // Yeni iş alt türü oluşturma
            CreateMap<CreateWorkSubTypeDto, WorkSubType>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkType,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequests,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrders,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PlannedMaintenances,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.RolePermissions,
                    opt => opt.Ignore());

            // Mevcut iş alt türünü güncelleme
            CreateMap<UpdateWorkSubTypeDto, WorkSubType>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkType,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequests,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrders,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PlannedMaintenances,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.RolePermissions,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<WorkSubType, UpdateWorkSubTypeDto>();

            // İş alt türlerini listeleme
            CreateMap<WorkSubType, ResultWorkSubTypeDto>()
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkType.Name))
                .ForMember(
                    dest => dest.WorkTypeIsActive,
                    opt => opt.MapFrom(src =>
                        src.WorkType.IsActive));

            // İş alt türü detaylarını görüntüleme
            CreateMap<WorkSubType, GetByIdWorkSubTypeDto>()
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkType.Name))
                .ForMember(
                    dest => dest.WorkTypeDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkType.Description))
                .ForMember(
                    dest => dest.WorkTypeIsActive,
                    opt => opt.MapFrom(src =>
                        src.WorkType.IsActive))
                .ForMember(
                    dest => dest.WorkRequestCount,
                    opt => opt.MapFrom(src =>
                        src.WorkRequests.Count))
                .ForMember(
                    dest => dest.WorkOrderCount,
                    opt => opt.MapFrom(src =>
                        src.WorkOrders.Count))
                .ForMember(
                    dest => dest.PlannedMaintenanceCount,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenances.Count))
                .ForMember(
                    dest => dest.RolePermissionCount,
                    opt => opt.MapFrom(src =>
                        src.RolePermissions.Count))
                .ForMember(
                    dest => dest.ActiveRolePermissionCount,
                    opt => opt.MapFrom(src =>
                        src.RolePermissions.Count(x => x.IsActive)));

            #endregion

            #region SiteLocation Mapping

            // Yeni saha / lokasyon oluşturma
            CreateMap<CreateSiteLocationDto, SiteLocation>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequests,
                    opt => opt.Ignore());

            // Mevcut saha / lokasyon bilgilerini güncelleme
            CreateMap<UpdateSiteLocationDto, SiteLocation>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequests,
                    opt => opt.Ignore());

            // Güncelleme formunu doldurma
            CreateMap<SiteLocation, UpdateSiteLocationDto>();

            // Lokasyonları listeleme
            CreateMap<SiteLocation, ResultSiteLocationDto>();

            // Lokasyon detaylarını görüntüleme
            CreateMap<SiteLocation, GetByIdSiteLocationDto>()
                .ForMember(
                    dest => dest.WorkRequestCount,
                    opt => opt.MapFrom(src =>
                        src.WorkRequests.Count));

            #endregion

            #region WorkRequest Mapping

            // CREATE
            CreateMap<CreateWorkRequestDto, WorkRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RequestNumber, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ManagerUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.RequestDate, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.RequesterUnit, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsibleUnit, opt => opt.Ignore())
                .ForMember(dest => dest.ManagerUser, opt => opt.Ignore())
                .ForMember(dest => dest.WorkSubType, opt => opt.Ignore())
                .ForMember(dest => dest.SiteLocation, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
                .ForMember(dest => dest.Files, opt => opt.Ignore())
                .ForMember(dest => dest.Actions, opt => opt.Ignore());


            // UPDATE
            CreateMap<UpdateWorkRequestDto, WorkRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RequestNumber, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ManagerUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.RequestDate, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.RequesterUnit, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsibleUnit, opt => opt.Ignore())
                .ForMember(dest => dest.ManagerUser, opt => opt.Ignore())
                .ForMember(dest => dest.WorkSubType, opt => opt.Ignore())
                .ForMember(dest => dest.SiteLocation, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
                .ForMember(dest => dest.Files, opt => opt.Ignore())
                .ForMember(dest => dest.Actions, opt => opt.Ignore());


            // ENTITY -> UPDATE DTO
            CreateMap<WorkRequest, UpdateWorkRequestDto>();


            // RESULT
            CreateMap<WorkRequest, ResultWorkRequestDto>()
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset != null ? src.Asset.AssetCode : null))
                .ForMember(
                    dest => dest.RequestedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.RequestedByUser.FirstName + " " +
                        src.RequestedByUser.LastName))
                .ForMember(
                    dest => dest.RequesterUnitName,
                    opt => opt.MapFrom(src =>
                        src.RequesterUnit.Name))
                .ForMember(
                    dest => dest.ResponsibleUnitName,
                    opt => opt.MapFrom(src =>
                        src.ResponsibleUnit.Name))
                .ForMember(
                    dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.Name))
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkType.Name))
                .ForMember(
                    dest => dest.ManagerUserFullName,
                    opt => opt.MapFrom(src =>
                        src.ManagerUser != null
                            ? src.ManagerUser.FirstName + " " +
                              src.ManagerUser.LastName
                            : null))
                .ForMember(
                    dest => dest.SiteLocationName,
                    opt => opt.MapFrom(src =>
                        src.SiteLocation != null
                            ? src.SiteLocation.Name
                            : null));


            // GET BY ID
            CreateMap<WorkRequest, GetByIdWorkRequestDto>()
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset != null ? src.Asset.AssetCode : null))
                .ForMember(
                    dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset != null ? src.Asset.Name : null))
                .ForMember(
                    dest => dest.RequestedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.RequestedByUser.FirstName + " " +
                        src.RequestedByUser.LastName))
                .ForMember(
                    dest => dest.RequestedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.RequestedByUser.Email))
                .ForMember(
                    dest => dest.RequestedByUserIsActive,
                    opt => opt.MapFrom(src =>
                        src.RequestedByUser.IsActive))
                .ForMember(
                    dest => dest.RequesterUnitName,
                    opt => opt.MapFrom(src =>
                        src.RequesterUnit.Name))
                .ForMember(
                    dest => dest.RequesterUnitCode,
                    opt => opt.MapFrom(src =>
                        src.RequesterUnit.Code))
                .ForMember(
                    dest => dest.ResponsibleUnitName,
                    opt => opt.MapFrom(src =>
                        src.ResponsibleUnit.Name))
                .ForMember(
                    dest => dest.ResponsibleUnitCode,
                    opt => opt.MapFrom(src =>
                        src.ResponsibleUnit.Code))
                .ForMember(
                    dest => dest.WorkTypeId,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkTypeId))
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkType.Name))
                .ForMember(
                    dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.Name))
                .ForMember(
                    dest => dest.WorkSubTypeDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.Description))
                .ForMember(
                    dest => dest.WorkSubTypeIsActive,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.IsActive))
                .ForMember(
                    dest => dest.ManagerUserFullName,
                    opt => opt.MapFrom(src =>
                        src.ManagerUser != null
                            ? src.ManagerUser.FirstName + " " +
                              src.ManagerUser.LastName
                            : null))
                .ForMember(
                    dest => dest.ManagerUserEmail,
                    opt => opt.MapFrom(src =>
                        src.ManagerUser != null
                            ? src.ManagerUser.Email
                            : null))
                .ForMember(
                    dest => dest.SiteLocationName,
                    opt => opt.MapFrom(src =>
                        src.SiteLocation != null
                            ? src.SiteLocation.Name
                            : null))
                .ForMember(
                    dest => dest.SiteLocationAddress,
                    opt => opt.MapFrom(src =>
                        src.SiteLocation != null
                            ? src.SiteLocation.Address
                            : null))
                .ForMember(
                    dest => dest.WorkOrderId,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder != null
                            ? (int?)src.WorkOrder.Id
                            : null))
                .ForMember(
                    dest => dest.FileCount,
                    opt => opt.MapFrom(src =>
                        src.Files.Count))
                .ForMember(
                    dest => dest.ActionCount,
                    opt => opt.MapFrom(src =>
                        src.Actions.Count));

            #endregion

            #region WorkRequestFile Mapping

            // CREATE
            CreateMap<CreateWorkRequestFileDto, WorkRequestFile>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UploadedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequest,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UploadedByUser,
                    opt => opt.Ignore());


            // UPDATE
            CreateMap<UpdateWorkRequestFileDto, WorkRequestFile>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequestId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.FilePath,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ContentType,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.FileSizeBytes,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UploadedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkRequest,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UploadedByUser,
                    opt => opt.Ignore());


            // ENTITY -> UPDATE DTO
            CreateMap<WorkRequestFile, UpdateWorkRequestFileDto>();


            // RESULT
            CreateMap<WorkRequestFile, ResultWorkRequestFileDto>()
                .ForMember(
                    dest => dest.WorkRequestNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkRequest.RequestNumber))
                .ForMember(
                    dest => dest.WorkRequestTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkRequest.Title))
                .ForMember(
                    dest => dest.UploadedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.UploadedByUser.FirstName + " " +
                        src.UploadedByUser.LastName));


            // GET BY ID
            CreateMap<WorkRequestFile, GetByIdWorkRequestFileDto>()
                .ForMember(
                    dest => dest.WorkRequestNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkRequest.RequestNumber))
                .ForMember(
                    dest => dest.WorkRequestTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkRequest.Title))
                .ForMember(
                    dest => dest.WorkRequestDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkRequest.Description))
                .ForMember(
                    dest => dest.UploadedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.UploadedByUser.FirstName + " " +
                        src.UploadedByUser.LastName))
                .ForMember(
                    dest => dest.UploadedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.UploadedByUser.Email))
                .ForMember(
                    dest => dest.UploadedByUserIsActive,
                    opt => opt.MapFrom(src =>
                        src.UploadedByUser.IsActive));

            #endregion

            #region WorkRequestAction Mapping

            CreateMap<CreateWorkRequestActionDto, WorkRequestAction>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.PerformedByUserId, opt => opt.Ignore())
    .ForMember(dest => dest.PerformedAt, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
    .ForMember(dest => dest.WorkRequest, opt => opt.Ignore())
    .ForMember(dest => dest.PerformedByUser, opt => opt.Ignore());

            CreateMap<UpdateWorkRequestActionDto, WorkRequestAction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkRequestId, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ActionType, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.WorkRequest, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedByUser, opt => opt.Ignore());

            CreateMap<WorkRequestAction, UpdateWorkRequestActionDto>();

            CreateMap<WorkRequestAction, ResultWorkRequestActionDto>()
                .ForMember(dest => dest.WorkRequestNumber,
                    opt => opt.MapFrom(src => src.WorkRequest.RequestNumber))
                .ForMember(dest => dest.WorkRequestTitle,
                    opt => opt.MapFrom(src => src.WorkRequest.Title))
                .ForMember(dest => dest.PerformedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.FirstName + " " +
                        src.PerformedByUser.LastName));

            CreateMap<WorkRequestAction, GetByIdWorkRequestActionDto>()
                .ForMember(dest => dest.WorkRequestNumber,
                    opt => opt.MapFrom(src => src.WorkRequest.RequestNumber))
                .ForMember(dest => dest.WorkRequestTitle,
                    opt => opt.MapFrom(src => src.WorkRequest.Title))
                .ForMember(dest => dest.WorkRequestDescription,
                    opt => opt.MapFrom(src => src.WorkRequest.Description))
                .ForMember(dest => dest.WorkRequestDate,
                    opt => opt.MapFrom(src => src.WorkRequest.RequestDate))
                .ForMember(dest => dest.PerformedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.FirstName + " " +
                        src.PerformedByUser.LastName))
                .ForMember(dest => dest.PerformedByUserEmail,
                    opt => opt.MapFrom(src => src.PerformedByUser.Email))
                .ForMember(dest => dest.PerformedByUserTitle,
                    opt => opt.MapFrom(src => src.PerformedByUser.Title))
                .ForMember(dest => dest.PerformedByUserIsActive,
                    opt => opt.MapFrom(src => src.PerformedByUser.IsActive));

            #endregion

            #region WorkOrders Mapping

            // CREATE
            CreateMap<CreateWorkOrderDto, WorkOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedForApprovalAt, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledAt, opt => opt.Ignore())
                .ForMember(dest => dest.ResultDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.WorkRequest, opt => opt.Ignore())
                .ForMember(dest => dest.PlannedMaintenance, opt => opt.Ignore())
                .ForMember(dest => dest.WorkSubType, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsibleUnit, opt => opt.Ignore())
                .ForMember(dest => dest.Team, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.Personnel, opt => opt.Ignore())
                .ForMember(dest => dest.Materials, opt => opt.Ignore())
                .ForMember(dest => dest.Actions, opt => opt.Ignore())
                .ForMember(dest => dest.StatusHistory, opt => opt.Ignore())
                .ForMember(dest => dest.Files, opt => opt.Ignore())
                .ForMember(dest => dest.ChecklistResults, opt => opt.Ignore());


            // UPDATE
            CreateMap<UpdateWorkOrderDto, WorkOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedForApprovalAt, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.WorkRequest, opt => opt.Ignore())
                .ForMember(dest => dest.PlannedMaintenance, opt => opt.Ignore())
                .ForMember(dest => dest.WorkSubType, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsibleUnit, opt => opt.Ignore())
                .ForMember(dest => dest.Team, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.Personnel, opt => opt.Ignore())
                .ForMember(dest => dest.Materials, opt => opt.Ignore())
                .ForMember(dest => dest.Actions, opt => opt.Ignore())
                .ForMember(dest => dest.StatusHistory, opt => opt.Ignore())
                .ForMember(dest => dest.Files, opt => opt.Ignore())
                .ForMember(dest => dest.ChecklistResults, opt => opt.Ignore());


            // ENTITY -> UPDATE DTO
            CreateMap<WorkOrder, UpdateWorkOrderDto>();


            // RESULT
            CreateMap<WorkOrder, ResultWorkOrderDto>()
                .ForMember(dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset != null ? src.Asset.AssetCode : null))
                .ForMember(dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset != null ? src.Asset.Name : null))
                .ForMember(dest => dest.WorkRequestNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkRequest != null
                            ? src.WorkRequest.RequestNumber
                            : null))
                .ForMember(dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(src => src.WorkSubType.Name))
                .ForMember(dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkType.Name))
                .ForMember(dest => dest.ResponsibleUnitName,
                    opt => opt.MapFrom(src => src.ResponsibleUnit.Name))
                .ForMember(dest => dest.TeamName,
                    opt => opt.MapFrom(src => src.Team.Name))
                .ForMember(dest => dest.CreatedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByUser.FirstName + " " +
                        src.CreatedByUser.LastName))
                .ForMember(dest => dest.ApprovedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.ApprovedByUser != null
                            ? src.ApprovedByUser.FirstName + " " +
                              src.ApprovedByUser.LastName
                            : null));


            // GET BY ID
            CreateMap<WorkOrder, GetByIdWorkOrderDto>()
                .ForMember(dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset != null ? src.Asset.AssetCode : null))
                .ForMember(dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset != null ? src.Asset.Name : null))
                .ForMember(dest => dest.WorkRequestNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkRequest != null
                            ? src.WorkRequest.RequestNumber
                            : null))
                .ForMember(dest => dest.WorkRequestTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkRequest != null
                            ? src.WorkRequest.Title
                            : null))
                .ForMember(dest => dest.WorkTypeId,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkTypeId))
                .ForMember(dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkType.Name))
                .ForMember(dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(src => src.WorkSubType.Name))
                .ForMember(dest => dest.WorkSubTypeDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.Description))
                .ForMember(dest => dest.ResponsibleUnitName,
                    opt => opt.MapFrom(src => src.ResponsibleUnit.Name))
                .ForMember(dest => dest.ResponsibleUnitCode,
                    opt => opt.MapFrom(src => src.ResponsibleUnit.Code))
                .ForMember(dest => dest.TeamName,
                    opt => opt.MapFrom(src => src.Team.Name))
                .ForMember(dest => dest.CreatedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByUser.FirstName + " " +
                        src.CreatedByUser.LastName))
                .ForMember(dest => dest.CreatedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.CreatedByUser.Email))
                .ForMember(dest => dest.ApprovedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.ApprovedByUser != null
                            ? src.ApprovedByUser.FirstName + " " +
                              src.ApprovedByUser.LastName
                            : null))
                .ForMember(dest => dest.ApprovedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.ApprovedByUser != null
                            ? src.ApprovedByUser.Email
                            : null))
                .ForMember(dest => dest.PersonnelCount,
                    opt => opt.MapFrom(src => src.Personnel.Count))
                .ForMember(dest => dest.MaterialCount,
                    opt => opt.MapFrom(src => src.Materials.Count))
                .ForMember(dest => dest.ActionCount,
                    opt => opt.MapFrom(src => src.Actions.Count))
                .ForMember(dest => dest.StatusHistoryCount,
                    opt => opt.MapFrom(src => src.StatusHistory.Count))
                .ForMember(dest => dest.FileCount,
                    opt => opt.MapFrom(src => src.Files.Count))
                .ForMember(dest => dest.ChecklistResultCount,
                    opt => opt.MapFrom(src => src.ChecklistResults.Count));

            #endregion


            #region WorkOrderPersonnel Mapping

            // CREATE
            CreateMap<CreateWorkOrderPersonnelDto, WorkOrderPersonnel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ReleasedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
                .ForMember(dest => dest.TeamMember, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByUser, opt => opt.Ignore());


            // UPDATE
            CreateMap<UpdateWorkOrderPersonnelDto, WorkOrderPersonnel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.TeamMemberId, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ReleasedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
                .ForMember(dest => dest.TeamMember, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByUser, opt => opt.Ignore());


            // ENTITY -> UPDATE DTO
            CreateMap<WorkOrderPersonnel, UpdateWorkOrderPersonnelDto>();


            // RESULT
            CreateMap<WorkOrderPersonnel, ResultWorkOrderPersonnelDto>()
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))
                .ForMember(
                    dest => dest.TeamId,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.TeamId))
                .ForMember(
                    dest => dest.TeamName,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.Team.Name))
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.UserId))
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.User.FirstName + " " +
                        src.TeamMember.User.LastName))
                .ForMember(
                    dest => dest.AssignedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.AssignedByUser.FirstName + " " +
                        src.AssignedByUser.LastName));


            // GET BY ID
            CreateMap<WorkOrderPersonnel, GetByIdWorkOrderPersonnelDto>()
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))
                .ForMember(
                    dest => dest.TeamId,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.TeamId))
                .ForMember(
                    dest => dest.TeamName,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.Team.Name))
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.UserId))
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.User.FirstName + " " +
                        src.TeamMember.User.LastName))
                .ForMember(
                    dest => dest.UserEmail,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.User.Email))
                .ForMember(
                    dest => dest.UserTitle,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.User.Title))
                .ForMember(
                    dest => dest.UserIsActive,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.User.IsActive))
                .ForMember(
                    dest => dest.TeamMemberIsActive,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.IsActive))
                .ForMember(
                    dest => dest.TeamMemberJoinedAt,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.JoinedAt))
                .ForMember(
                    dest => dest.TeamMemberLeftAt,
                    opt => opt.MapFrom(src =>
                        src.TeamMember.LeftAt))
                .ForMember(
                    dest => dest.AssignedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.AssignedByUser.FirstName + " " +
                        src.AssignedByUser.LastName))
                .ForMember(
                    dest => dest.AssignedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.AssignedByUser.Email));

            #endregion

            #region WorkOrderAction Mapping

            // CREATE
            // Yeni iş emri işlem kaydı oluşturma
            // ===================================================

            CreateMap<CreateWorkOrderActionDto, WorkOrderAction>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PerformedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PerformedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrder,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PerformedByUser,
                    opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut iş emri işlem kaydını güncelleme
            // ===================================================

            CreateMap<UpdateWorkOrderActionDto, WorkOrderAction>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrderId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PerformedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PerformedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrder,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PerformedByUser,
                    opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<WorkOrderAction, UpdateWorkOrderActionDto>();


            // ===================================================
            // RESULT
            // İş emri işlem geçmişini listeleme
            // ===================================================

            CreateMap<WorkOrderAction, ResultWorkOrderActionDto>()

                // İş emri numarası
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))

                // İş emri başlığı
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))

                // İşlemi gerçekleştiren kullanıcı
                .ForMember(
                    dest => dest.PerformedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.FirstName + " " +
                        src.PerformedByUser.LastName));


            // ===================================================
            // GET BY ID
            // İş emri işlem geçmişi detayları
            // ===================================================

            CreateMap<WorkOrderAction, GetByIdWorkOrderActionDto>()

                // İş emri numarası
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))

                // İş emri başlığı
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))

                // İş emri açıklaması
                .ForMember(
                    dest => dest.WorkOrderDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Description))

                // İşlemi gerçekleştiren kullanıcı adı
                .ForMember(
                    dest => dest.PerformedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.FirstName + " " +
                        src.PerformedByUser.LastName))

                // İşlemi gerçekleştiren kullanıcı e-postası
                .ForMember(
                    dest => dest.PerformedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.Email))

                // İşlemi gerçekleştiren kullanıcı unvanı
                .ForMember(
                    dest => dest.PerformedByUserTitle,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.Title))

                // İşlemi gerçekleştiren kullanıcı durumu
                .ForMember(
                    dest => dest.PerformedByUserIsActive,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.IsActive));


            #endregion


            #region WorkOrderStatusHistory Mapping

            // CREATE
            // Yeni durum geçmişi kaydı oluşturma
            // ===================================================

            CreateMap<CreateWorkOrderStatusHistoryDto, WorkOrderStatusHistory>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PreviousStatus,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ChangedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ChangedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrder,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ChangedByUser,
                    opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Durum geçmişi açıklamasını güncelleme
            // ===================================================

            CreateMap<UpdateWorkOrderStatusHistoryDto, WorkOrderStatusHistory>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrderId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PreviousStatus,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.NewStatus,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ChangedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ChangedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrder,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ChangedByUser,
                    opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // ===================================================

            CreateMap<WorkOrderStatusHistory, UpdateWorkOrderStatusHistoryDto>();


            // ===================================================
            // RESULT
            // Durum geçmişi kayıtlarını listeleme
            // ===================================================

            CreateMap<WorkOrderStatusHistory, ResultWorkOrderStatusHistoryDto>()
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))
                .ForMember(
                    dest => dest.ChangedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.ChangedByUser.FirstName + " " +
                        src.ChangedByUser.LastName));


            // ===================================================
            // GET BY ID
            // Durum geçmişi detaylarını görüntüleme
            // ===================================================

            CreateMap<WorkOrderStatusHistory, GetByIdWorkOrderStatusHistoryDto>()
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))
                .ForMember(
                    dest => dest.WorkOrderDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Description))
                .ForMember(
                    dest => dest.WorkOrderCurrentStatus,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Status))
                .ForMember(
                    dest => dest.ChangedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.ChangedByUser.FirstName + " " +
                        src.ChangedByUser.LastName))
                .ForMember(
                    dest => dest.ChangedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.ChangedByUser.Email))
                .ForMember(
                    dest => dest.ChangedByUserTitle,
                    opt => opt.MapFrom(src =>
                        src.ChangedByUser.Title))
                .ForMember(
                    dest => dest.ChangedByUserIsActive,
                    opt => opt.MapFrom(src =>
                        src.ChangedByUser.IsActive));


            #endregion

            #region WorkOrderFiles Mapping

            // CREATE
            // Yeni iş emri dosya kaydı oluşturma
            // ===================================================

            CreateMap<CreateWorkOrderFileDto, WorkOrderFile>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UploadedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrder,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UploadedByUser,
                    opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut iş emri dosyasının adını güncelleme
            // ===================================================

            CreateMap<UpdateWorkOrderFileDto, WorkOrderFile>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrderId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.FilePath,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ContentType,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.FileSizeBytes,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UploadedByUserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.WorkOrder,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UploadedByUser,
                    opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<WorkOrderFile, UpdateWorkOrderFileDto>();


            // ===================================================
            // RESULT
            // İş emri dosyalarını listeleme
            // ===================================================

            CreateMap<WorkOrderFile, ResultWorkOrderFileDto>()
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))
                .ForMember(
                    dest => dest.UploadedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.UploadedByUser.FirstName + " " +
                        src.UploadedByUser.LastName));


            // ===================================================
            // GET BY ID
            // İş emri dosyası detaylarını görüntüleme
            // ===================================================

            CreateMap<WorkOrderFile, GetByIdWorkOrderFileDto>()
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))
                .ForMember(
                    dest => dest.WorkOrderDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Description))
                .ForMember(
                    dest => dest.UploadedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.UploadedByUser.FirstName + " " +
                        src.UploadedByUser.LastName))
                .ForMember(
                    dest => dest.UploadedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.UploadedByUser.Email))
                .ForMember(
                    dest => dest.UploadedByUserIsActive,
                    opt => opt.MapFrom(src =>
                        src.UploadedByUser.IsActive));

            #endregion

            #region PlannedMaintenances Mapping

            // CREATE
            // Yeni planlı bakım oluşturma
            // ===================================================

            CreateMap<CreatePlannedMaintenanceDto, PlannedMaintenance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.WorkSubType, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrders, opt => opt.Ignore())
                .ForMember(dest => dest.Checklists, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut planlı bakım bilgilerini güncelleme
            // ===================================================

            CreateMap<UpdatePlannedMaintenanceDto, PlannedMaintenance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.WorkSubType, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrders, opt => opt.Ignore())
                .ForMember(dest => dest.Checklists, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<PlannedMaintenance, UpdatePlannedMaintenanceDto>();


            // ===================================================
            // RESULT
            // Planlı bakım kayıtlarını listeleme
            // ===================================================

            CreateMap<PlannedMaintenance, ResultPlannedMaintenanceDto>()

                // Demirbaş kodu
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset.AssetCode))

                // Demirbaş adı
                .ForMember(
                    dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset.Name))

                // Sorumlu birim adı
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit.Name))

                // İş alt türü adı
                .ForMember(
                    dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.Name))

                // Ana iş türü adı
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkType.Name))

                // Planı oluşturan kullanıcı
                .ForMember(
                    dest => dest.CreatedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByUser != null
                            ? src.CreatedByUser.FirstName + " " +
                              src.CreatedByUser.LastName
                            : null));


            // ===================================================
            // GET BY ID
            // Planlı bakım detaylarını görüntüleme
            // ===================================================

            CreateMap<PlannedMaintenance, GetByIdPlannedMaintenanceDto>()

                // Demirbaş kodu
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.Asset.AssetCode))

                // Demirbaş adı
                .ForMember(
                    dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.Asset.Name))

                // Birim adı
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit.Name))

                // Birim kodu
                .ForMember(
                    dest => dest.UnitCode,
                    opt => opt.MapFrom(src =>
                        src.Unit.Code))

                // İş alt türü adı
                .ForMember(
                    dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.Name))

                // İş alt türü açıklaması
                .ForMember(
                    dest => dest.WorkSubTypeDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.Description))

                // Ana iş türü ID
                .ForMember(
                    dest => dest.WorkTypeId,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkTypeId))

                // Ana iş türü adı
                .ForMember(
                    dest => dest.WorkTypeName,
                    opt => opt.MapFrom(src =>
                        src.WorkSubType.WorkType.Name))

                // Oluşturan kullanıcının adı
                .ForMember(
                    dest => dest.CreatedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByUser != null
                            ? src.CreatedByUser.FirstName + " " +
                              src.CreatedByUser.LastName
                            : null))

                // Oluşturan kullanıcının e-posta adresi
                .ForMember(
                    dest => dest.CreatedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.CreatedByUser != null
                            ? src.CreatedByUser.Email
                            : null))

                // Oluşturan kullanıcının unvanı
                .ForMember(
                    dest => dest.CreatedByUserTitle,
                    opt => opt.MapFrom(src =>
                        src.CreatedByUser != null
                            ? src.CreatedByUser.Title
                            : null))

                // Bu bakım planına bağlı iş emri sayısı
                .ForMember(
                    dest => dest.WorkOrderCount,
                    opt => opt.MapFrom(src =>
                        src.WorkOrders.Count))

                // Bakım kontrol listesi sayısı
                .ForMember(
                    dest => dest.ChecklistCount,
                    opt => opt.MapFrom(src =>
                        src.Checklists.Count));

            #endregion


            #region MaintenanceChecklist Mapping
            // CREATE
            // Yeni bakım kontrol maddesi oluşturma
            // ===================================================

            CreateMap<CreateMaintenanceChecklistDto, MaintenanceChecklist>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PlannedMaintenance, opt => opt.Ignore())
                .ForMember(dest => dest.Results, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Bakım kontrol maddesini güncelleme
            // ===================================================

            CreateMap<UpdateMaintenanceChecklistDto, MaintenanceChecklist>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PlannedMaintenanceId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PlannedMaintenance, opt => opt.Ignore())
                .ForMember(dest => dest.Results, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<MaintenanceChecklist, UpdateMaintenanceChecklistDto>();


            // ===================================================
            // RESULT
            // Bakım kontrol maddelerini listeleme
            // ===================================================

            CreateMap<MaintenanceChecklist, ResultMaintenanceChecklistDto>()
                .ForMember(
                    dest => dest.PlannedMaintenanceName,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.Name));


            // ===================================================
            // GET BY ID
            // Bakım kontrol maddesinin detaylarını getirme
            // ===================================================

            CreateMap<MaintenanceChecklist, GetByIdMaintenanceChecklistDto>()

                // Bakım planı adı
                .ForMember(
                    dest => dest.PlannedMaintenanceName,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.Name))

                // Bakım planı açıklaması
                .ForMember(
                    dest => dest.PlannedMaintenanceDescription,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.Description))

                // Demirbaş ID
                .ForMember(
                    dest => dest.AssetId,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.AssetId))

                // Demirbaş kodu
                .ForMember(
                    dest => dest.AssetCode,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.Asset.AssetCode))

                // Demirbaş adı
                .ForMember(
                    dest => dest.AssetName,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.Asset.Name))

                // Sorumlu birim ID
                .ForMember(
                    dest => dest.UnitId,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.UnitId))

                // Sorumlu birim adı
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.Unit.Name))

                // İş alt türü ID
                .ForMember(
                    dest => dest.WorkSubTypeId,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.WorkSubTypeId))

                // İş alt türü adı
                .ForMember(
                    dest => dest.WorkSubTypeName,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.WorkSubType.Name))

                // Bakım periyodu
                .ForMember(
                    dest => dest.PeriodDays,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.PeriodDays))

                // Sonraki bakım tarihi
                .ForMember(
                    dest => dest.NextMaintenanceDate,
                    opt => opt.MapFrom(src =>
                        src.PlannedMaintenance.NextMaintenanceDate))

                // Kontrol sonuçlarının sayısı
                .ForMember(
                    dest => dest.ResultCount,
                    opt => opt.MapFrom(src =>
                        src.Results.Count));


            #endregion

            #region WorkOrderChecklistResult Mapping


            // CREATE
            // Yeni iş emri kontrol sonucu oluşturma
            // ===================================================

            CreateMap<CreateWorkOrderChecklistResultDto, WorkOrderChecklistResult>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsCompleted, opt => opt.Ignore())
                .ForMember(dest => dest.IsSuccessful, opt => opt.Ignore())
                .ForMember(dest => dest.CheckedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CheckedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
                .ForMember(dest => dest.MaintenanceChecklist, opt => opt.Ignore())
                .ForMember(dest => dest.CheckedByUser, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut iş emri kontrol sonucunu güncelleme
            // ===================================================

            CreateMap<UpdateWorkOrderChecklistResultDto, WorkOrderChecklistResult>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.MaintenanceChecklistId, opt => opt.Ignore())
                .ForMember(dest => dest.CheckedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CheckedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
                .ForMember(dest => dest.MaintenanceChecklist, opt => opt.Ignore())
                .ForMember(dest => dest.CheckedByUser, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<WorkOrderChecklistResult, UpdateWorkOrderChecklistResultDto>();


            // ===================================================
            // RESULT
            // İş emri kontrol sonuçlarını listeleme
            // ===================================================

            CreateMap<WorkOrderChecklistResult, ResultWorkOrderChecklistResultDto>()

                // İş emri numarası
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))

                // İş emri başlığı
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))

                // Kontrol maddesi başlığı
                .ForMember(
                    dest => dest.MaintenanceChecklistTitle,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.Title))

                // Kontrol maddesi sıra numarası
                .ForMember(
                    dest => dest.MaintenanceChecklistSequenceNumber,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.SequenceNumber))

                // Kontrol maddesinin zorunluluk durumu
                .ForMember(
                    dest => dest.MaintenanceChecklistIsRequired,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.IsRequired))

                // Kontrolü yapan kullanıcı
                .ForMember(
                    dest => dest.CheckedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.CheckedByUser != null
                            ? src.CheckedByUser.FirstName + " " +
                              src.CheckedByUser.LastName
                            : null));


            // ===================================================
            // GET BY ID
            // İş emri kontrol sonucu detaylarını görüntüleme
            // ===================================================

            CreateMap<WorkOrderChecklistResult, GetByIdWorkOrderChecklistResultDto>()

                // İş emri numarası
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))

                // İş emri başlığı
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))

                // İş emri açıklaması
                .ForMember(
                    dest => dest.WorkOrderDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Description))

                // Kontrol maddesi başlığı
                .ForMember(
                    dest => dest.MaintenanceChecklistTitle,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.Title))

                // Kontrol maddesi açıklaması
                .ForMember(
                    dest => dest.MaintenanceChecklistDescription,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.Description))

                // Kontrol maddesi sıra numarası
                .ForMember(
                    dest => dest.MaintenanceChecklistSequenceNumber,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.SequenceNumber))

                // Kontrol maddesinin zorunluluk durumu
                .ForMember(
                    dest => dest.MaintenanceChecklistIsRequired,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.IsRequired))

                // Kontrol maddesinin aktiflik durumu
                .ForMember(
                    dest => dest.MaintenanceChecklistIsActive,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.IsActive))

                // Planlı bakım ID
                .ForMember(
                    dest => dest.PlannedMaintenanceId,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.PlannedMaintenanceId))

                // Planlı bakım adı
                .ForMember(
                    dest => dest.PlannedMaintenanceName,
                    opt => opt.MapFrom(src =>
                        src.MaintenanceChecklist.PlannedMaintenance.Name))

                // Kontrolü yapan kullanıcı adı
                .ForMember(
                    dest => dest.CheckedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.CheckedByUser != null
                            ? src.CheckedByUser.FirstName + " " +
                              src.CheckedByUser.LastName
                            : null))

                // Kontrolü yapan kullanıcı e-postası
                .ForMember(
                    dest => dest.CheckedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.CheckedByUser != null
                            ? src.CheckedByUser.Email
                            : null))

                // Kontrolü yapan kullanıcı unvanı
                .ForMember(
                    dest => dest.CheckedByUserTitle,
                    opt => opt.MapFrom(src =>
                        src.CheckedByUser != null
                            ? src.CheckedByUser.Title
                            : null));

            #endregion


            #region WorkOrderMaterial Mapping

            // CREATE
            // İş emrine yeni malzeme kullanım kaydı ekleme
            // ===================================================

            CreateMap<CreateWorkOrderMaterialDto, WorkOrderMaterial>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StockMovementId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
                .ForMember(dest => dest.Material, opt => opt.Ignore())
                .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
                .ForMember(dest => dest.StockMovement, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut malzeme kullanım kaydını güncelleme
            // ===================================================

            CreateMap<UpdateWorkOrderMaterialDto, WorkOrderMaterial>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.Ignore())
                .ForMember(dest => dest.StockMovementId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
                .ForMember(dest => dest.Material, opt => opt.Ignore())
                .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
                .ForMember(dest => dest.StockMovement, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<WorkOrderMaterial, UpdateWorkOrderMaterialDto>();


            // ===================================================
            // RESULT
            // İş emrinde kullanılan malzemeleri listeleme
            // ===================================================

            CreateMap<WorkOrderMaterial, ResultWorkOrderMaterialDto>()

                // İş emri numarası
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))

                // İş emri başlığı
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))

                // Malzeme kodu
                .ForMember(
                    dest => dest.MaterialCode,
                    opt => opt.MapFrom(src =>
                        src.Material.MaterialCode))

                // Malzeme adı
                .ForMember(
                    dest => dest.MaterialName,
                    opt => opt.MapFrom(src =>
                        src.Material.Name))

                // Ölçü birimi adı
                .ForMember(
                    dest => dest.MeasurementUnitName,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Name))

                // Ölçü birimi sembolü
                .ForMember(
                    dest => dest.MeasurementUnitSymbol,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Symbol))

                // Ambar kodu
                .ForMember(
                    dest => dest.WarehouseCode,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Code))

                // Ambar adı
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Name));


            // ===================================================
            // GET BY ID
            // Malzeme kullanım kaydının detaylarını getirme
            // ===================================================

            CreateMap<WorkOrderMaterial, GetByIdWorkOrderMaterialDto>()

                // İş emri numarası
                .ForMember(
                    dest => dest.WorkOrderNumber,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.WorkOrderNumber))

                // İş emri başlığı
                .ForMember(
                    dest => dest.WorkOrderTitle,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Title))

                // İş emri açıklaması
                .ForMember(
                    dest => dest.WorkOrderDescription,
                    opt => opt.MapFrom(src =>
                        src.WorkOrder.Description))

                // Malzeme kodu
                .ForMember(
                    dest => dest.MaterialCode,
                    opt => opt.MapFrom(src =>
                        src.Material.MaterialCode))

                // Malzeme adı
                .ForMember(
                    dest => dest.MaterialName,
                    opt => opt.MapFrom(src =>
                        src.Material.Name))

                // Malzeme açıklaması
                .ForMember(
                    dest => dest.MaterialDescription,
                    opt => opt.MapFrom(src =>
                        src.Material.Description))

                // Malzemenin aktiflik durumu
                .ForMember(
                    dest => dest.MaterialIsActive,
                    opt => opt.MapFrom(src =>
                        src.Material.IsActive))

                // Minimum stok seviyesi
                .ForMember(
                    dest => dest.MaterialMinimumStockLevel,
                    opt => opt.MapFrom(src =>
                        src.Material.MinimumStockLevel))

                // Ölçü birimi ID
                .ForMember(
                    dest => dest.MeasurementUnitId,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnitId))

                // Ölçü birimi adı
                .ForMember(
                    dest => dest.MeasurementUnitName,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Name))

                // Ölçü birimi sembolü
                .ForMember(
                    dest => dest.MeasurementUnitSymbol,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Symbol))

                // Ambar kodu
                .ForMember(
                    dest => dest.WarehouseCode,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Code))

                // Ambar adı
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Name))

                // Ambar konumu
                .ForMember(
                    dest => dest.WarehouseLocation,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Location))

                // Ambar aktiflik durumu
                .ForMember(
                    dest => dest.WarehouseIsActive,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.IsActive))

                // Stok hareketi referans numarası
                .ForMember(
                    dest => dest.StockMovementReferenceNumber,
                    opt => opt.MapFrom(src =>
                        src.StockMovement != null
                            ? src.StockMovement.ReferenceNumber
                            : null))

                // Stok hareketi tarihi
                .ForMember(
                    dest => dest.StockMovementDate,
                    opt => opt.MapFrom(src =>
                        src.StockMovement != null
                            ? (DateTime?)src.StockMovement.MovementDate
                            : null));

            #endregion

            #region MeasurementUnit Mapping

            // CREATE
            // Yeni ölçü birimi oluşturma
            // ===================================================

            CreateMap<CreateMeasurementUnitDto, MeasurementUnit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Materials, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut ölçü birimini güncelleme
            // ===================================================

            CreateMap<UpdateMeasurementUnitDto, MeasurementUnit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Materials, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<MeasurementUnit, UpdateMeasurementUnitDto>();


            // ===================================================
            // RESULT
            // Ölçü birimlerini listeleme
            // ===================================================

            CreateMap<MeasurementUnit, ResultMeasurementUnitDto>()
                .ForMember(
                    dest => dest.MaterialCount,
                    opt => opt.MapFrom(src =>
                        src.Materials.Count));


            // ===================================================
            // GET BY ID
            // Ölçü biriminin detaylarını getirme
            // ===================================================

            CreateMap<MeasurementUnit, GetByIdMeasurementUnitDto>()

                // Toplam malzeme sayısı
                .ForMember(
                    dest => dest.MaterialCount,
                    opt => opt.MapFrom(src =>
                        src.Materials.Count))

                // Aktif malzeme sayısı
                .ForMember(
                    dest => dest.ActiveMaterialCount,
                    opt => opt.MapFrom(src =>
                        src.Materials.Count(x => x.IsActive)))

                // Pasif malzeme sayısı
                .ForMember(
                    dest => dest.PassiveMaterialCount,
                    opt => opt.MapFrom(src =>
                        src.Materials.Count(x => !x.IsActive)));

            #endregion

            #region MaterialCategory Mapping


            // CREATE
            // Yeni malzeme kategorisi oluşturma
            // ===================================================

            CreateMap<CreateMaterialCategoryDto, MaterialCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Materials, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut malzeme kategorisini güncelleme
            // ===================================================

            CreateMap<UpdateMaterialCategoryDto, MaterialCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Materials, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<MaterialCategory, UpdateMaterialCategoryDto>();


            // ===================================================
            // RESULT
            // Malzeme kategorilerini listeleme
            // ===================================================

            CreateMap<MaterialCategory, ResultMaterialCategoryDto>()

                // Kategoriye bağlı toplam malzeme sayısı
                .ForMember(
                    dest => dest.MaterialCount,
                    opt => opt.MapFrom(src =>
                        src.Materials.Count));


            // ===================================================
            // GET BY ID
            // Malzeme kategorisinin detaylarını getirme
            // ===================================================

            CreateMap<MaterialCategory, GetByIdMaterialCategoryDto>()

                // Toplam malzeme sayısı
                .ForMember(
                    dest => dest.MaterialCount,
                    opt => opt.MapFrom(src =>
                        src.Materials.Count))

                // Aktif malzeme sayısı
                .ForMember(
                    dest => dest.ActiveMaterialCount,
                    opt => opt.MapFrom(src =>
                        src.Materials.Count(x => x.IsActive)))

                // Pasif malzeme sayısı
                .ForMember(
                    dest => dest.PassiveMaterialCount,
                    opt => opt.MapFrom(src =>
                        src.Materials.Count(x => !x.IsActive)));
            #endregion

            #region Material Mapping

            // CREATE
            // Yeni malzeme oluşturma
            // ===================================================

            CreateMap<CreateMaterialDto, Material>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.MeasurementUnit, opt => opt.Ignore())
                .ForMember(dest => dest.MaterialCategory, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderMaterials, opt => opt.Ignore())
                .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseStocks, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut malzemeyi güncelleme
            // ===================================================

            CreateMap<UpdateMaterialDto, Material>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.MeasurementUnit, opt => opt.Ignore())
                .ForMember(dest => dest.MaterialCategory, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderMaterials, opt => opt.Ignore())
                .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseStocks, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<Material, UpdateMaterialDto>();


            // ===================================================
            // RESULT
            // Malzemeleri listeleme
            // ===================================================

            CreateMap<Material, ResultMaterialDto>()

                // Ölçü birimi adı
                .ForMember(
                    dest => dest.MeasurementUnitName,
                    opt => opt.MapFrom(src =>
                        src.MeasurementUnit.Name))

                // Ölçü birimi sembolü
                .ForMember(
                    dest => dest.MeasurementUnitSymbol,
                    opt => opt.MapFrom(src =>
                        src.MeasurementUnit.Symbol))

                // Malzeme kategorisi adı
                .ForMember(
                    dest => dest.MaterialCategoryName,
                    opt => opt.MapFrom(src =>
                        src.MaterialCategory != null
                            ? src.MaterialCategory.Name
                            : null))

                // İlgili organizasyon birimi adı
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit != null
                            ? src.Unit.Name
                            : null));


            // ===================================================
            // GET BY ID
            // Malzeme detaylarını getirme
            // ===================================================

            CreateMap<Material, GetByIdMaterialDto>()

                // Ölçü birimi adı
                .ForMember(
                    dest => dest.MeasurementUnitName,
                    opt => opt.MapFrom(src =>
                        src.MeasurementUnit.Name))

                // Ölçü birimi sembolü
                .ForMember(
                    dest => dest.MeasurementUnitSymbol,
                    opt => opt.MapFrom(src =>
                        src.MeasurementUnit.Symbol))

                // Malzeme kategorisi adı
                .ForMember(
                    dest => dest.MaterialCategoryName,
                    opt => opt.MapFrom(src =>
                        src.MaterialCategory != null
                            ? src.MaterialCategory.Name
                            : null))

                // Malzeme kategorisi açıklaması
                .ForMember(
                    dest => dest.MaterialCategoryDescription,
                    opt => opt.MapFrom(src =>
                        src.MaterialCategory != null
                            ? src.MaterialCategory.Description
                            : null))

                // Malzeme kategorisi aktiflik durumu
                .ForMember(
                    dest => dest.MaterialCategoryIsActive,
                    opt => opt.MapFrom(src =>
                        src.MaterialCategory != null
                            ? (bool?)src.MaterialCategory.IsActive
                            : null))

                // İlgili organizasyon birimi adı
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit != null
                            ? src.Unit.Name
                            : null))

                // İlgili organizasyon birimi kodu
                .ForMember(
                    dest => dest.UnitCode,
                    opt => opt.MapFrom(src =>
                        src.Unit != null
                            ? src.Unit.Code
                            : null))

                // İş emrinde kullanım kayıt sayısı
                .ForMember(
                    dest => dest.WorkOrderMaterialCount,
                    opt => opt.MapFrom(src =>
                        src.WorkOrderMaterials.Count))

                // Stok hareketi sayısı
                .ForMember(
                    dest => dest.StockMovementCount,
                    opt => opt.MapFrom(src =>
                        src.StockMovements.Count))

                // Ambar stok kayıt sayısı
                .ForMember(
                    dest => dest.WarehouseStockCount,
                    opt => opt.MapFrom(src =>
                        src.WarehouseStocks.Count));

            #endregion


            #region Warehouse Mapping

            // CREATE
            // Yeni ambar oluşturma
            // ===================================================

            CreateMap<CreateWarehouseDto, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseStocks, opt => opt.Ignore())
                .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderMaterials, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut ambarı güncelleme
            // ===================================================

            CreateMap<UpdateWarehouseDto, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseStocks, opt => opt.Ignore())
                .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
                .ForMember(dest => dest.WorkOrderMaterials, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<Warehouse, UpdateWarehouseDto>();


            // ===================================================
            // RESULT
            // Ambar kayıtlarını listeleme
            // ===================================================

            CreateMap<Warehouse, ResultWarehouseDto>()

                // Bağlı organizasyon biriminin adı
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit != null
                            ? src.Unit.Name
                            : null))

                // Toplam stok kayıt sayısı
                .ForMember(
                    dest => dest.WarehouseStockCount,
                    opt => opt.MapFrom(src =>
                        src.WarehouseStocks.Count))

                // Toplam stok hareketi sayısı
                .ForMember(
                    dest => dest.StockMovementCount,
                    opt => opt.MapFrom(src =>
                        src.StockMovements.Count));


            // ===================================================
            // GET BY ID
            // Ambar detaylarını görüntüleme
            // ===================================================

            CreateMap<Warehouse, GetByIdWarehouseDto>()

                // Bağlı organizasyon biriminin adı
                .ForMember(
                    dest => dest.UnitName,
                    opt => opt.MapFrom(src =>
                        src.Unit != null
                            ? src.Unit.Name
                            : null))

                // Bağlı organizasyon biriminin kodu
                .ForMember(
                    dest => dest.UnitCode,
                    opt => opt.MapFrom(src =>
                        src.Unit != null
                            ? src.Unit.Code
                            : null))

                // Toplam stok kayıt sayısı
                .ForMember(
                    dest => dest.WarehouseStockCount,
                    opt => opt.MapFrom(src =>
                        src.WarehouseStocks.Count))

                // Miktarı sıfırdan büyük stok kayıtları
                .ForMember(
                    dest => dest.AvailableMaterialCount,
                    opt => opt.MapFrom(src =>
                        src.WarehouseStocks.Count(x =>
                            x.Quantity > 0)))

                // Miktarı sıfır olan stok kayıtları
                .ForMember(
                    dest => dest.OutOfStockCount,
                    opt => opt.MapFrom(src =>
                        src.WarehouseStocks.Count(x =>
                            x.Quantity == 0)))

                // Toplam stok hareketi sayısı
                .ForMember(
                    dest => dest.StockMovementCount,
                    opt => opt.MapFrom(src =>
                        src.StockMovements.Count))

                // İş emrinde kullanılan malzeme kayıt sayısı
                .ForMember(
                    dest => dest.WorkOrderMaterialCount,
                    opt => opt.MapFrom(src =>
                        src.WorkOrderMaterials.Count));

            #endregion

            #region WarehouseStock Mapping

            // CREATE
            // Yeni ambar stok kaydı oluşturma
            // ===================================================

            CreateMap<CreateWarehouseStockDto, WarehouseStock>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LastMovementDate, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
                .ForMember(dest => dest.Material, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Mevcut ambar stok miktarını güncelleme
            // ===================================================

            CreateMap<UpdateWarehouseStockDto, WarehouseStock>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore())
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
                .ForMember(dest => dest.LastMovementDate, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
                .ForMember(dest => dest.Material, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<WarehouseStock, UpdateWarehouseStockDto>();


            // ===================================================
            // RESULT
            // Ambar stok kayıtlarını listeleme
            // ===================================================

            CreateMap<WarehouseStock, ResultWarehouseStockDto>()

                // Ambar kodu
                .ForMember(
                    dest => dest.WarehouseCode,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Code))

                // Ambar adı
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Name))

                // Malzeme kodu
                .ForMember(
                    dest => dest.MaterialCode,
                    opt => opt.MapFrom(src =>
                        src.Material.MaterialCode))

                // Malzeme adı
                .ForMember(
                    dest => dest.MaterialName,
                    opt => opt.MapFrom(src =>
                        src.Material.Name))

                // Ölçü birimi adı
                .ForMember(
                    dest => dest.MeasurementUnitName,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Name))

                // Ölçü birimi sembolü
                .ForMember(
                    dest => dest.MeasurementUnitSymbol,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Symbol))

                // Minimum stok seviyesi
                .ForMember(
                    dest => dest.MinimumStockLevel,
                    opt => opt.MapFrom(src =>
                        src.Material.MinimumStockLevel));


            // ===================================================
            // GET BY ID
            // Ambar stok kaydının detaylarını getirme
            // ===================================================

            CreateMap<WarehouseStock, GetByIdWarehouseStockDto>()

                // Ambar kodu
                .ForMember(
                    dest => dest.WarehouseCode,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Code))

                // Ambar adı
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Name))

                // Ambar konumu
                .ForMember(
                    dest => dest.WarehouseLocation,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.Location))

                // Ambar aktiflik durumu
                .ForMember(
                    dest => dest.WarehouseIsActive,
                    opt => opt.MapFrom(src =>
                        src.Warehouse.IsActive))

                // Malzeme kodu
                .ForMember(
                    dest => dest.MaterialCode,
                    opt => opt.MapFrom(src =>
                        src.Material.MaterialCode))

                // Malzeme adı
                .ForMember(
                    dest => dest.MaterialName,
                    opt => opt.MapFrom(src =>
                        src.Material.Name))

                // Malzeme açıklaması
                .ForMember(
                    dest => dest.MaterialDescription,
                    opt => opt.MapFrom(src =>
                        src.Material.Description))

                // Malzeme aktiflik durumu
                .ForMember(
                    dest => dest.MaterialIsActive,
                    opt => opt.MapFrom(src =>
                        src.Material.IsActive))

                // Malzeme kategorisi ID
                .ForMember(
                    dest => dest.MaterialCategoryId,
                    opt => opt.MapFrom(src =>
                        src.Material.MaterialCategoryId))

                // Malzeme kategorisi adı
                .ForMember(
                    dest => dest.MaterialCategoryName,
                    opt => opt.MapFrom(src =>
                        src.Material.MaterialCategory != null
                            ? src.Material.MaterialCategory.Name
                            : null))

                // Ölçü birimi ID
                .ForMember(
                    dest => dest.MeasurementUnitId,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnitId))

                // Ölçü birimi adı
                .ForMember(
                    dest => dest.MeasurementUnitName,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Name))

                // Ölçü birimi sembolü
                .ForMember(
                    dest => dest.MeasurementUnitSymbol,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Symbol))

                // Minimum stok seviyesi
                .ForMember(
                    dest => dest.MinimumStockLevel,
                    opt => opt.MapFrom(src =>
                        src.Material.MinimumStockLevel));

            #endregion


            #region StockMovements Mapping
            // CREATE
            // Yeni stok giriş veya çıkış hareketi oluşturma
            // ===================================================

            CreateMap<CreateStockMovementDto, StockMovement>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.MovementDate, opt => opt.Ignore())
                .ForMember(dest => dest.TransferGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
                .ForMember(dest => dest.Material, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedByUser, opt => opt.Ignore());


            // ===================================================
            // UPDATE
            // Stok hareketinin açıklama ve referansını güncelleme
            // ===================================================

            CreateMap<UpdateStockMovementDto, StockMovement>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore())
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.MovementType, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.Ignore())
                .ForMember(dest => dest.MovementDate, opt => opt.Ignore())
                .ForMember(dest => dest.TransferGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
                .ForMember(dest => dest.Material, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedByUser, opt => opt.Ignore());


            // ===================================================
            // ENTITY -> UPDATE DTO
            // Güncelleme formunu doldurma
            // ===================================================

            CreateMap<StockMovement, UpdateStockMovementDto>();


            // ===================================================
            // RESULT
            // Stok hareketlerini listeleme
            // ===================================================

            CreateMap<StockMovement, ResultStockMovementDto>()
                .ForMember(
                    dest => dest.WarehouseCode,
                    opt => opt.MapFrom(src => src.Warehouse.Code))
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src => src.Warehouse.Name))
                .ForMember(
                    dest => dest.MaterialCode,
                    opt => opt.MapFrom(src => src.Material.MaterialCode))
                .ForMember(
                    dest => dest.MaterialName,
                    opt => opt.MapFrom(src => src.Material.Name))
                .ForMember(
                    dest => dest.MeasurementUnitName,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Name))
                .ForMember(
                    dest => dest.MeasurementUnitSymbol,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Symbol))
                .ForMember(
                    dest => dest.PerformedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.FirstName + " " +
                        src.PerformedByUser.LastName));


            // ===================================================
            // GET BY ID
            // Stok hareketinin ayrıntılarını getirme
            // ===================================================

            CreateMap<StockMovement, GetByIdStockMovementDto>()
                .ForMember(
                    dest => dest.WarehouseCode,
                    opt => opt.MapFrom(src => src.Warehouse.Code))
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src => src.Warehouse.Name))
                .ForMember(
                    dest => dest.WarehouseLocation,
                    opt => opt.MapFrom(src => src.Warehouse.Location))
                .ForMember(
                    dest => dest.WarehouseIsActive,
                    opt => opt.MapFrom(src => src.Warehouse.IsActive))
                .ForMember(
                    dest => dest.MaterialCode,
                    opt => opt.MapFrom(src => src.Material.MaterialCode))
                .ForMember(
                    dest => dest.MaterialName,
                    opt => opt.MapFrom(src => src.Material.Name))
                .ForMember(
                    dest => dest.MaterialDescription,
                    opt => opt.MapFrom(src => src.Material.Description))
                .ForMember(
                    dest => dest.MaterialCategoryId,
                    opt => opt.MapFrom(src =>
                        src.Material.MaterialCategoryId))
                .ForMember(
                    dest => dest.MaterialCategoryName,
                    opt => opt.MapFrom(src =>
                        src.Material.MaterialCategory != null
                            ? src.Material.MaterialCategory.Name
                            : null))
                .ForMember(
                    dest => dest.MeasurementUnitId,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnitId))
                .ForMember(
                    dest => dest.MeasurementUnitName,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Name))
                .ForMember(
                    dest => dest.MeasurementUnitSymbol,
                    opt => opt.MapFrom(src =>
                        src.Material.MeasurementUnit.Symbol))
                .ForMember(
                    dest => dest.PerformedByUserFullName,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.FirstName + " " +
                        src.PerformedByUser.LastName))
                .ForMember(
                    dest => dest.PerformedByUserEmail,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.Email))
                .ForMember(
                    dest => dest.PerformedByUserTitle,
                    opt => opt.MapFrom(src =>
                        src.PerformedByUser.Title));


            #endregion
        }

    }
}
