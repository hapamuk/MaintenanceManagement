using MaintenanceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Data
{
    public static class AdminSeedData
    {
        #region Constants

        private const string AdminRoleName = "admin";

        private const string AdminRoleCode = "ADMIN";

        private const string AdminUserName = "admin";

        private const string InitialPassword = "1";

        private const string SystemOrganizationTypeCode = "SYSTEM";

        private const string SystemUnitCode = "SYS_ADMIN";

        #endregion

        #region SeedAsync

        /// <summary>
        /// Organizasyon, birim, rol, kullanıcı ve
        /// kullanıcı-rol ilişkisini oluşturur.
        ///
        /// Bu metod yalnızca geliştirme ortamında
        /// çağrılmalıdır.
        /// </summary>
        public static async Task SeedAsync(
            ApplicationDbContext context,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            // =====================================
            // 1. TRANSACTION BAŞLAT
            // =====================================

            await using var transaction =
                await context.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                // =====================================
                // 2. ADMIN ROLÜNÜ KONTROL ET
                // =====================================

                var adminRole = await context.Roles
                    .FirstOrDefaultAsync(
                        x => x.Code == AdminRoleCode,
                        cancellationToken);

                // =====================================
                // 3. ADMIN ROLÜ YOKSA OLUŞTUR
                // =====================================

                if (adminRole == null)
                {
                    // =================================
                    // 3.1 ORGANİZASYON TÜRÜ
                    // =================================

                    var organizationType =
                        await context.OrganizationTypes
                            .FirstOrDefaultAsync(
                                x => x.Code ==
                                    SystemOrganizationTypeCode,
                                cancellationToken);

                    if (organizationType == null)
                    {
                        organizationType = new OrganizationType
                        {
                            Name = "Sistem",

                            Code = SystemOrganizationTypeCode,

                            Description =
                                "Sistem yönetimi için başlangıç " +
                                "organizasyon türü.",

                            IsActive = true,

                            CreatedAt = DateTime.UtcNow
                        };

                        await context.OrganizationTypes
                            .AddAsync(
                                organizationType,
                                cancellationToken);

                        await context.SaveChangesAsync(
                            cancellationToken);
                    }

                    // =================================
                    // 3.2 ORGANİZASYON BİRİMİ
                    // =================================

                    var systemUnit = await context.Units
                        .FirstOrDefaultAsync(
                            x => x.Code == SystemUnitCode,
                            cancellationToken);

                    if (systemUnit == null)
                    {
                        systemUnit = new Unit
                        {
                            Name = "Sistem Yönetimi",

                            Code = SystemUnitCode,

                            OrganizationTypeId =
                                organizationType.Id,

                            IsActive = true,

                            CreatedAt = DateTime.UtcNow
                        };

                        await context.Units.AddAsync(
                            systemUnit,
                            cancellationToken);

                        await context.SaveChangesAsync(
                            cancellationToken);
                    }

                    // =================================
                    // 3.3 ADMIN ROLÜ
                    // =================================

                    adminRole = new Role
                    {
                        Name = AdminRoleName,

                        Code = AdminRoleCode,

                        Description = "Sistem yöneticisi rolü.",

                        UnitId = systemUnit.Id,

                        IsActive = true,

                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Roles.AddAsync(
                        adminRole,
                        cancellationToken);

                    await context.SaveChangesAsync(
                        cancellationToken);
                }

                // =====================================
                // 4. ADMIN ROLÜNÜ DOĞRULA
                // =====================================

                if (!string.Equals(
                    adminRole.Name,
                    AdminRoleName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "ADMIN kodu farklı isimli bir " +
                        "role ait. Başlangıç verisi " +
                        "oluşturulamadı.");
                }

                // =====================================
                // 5. ADMIN KULLANICISINI ARA
                // =====================================

                var adminUser = await context.Users
                    .FirstOrDefaultAsync(
                        x => x.Username == AdminUserName,
                        cancellationToken);

                // =====================================
                // 6. ADMIN KULLANICISI YOKSA OLUŞTUR
                // =====================================

                if (adminUser == null)
                {
                    // =================================
                    // 6.1 ŞİFREYİ BCRYPT İLE HASH'LE
                    // =================================

                    string passwordHash =
                        BCrypt.Net.BCrypt.HashPassword(
                            InitialPassword,
                            workFactor: 12);

                    // =================================
                    // 6.2 KULLANICI ENTITY
                    // =================================

                    adminUser = new User
                    {
                        FirstName = "admin",

                        LastName = "admin",

                        Username = AdminUserName,

                        PasswordHash = passwordHash,

                        PrimaryUnitId = adminRole.UnitId,

                        HasSystemAccess = true,

                        IsActive = true,

                        CreatedAt = DateTime.UtcNow
                    };

                    // =================================
                    // 6.3 KULLANICIYI EKLE
                    // =================================

                    await context.Users.AddAsync(
                        adminUser,
                        cancellationToken);

                    await context.SaveChangesAsync(
                        cancellationToken);
                }

                // =====================================
                // 7. KULLANICI ROL ATAMASINI KONTROL ET
                // =====================================

                bool roleAssigned =
                    await context.UserUnitRoles.AnyAsync(
                        x =>
                            x.UserId == adminUser.Id &&
                            x.RoleId == adminRole.Id,
                        cancellationToken);

                // =====================================
                // 8. ROL ATAMASI YOKSA OLUŞTUR
                // =====================================

                if (!roleAssigned)
                {
                    var userUnitRole = new UserUnitRole
                    {
                        UserId = adminUser.Id,

                        RoleId = adminRole.Id,

                        AssignedAt = DateTime.UtcNow,

                        IsActive = true,

                        CreatedAt = DateTime.UtcNow
                    };

                    await context.UserUnitRoles.AddAsync(
                        userUnitRole,
                        cancellationToken);

                    await context.SaveChangesAsync(
                        cancellationToken);
                }

                // =====================================
                // 9. TRANSACTION COMMIT
                // =====================================

                await transaction.CommitAsync(
                    cancellationToken);
            }
            catch
            {
                // =====================================
                // 10. HATA DURUMUNDA ROLLBACK
                // =====================================

                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }

        #endregion
    }
}
