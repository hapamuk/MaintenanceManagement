using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class yenieklenenlerioluşturmak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AssetUsers_AssetId",
                table: "AssetUsers");

            migrationBuilder.CreateTable(
                name: "AssetAuditEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAuditEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetAuditEvents_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAuditEvents_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_AssetUsers_OneOpenAssignment",
                table: "AssetUsers",
                column: "AssetId",
                unique: true,
                filter: "[EndDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAuditEvents_AssetId_OccurredAt",
                table: "AssetAuditEvents",
                columns: new[] { "AssetId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AssetAuditEvents_PerformedByUserId",
                table: "AssetAuditEvents",
                column: "PerformedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetAuditEvents");

            migrationBuilder.DropIndex(
                name: "UX_AssetUsers_OneOpenAssignment",
                table: "AssetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AssetUsers_AssetId",
                table: "AssetUsers",
                column: "AssetId");
        }
    }
}
