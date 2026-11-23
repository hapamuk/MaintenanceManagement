using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class yenieklnensınıfıekle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetBuildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetBuildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetBuildingFloors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetBuildingId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetBuildingFloors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetBuildingFloors_AssetBuildings_AssetBuildingId",
                        column: x => x.AssetBuildingId,
                        principalTable: "AssetBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetBuildingRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetBuildingFloorId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SiteLocationId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetBuildingRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetBuildingRooms_AssetBuildingFloors_AssetBuildingFloorId",
                        column: x => x.AssetBuildingFloorId,
                        principalTable: "AssetBuildingFloors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetBuildingRooms_SiteLocations_SiteLocationId",
                        column: x => x.SiteLocationId,
                        principalTable: "SiteLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetBuildingFloors_AssetBuildingId_Name",
                table: "AssetBuildingFloors",
                columns: new[] { "AssetBuildingId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetBuildingRooms_AssetBuildingFloorId_Name",
                table: "AssetBuildingRooms",
                columns: new[] { "AssetBuildingFloorId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetBuildingRooms_SiteLocationId",
                table: "AssetBuildingRooms",
                column: "SiteLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetBuildingRooms");

            migrationBuilder.DropTable(
                name: "AssetBuildingFloors");

            migrationBuilder.DropTable(
                name: "AssetBuildings");
        }
    }
}
