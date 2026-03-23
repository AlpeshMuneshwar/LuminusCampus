using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webappi.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlToken",
                table: "ErpPages",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlToken",
                table: "ErpPages");
        }
    }
}
