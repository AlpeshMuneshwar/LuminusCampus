using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webappi.Migrations
{
    /// <inheritdoc />
    public partial class AddHierarchyAndDesignation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DesignationId",
                table: "A_Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentUserId",
                table: "A_Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ErpDesignations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErpDesignations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_A_Users_DesignationId",
                table: "A_Users",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_A_Users_ParentUserId",
                table: "A_Users",
                column: "ParentUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_A_Users_A_Users_ParentUserId",
                table: "A_Users",
                column: "ParentUserId",
                principalTable: "A_Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_A_Users_ErpDesignations_DesignationId",
                table: "A_Users",
                column: "DesignationId",
                principalTable: "ErpDesignations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_A_Users_A_Users_ParentUserId",
                table: "A_Users");

            migrationBuilder.DropForeignKey(
                name: "FK_A_Users_ErpDesignations_DesignationId",
                table: "A_Users");

            migrationBuilder.DropTable(
                name: "ErpDesignations");

            migrationBuilder.DropIndex(
                name: "IX_A_Users_DesignationId",
                table: "A_Users");

            migrationBuilder.DropIndex(
                name: "IX_A_Users_ParentUserId",
                table: "A_Users");

            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "A_Users");

            migrationBuilder.DropColumn(
                name: "ParentUserId",
                table: "A_Users");
        }
    }
}
