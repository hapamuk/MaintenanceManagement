using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class yenieklenenlerioluştur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BarcodeNumber",
                table: "Assets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteLocationId",
                table: "Assets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsingUnitId",
                table: "Assets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SiteLocationId",
                table: "Assets",
                column: "SiteLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_UsingUnitId",
                table: "Assets",
                column: "UsingUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_SiteLocations_SiteLocationId",
                table: "Assets",
                column: "SiteLocationId",
                principalTable: "SiteLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Units_UsingUnitId",
                table: "Assets",
                column: "UsingUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_SiteLocations_SiteLocationId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Units_UsingUnitId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_SiteLocationId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_UsingUnitId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "BarcodeNumber",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "SiteLocationId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "UsingUnitId",
                table: "Assets");
        }
    }
}
