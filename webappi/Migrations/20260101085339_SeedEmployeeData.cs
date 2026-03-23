using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webappi.Migrations
{
    /// <inheritdoc />
    public partial class SeedEmployeeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ErpModules (ModuleName, ModuleCode, IsActive, CreatedOn, IsDeleted, LastModifiedOn) 
                SELECT 'Employee Management', 'EMP', 1, UTC_TIMESTAMP(), 0, UTC_TIMESTAMP()
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM ErpModules WHERE ModuleName = 'Employee Management');
                
                INSERT INTO ErpPages (ModuleId, PageName, PageUrl, Description, IsActive, CreatedOn, IsDeleted, LastModifiedOn, UrlToken)
                SELECT (SELECT Id FROM ErpModules WHERE ModuleName = 'Employee Management' LIMIT 1), 'Departments', '/Department', 'Manage Departments', 1, UTC_TIMESTAMP(), 0, UTC_TIMESTAMP(), REPLACE(UUID(), '-', '')
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM ErpPages WHERE PageUrl = '/Department');

                INSERT INTO ErpPages (ModuleId, PageName, PageUrl, Description, IsActive, CreatedOn, IsDeleted, LastModifiedOn, UrlToken)
                SELECT (SELECT Id FROM ErpModules WHERE ModuleName = 'Employee Management' LIMIT 1), 'Employees', '/Employee', 'Manage Employees', 1, UTC_TIMESTAMP(), 0, UTC_TIMESTAMP(), REPLACE(UUID(), '-', '')
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM ErpPages WHERE PageUrl = '/Employee');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
