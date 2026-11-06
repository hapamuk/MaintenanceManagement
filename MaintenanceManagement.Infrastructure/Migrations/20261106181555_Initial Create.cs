using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_OrganizationTypes_OrganizationTypeId",
                table: "Units");

            migrationBuilder.DropTable(
                name: "OrganizationTypes");

            migrationBuilder.DropIndex(
                name: "IX_Units_OrganizationTypeId",
                table: "Units");

            migrationBuilder.RenameColumn(
                name: "OrganizationTypeId",
                table: "Units",
                newName: "OrganizationTypeEnum");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrganizationTypeEnum",
                table: "Units",
                newName: "OrganizationTypeId");

            migrationBuilder.CreateTable(
                name: "OrganizationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Units_OrganizationTypeId",
                table: "Units",
                column: "OrganizationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_OrganizationTypes_OrganizationTypeId",
                table: "Units",
                column: "OrganizationTypeId",
                principalTable: "OrganizationTypes",
                principalColumn: "Id");
        }
    }
}
