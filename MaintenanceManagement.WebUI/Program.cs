using MaintenanceManagement.Application.DependencyInjection;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Application.Mapping;
using MaintenanceManagement.Infrastructure.Data;
using MaintenanceManagement.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// 1. MVC SERVİSLERİ
// =====================================

// Add services to the container.
builder.Services.AddControllersWithViews();


// =====================================
// 2. VERİTABANI BAĞLANTISI
// =====================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlConnection"));
});




// =====================================
// 3. APPLICATION KATMANI
// =====================================

#region Application Services

// Application katmanındaki bütün servisleri kaydeder.
//
// IRoleService - RoleService
// IUserService - UserService
// IUnitService - UnitService
// IMaterialService - MaterialService
// IWarehouseService - WarehouseService
// ve diğer servisler.
//
// GeneralMapping AutoMapper kaydı da
// AddApplicationServices içerisinde yapılır.

builder.Services.AddApplicationServices();

#endregion


// =====================================
// 4. INFRASTRUCTURE KATMANI
// =====================================

#region Infrastructure Repositories

// Infrastructure katmanındaki Repository
// implementasyonlarını Dependency Injection'a ekler.
//
// IRoleRepository - RoleRepository
// IUserRepository - UserRepository
// IMaterialRepository - MaterialRepository
// IWarehouseRepository - WarehouseRepository
// ve diğer Repository sınıfları.

builder.Services.AddInfrastructureRepositories();
builder.Services.AddScoped<
    IPasswordResetService,PasswordResetService>();

#endregion

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MaintenanceManagement.Auth";
        options.Cookie.HttpOnly = true;

        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;

        options.LoginPath = "/Admin/User/Login";
        options.AccessDeniedPath = "/Admin/User/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromMinutes(30);

        options.SlidingExpiration = true;
    });


// =====================================
// 5. APPLICATION BUILD
// =====================================

var app = builder.Build();


// =====================================
// 6. ADMIN BAŞLANGIÇ VERİLERİ
// =====================================

#region Admin Seed Data

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await AdminSeedData.SeedAsync(context);
}

#endregion


// =====================================
// 7. HTTP REQUEST PIPELINE
// =====================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();




app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// =====================================
// 9. APPLICATION RUN
// =====================================

app.Run();