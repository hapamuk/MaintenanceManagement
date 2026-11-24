using MaintenanceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MaintenanceManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructor

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #endregion


        #region Organizasyon, Kullanıcı, Rol ve Yetki Yönetimi



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

        // Demirbaş kategori - organizasyon kullanım kapsamı
        public DbSet<AssetCategoryUnit> AssetCategoryUnits
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

        #region Demirbaş Konum Yönetimi (Bina / Kat / Oda)

        public DbSet<AssetBuilding> AssetBuildings
        { get; set; } = null!;

        public DbSet<AssetBuildingFloor> AssetBuildingFloors
        { get; set; } = null!;

        public DbSet<AssetBuildingRoom> AssetBuildingRooms
        { get; set; } = null!;

        #endregion

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

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

        public DbSet<UnitCodeCounter> UnitCodeCounters { get; set; }

        public DbSet<MaterialCategoryUnit> MaterialCategoryUnits { get; set; }

        public DbSet<WarehousePersonnel> WarehousePersonnels { get; set; } = null!;


        #region Demirbaş Tür-Marka ve Üst Varlık Bağlantıları

        public DbSet<AssetKindBrand> AssetKindBrands { get; set; } = null!;
        public DbSet<AssetParentLink> AssetParentLinks { get; set; } = null!;

        #endregion

        // Demirbaşın zimmet ve iş emri DIŞINDAKİ değişiklik geçmişi.
        public DbSet<AssetAuditEvent> AssetAuditEvents { get; set; } = null!;

        #region OnModelCreating

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            #region Unit Fluent API

            modelBuilder.Entity<Unit>(entity =>
            {
                // =====================================
                // 1. ORGANİZASYON TÜRÜ (ENUM)
                // =====================================

                // OrganizationType entity / foreign key kullanılmaz.
                // Enum, Units tablosunda int olarak saklanır.
                entity.Property(x => x.OrganizationTypeEnum)
                    .HasConversion<int>()
                    .IsRequired();

                // =====================================
                // 2. ÜST BİRİM - ALT BİRİM
                // =====================================

                entity.HasOne(x => x.ParentUnit)
                    .WithMany(x => x.ChildUnits)
                    .HasForeignKey(x => x.ParentUnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region User Fluent API

            modelBuilder.Entity<User>(entity =>
            {
                // =====================================
                // 1. KULLANICININ ANA BİRİMİ
                // =====================================

                entity.HasOne(x => x.PrimaryUnit)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.PrimaryUnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region AssetCategoryUnit Fluent API

            // Mevcut AssetCategory ve Unit kayıtlarına bağlanır.
            // AssetCategoryUnit entity'sinde Id bulunmadığı için
            // AssetCategoryId + UnitId birlikte birincil anahtardır.
            modelBuilder.Entity<AssetCategoryUnit>(entity =>
            {
                entity.ToTable("AssetCategoryUnits");

                entity.HasKey(x => new
                {
                    x.AssetCategoryId,
                    x.UnitId
                });

                entity.HasOne<AssetCategory>()
                    .WithMany()
                    .HasForeignKey(x => x.AssetCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Unit>()
                    .WithMany()
                    .HasForeignKey(x => x.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region AssetMovement Fluent API

            modelBuilder.Entity<AssetMovement>(entity =>
            {
                // =====================================
                // 1. DEMİRBAŞIN ÇIKTIĞI BİRİM
                // =====================================

                entity.HasOne(x => x.FromUnit)
                    .WithMany(x => x.OutgoingAssetMovements)
                    .HasForeignKey(x => x.FromUnitId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. DEMİRBAŞIN GİTTİĞİ BİRİM
                // =====================================

                entity.HasOne(x => x.ToUnit)
                    .WithMany(x => x.IncomingAssetMovements)
                    .HasForeignKey(x => x.ToUnitId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 3. DEMİRBAŞIN ÇIKTIĞI KULLANICI
                // =====================================

                entity.HasOne(x => x.FromUser)
                    .WithMany(x => x.OutgoingUserAssetMovements)
                    .HasForeignKey(x => x.FromUserId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 4. DEMİRBAŞIN GİTTİĞİ KULLANICI
                // =====================================

                entity.HasOne(x => x.ToUser)
                    .WithMany(x => x.IncomingUserAssetMovements)
                    .HasForeignKey(x => x.ToUserId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 5. HAREKETİ YAPAN KULLANICI
                // =====================================

                entity.HasOne(x => x.PerformedByUser)
                    .WithMany(x => x.AssetMovementsPerformed)
                    .HasForeignKey(x => x.PerformedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region AssetUser Fluent API

            modelBuilder.Entity<AssetUser>(entity =>
            {
                // =====================================
                // 1. DEMİRBAŞIN ZİMMETLENDİĞİ KULLANICI
                // =====================================

                entity.HasOne(x => x.User)
                    .WithMany(x => x.AssetAssignments)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. ZİMMET İŞLEMİNİ YAPAN KULLANICI
                // =====================================

                entity.HasOne(x => x.AssignedByUser)
                    .WithMany(x => x.AssignmentsMade)
                    .HasForeignKey(x => x.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region WorkRequest Fluent API

            modelBuilder.Entity<WorkRequest>(entity =>
            {
                // =====================================
                // 1. TALEBİ OLUŞTURAN BİRİM
                // =====================================

                entity.HasOne(x => x.RequesterUnit)
                    .WithMany(x => x.RequestedWorkRequests)
                    .HasForeignKey(x => x.RequesterUnitId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. TALEPTEN SORUMLU BİRİM
                // =====================================

                entity.HasOne(x => x.ResponsibleUnit)
                    .WithMany(x => x.ResponsibleWorkRequests)
                    .HasForeignKey(x => x.ResponsibleUnitId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 3. TALEBİ OLUŞTURAN KULLANICI
                // =====================================

                entity.HasOne(x => x.RequestedByUser)
                    .WithMany(x => x.CreatedWorkRequests)
                    .HasForeignKey(x => x.RequestedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 4. TALEBİ YÖNETEN KULLANICI
                // =====================================

                entity.HasOne(x => x.ManagerUser)
                    .WithMany(x => x.ManagedWorkRequests)
                    .HasForeignKey(x => x.ManagerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region WorkRequestAction Fluent API

            modelBuilder.Entity<WorkRequestAction>(entity =>
            {
                // =====================================
                // 1. İŞ TALEBİ
                // =====================================

                entity.HasOne(x => x.WorkRequest)
                    .WithMany(x => x.Actions)
                    .HasForeignKey(x => x.WorkRequestId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. İŞLEMİ GERÇEKLEŞTİREN KULLANICI
                // =====================================

                entity.HasOne(x => x.PerformedByUser)
                    .WithMany(x => x.WorkRequestActions)
                    .HasForeignKey(x => x.PerformedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region WorkRequestFile Fluent API

            modelBuilder.Entity<WorkRequestFile>(entity =>
            {
                // =====================================
                // 1. İŞ TALEBİ
                // =====================================

                entity.HasOne(x => x.WorkRequest)
                    .WithMany(x => x.Files)
                    .HasForeignKey(x => x.WorkRequestId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. DOSYAYI YÜKLEYEN KULLANICI
                // =====================================

                entity.HasOne(x => x.UploadedByUser)
                    .WithMany(x => x.UploadedRequestFiles)
                    .HasForeignKey(x => x.UploadedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region WorkOrder Fluent API

            modelBuilder.Entity<WorkOrder>(entity =>
            {
                // =====================================
                // 1. İŞ EMRİNDEN SORUMLU BİRİM
                // =====================================

                entity.HasOne(x => x.ResponsibleUnit)
                    .WithMany(x => x.WorkOrders)
                    .HasForeignKey(x => x.ResponsibleUnitId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. İŞ EMRİNİ OLUŞTURAN KULLANICI
                // =====================================

                entity.HasOne(x => x.CreatedByUser)
                    .WithMany(x => x.CreatedWorkOrders)
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 3. İŞ EMRİNİ ONAYLAYAN KULLANICI
                // =====================================

                entity.HasOne(x => x.ApprovedByUser)
                    .WithMany(x => x.ApprovedWorkOrders)
                    .HasForeignKey(x => x.ApprovedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 4. İŞ EMRİNİN BAĞLI OLDUĞU TALEP
                // =====================================

                // DÜZELTİLDİ:
                //
                // WorkRequest.WorkOrder bir koleksiyon
                // değildir. Tek bir WorkOrder nesnesidir.
                //
                // Bu nedenle WithMany yerine
                // WithOne kullanılmalıdır.
                //
                // Foreign Key WorkOrder sınıfındadır.

                entity.HasOne(x => x.WorkRequest)
                    .WithOne(x => x.WorkOrder)
                    .HasForeignKey<WorkOrder>(
                        x => x.WorkRequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region WorkOrderPersonnel Fluent API

            modelBuilder.Entity<WorkOrderPersonnel>(entity =>
            {
                // =====================================
                // 1. İŞ EMRİ
                // =====================================

                entity.HasOne(x => x.WorkOrder)
                    .WithMany(x => x.Personnel)
                    .HasForeignKey(x => x.WorkOrderId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. PERSONELİ ATAYAN KULLANICI
                // =====================================

                entity.HasOne(x => x.AssignedByUser)
                    .WithMany(x => x.PersonnelAssignmentsMade)
                    .HasForeignKey(x => x.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region WorkOrderAction Fluent API

            modelBuilder.Entity<WorkOrderAction>(entity =>
            {
                // =====================================
                // 1. İŞ EMRİ
                // =====================================

                entity.HasOne(x => x.WorkOrder)
                    .WithMany(x => x.Actions)
                    .HasForeignKey(x => x.WorkOrderId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. İŞLEMİ GERÇEKLEŞTİREN KULLANICI
                // =====================================

                entity.HasOne(x => x.PerformedByUser)
                    .WithMany(x => x.WorkOrderActions)
                    .HasForeignKey(x => x.PerformedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region WorkOrderStatusHistory Fluent API

            modelBuilder.Entity<WorkOrderStatusHistory>(entity =>
            {
                // =====================================
                // 1. İŞ EMRİ
                // =====================================

                entity.HasOne(x => x.WorkOrder)
                    .WithMany(x => x.StatusHistory)
                    .HasForeignKey(x => x.WorkOrderId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. DURUMU DEĞİŞTİREN KULLANICI
                // =====================================

                entity.HasOne(x => x.ChangedByUser)
                    .WithMany(x => x.WorkOrderStatusChanges)
                    .HasForeignKey(x => x.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            #region WorkOrderFile Fluent API

            modelBuilder.Entity<WorkOrderFile>(entity =>
            {
                // =====================================
                // 1. İŞ EMRİ
                // =====================================

                entity.HasOne(x => x.WorkOrder)
                    .WithMany(x => x.Files)
                    .HasForeignKey(x => x.WorkOrderId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =====================================
                // 2. DOSYAYI YÜKLEYEN KULLANICI
                // =====================================

                entity.HasOne(x => x.UploadedByUser)
                    .WithMany(x => x.UploadedWorkOrderFiles)
                    .HasForeignKey(x => x.UploadedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion


            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<WarehousePersonnel>(entity =>
            {
                entity.HasIndex(x => new { x.WarehouseId, x.UserId }).IsUnique();
                entity.HasOne(x => x.Warehouse)
                    .WithMany(x => x.WarehousePersonnels)
                    .HasForeignKey(x => x.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            #region Decimal Precision Configurations

            // =====================================
            // 1. MALZEME - MINIMUM STOK SEVIYESI
            // =====================================

            modelBuilder.Entity<Material>()
                .Property(x => x.MinimumStockLevel)
                .HasPrecision(18, 4);


            // =====================================
            // 2. KONUM - ENLEM
            // =====================================

            modelBuilder.Entity<SiteLocation>()
                .Property(x => x.Latitude)
                .HasPrecision(10, 7);


            // =====================================
            // 3. KONUM - BOYLAM
            // =====================================

            modelBuilder.Entity<SiteLocation>()
                .Property(x => x.Longitude)
                .HasPrecision(10, 7);


            // =====================================
            // 4. STOK HAREKET MIKTARI
            // =====================================

            modelBuilder.Entity<StockMovement>()
                .Property(x => x.Quantity)
                .HasPrecision(18, 4);


            // =====================================
            // 5. AMBAR STOK MIKTARI
            // =====================================

            modelBuilder.Entity<WarehouseStock>()
                .Property(x => x.Quantity)
                .HasPrecision(18, 4);


            // =====================================
            // 6. IS EMRI MALZEME MIKTARI
            // =====================================

            modelBuilder.Entity<WorkOrderMaterial>()
                .Property(x => x.Quantity)
                .HasPrecision(18, 4);


            // =====================================
            // 7. IS EMRI MALZEME BIRIM MALIYETI
            // =====================================

            modelBuilder.Entity<WorkOrderMaterial>()
                .Property(x => x.UnitCost)
                .HasPrecision(18, 2);

            #endregion



            #region Demirbaş Konum Yönetimi Fluent API

            // Binalar: haritada işaretlenecek kayıtlı yerler.
            modelBuilder.Entity<AssetBuilding>(entity =>
            {
                entity.ToTable("AssetBuildings");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                // İl / ilçe / mahalle isteğe bağlı adres filtreleri (V6.4)
                entity.Property(x => x.Province)
                    .HasMaxLength(120);

                entity.Property(x => x.District)
                    .HasMaxLength(120);

                entity.Property(x => x.Neighborhood)
                    .HasMaxLength(120);
            });

            // Her binaya birden çok kat bağlanabilir.
            modelBuilder.Entity<AssetBuildingFloor>(entity =>
            {
                entity.ToTable("AssetBuildingFloors");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => new { x.AssetBuildingId, x.Name })
                    .IsUnique();

                entity.HasOne(x => x.Building)
                    .WithMany(x => x.Floors)
                    .HasForeignKey(x => x.AssetBuildingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Her kata birden çok oda bağlanır; oda mevcut SiteLocation'a bağlanabilir.
            modelBuilder.Entity<AssetBuildingRoom>(entity =>
            {
                entity.ToTable("AssetBuildingRooms");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => new { x.AssetBuildingFloorId, x.Name })
                    .IsUnique();

                entity.HasOne(x => x.Floor)
                    .WithMany(x => x.Rooms)
                    .HasForeignKey(x => x.AssetBuildingFloorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.SiteLocation)
                    .WithMany()
                    .HasForeignKey(x => x.SiteLocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region Demirbaş Tür-Marka ve Üst Varlık Fluent API

            modelBuilder.Entity<AssetKindBrand>(entity =>
            {
                entity.ToTable("AssetKindBrands");
                entity.HasKey(x => new { x.AssetKindId, x.BrandId });
                entity.HasOne<AssetKind>().WithMany()
                    .HasForeignKey(x => x.AssetKindId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Brand>().WithMany()
                    .HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AssetParentLink>(entity =>
            {
                entity.ToTable("AssetParentLinks");
                entity.HasKey(x => x.AssetId); // Bir demirbaşın yalnızca bir üst varlığı olur.
                entity.HasOne<Asset>().WithMany()
                    .HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Asset>().WithMany()
                    .HasForeignKey(x => x.ParentAssetId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(x => x.ParentAssetId);
                entity.ToTable(t => t.HasCheckConstraint("CK_AssetParentLinks_NotSelf", "[AssetId] <> [ParentAssetId]"));
            });

            #endregion

            #region Asset Kullanılan Organizasyon ve Oda Konumu (V7.1)

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasOne(x => x.UsingUnit)
                    .WithMany()
                    .HasForeignKey(x => x.UsingUnitId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.SiteLocation)
                    .WithMany()
                    .HasForeignKey(x => x.SiteLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.BarcodeNumber)
                    .HasMaxLength(100);
            });

            #endregion

            #region AssetAuditEvent Fluent API

            modelBuilder.Entity<AssetAuditEvent>(entity =>
            {
                entity.ToTable("AssetAuditEvents");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.EventType).HasMaxLength(120).IsRequired();
                entity.Property(x => x.FieldName).HasMaxLength(80).IsRequired();
                entity.Property(x => x.OldValue).HasMaxLength(2000);
                entity.Property(x => x.NewValue).HasMaxLength(2000);
                entity.Property(x => x.Description).HasMaxLength(4000).IsRequired();
                entity.HasIndex(x => new { x.AssetId, x.OccurredAt });
                entity.HasOne(x => x.Asset)
                    .WithMany()
                    .HasForeignKey(x => x.AssetId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.PerformedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Aynı demirbaşta aynı anda yalnız bir açık zimmet olabilir.
            // Mevcut verilerde birden çok açık zimmet varsa migration öncesi düzeltin.
            modelBuilder.Entity<AssetUser>()
                .HasIndex(x => x.AssetId)
                .HasDatabaseName("UX_AssetUsers_OneOpenAssignment")
                .IsUnique()
                .HasFilter("[EndDate] IS NULL");

            #endregion

            #region SQL Server Cascade Delete Prevention

            // SQL Server'daki multiple cascade paths
            // hatasini onlemek icin varsayilan Cascade
            // davranislarini NoAction olarak degistir.
            // Mevcut Restrict iliskileri korunur.

            foreach (var foreignKey in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(x => x.GetForeignKeys()))
            {
                if (foreignKey.DeleteBehavior == DeleteBehavior.Cascade)
                {
                    foreignKey.DeleteBehavior =
                        DeleteBehavior.NoAction;
                }
            }

            #endregion
        }

        #endregion
    }
}