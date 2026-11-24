using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class yenieklnensınıfıeklemek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetKindBrands",
                columns: table => new
                {
                    AssetKindId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetKindBrands", x => new { x.AssetKindId, x.BrandId });
                    table.ForeignKey(
                        name: "FK_AssetKindBrands_AssetKinds_AssetKindId",
                        column: x => x.AssetKindId,
                        principalTable: "AssetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetKindBrands_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetParentLinks",
                columns: table => new
                {
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    ParentAssetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetParentLinks", x => x.AssetId);
                    table.CheckConstraint("CK_AssetParentLinks_NotSelf", "[AssetId] <> [ParentAssetId]");
                    table.ForeignKey(
                        name: "FK_AssetParentLinks_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetParentLinks_Assets_ParentAssetId",
                        column: x => x.ParentAssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetKindBrands_BrandId",
                table: "AssetKindBrands",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParentLinks_ParentAssetId",
                table: "AssetParentLinks",
                column: "ParentAssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetKindBrands");

            migrationBuilder.DropTable(
                name: "AssetParentLinks");
        }
    }
}
