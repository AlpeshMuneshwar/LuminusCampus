using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webappi.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRolesAndRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- 1. Ensure 'Academic Admin' Role exists
                -- We use INSERT IGNORE or Check existence. 
                -- To be safe, we insert only if name doesn't exist, allowing Auto-Inc to handle ID.
                
                INSERT INTO A_Roles (Name, NormalizedName, ConcurrencyStamp)
                SELECT 'Academic Admin', 'ACADEMIC ADMIN', UUID()
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM A_Roles WHERE Name = 'Academic Admin');

                -- 2. Map 'Academic Admin' Role to ALM Pages
                -- Dynamic lookup of RoleId to avoid hardcoded ID conflicts
                INSERT INTO ErpRoleDefaultRights (RoleId, PageId, IsActive, CreatedOn, IsDeleted)
                SELECT r.Id, p.Id, 1, UTC_TIMESTAMP(), 0
                FROM ErpPages p
                JOIN ErpModules m ON p.ModuleId = m.Id
                JOIN A_Roles r ON r.Name = 'Academic Admin'
                WHERE m.ModuleCode = 'ALM'
                  AND NOT EXISTS (SELECT 1 FROM ErpRoleDefaultRights WHERE RoleId = r.Id AND PageId = p.Id);
            ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
