using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webappi.Migrations
{
    /// <inheritdoc />
    public partial class SeedAcademicData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Ensure ALM Module Exists (It should from Program.cs, but safe check)
                INSERT INTO ErpModules (ModuleName, ModuleCode, IsActive, CreatedOn, IsDeleted, LastModifiedOn) 
                SELECT 'Academic & Learning', 'ALM', 1, UTC_TIMESTAMP(), 0, UTC_TIMESTAMP()
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM ErpModules WHERE ModuleCode = 'ALM');
                
                -- Seed Pages
                INSERT INTO ErpPages (ModuleId, PageName, PageUrl, Description, IsActive, CreatedOn, IsDeleted, LastModifiedOn, UrlToken)
                SELECT (SELECT Id FROM ErpModules WHERE ModuleCode = 'ALM' LIMIT 1), 'Programs', '/ALM/Program', 'Manage Degrees/Programs', 1, UTC_TIMESTAMP(), 0, UTC_TIMESTAMP(), REPLACE(UUID(), '-', '')
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM ErpPages WHERE PageUrl = '/ALM/Program');

                INSERT INTO ErpPages (ModuleId, PageName, PageUrl, Description, IsActive, CreatedOn, IsDeleted, LastModifiedOn, UrlToken)
                SELECT (SELECT Id FROM ErpModules WHERE ModuleCode = 'ALM' LIMIT 1), 'Branches', '/ALM/Branch', 'Manage Streams/Branches', 1, UTC_TIMESTAMP(), 0, UTC_TIMESTAMP(), REPLACE(UUID(), '-', '')
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM ErpPages WHERE PageUrl = '/ALM/Branch');

                INSERT INTO ErpPages (ModuleId, PageName, PageUrl, Description, IsActive, CreatedOn, IsDeleted, LastModifiedOn, UrlToken)
                SELECT (SELECT Id FROM ErpModules WHERE ModuleCode = 'ALM' LIMIT 1), 'Academic Years', '/ALM/AcademicYear', 'Manage Batches/Sessions', 1, UTC_TIMESTAMP(), 0, UTC_TIMESTAMP(), REPLACE(UUID(), '-', '')
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM ErpPages WHERE PageUrl = '/ALM/AcademicYear');
                
                INSERT INTO ErpPages (ModuleId, PageName, PageUrl, Description, IsActive, CreatedOn, IsDeleted, LastModifiedOn, UrlToken)
                SELECT (SELECT Id FROM ErpModules WHERE ModuleCode = 'ALM' LIMIT 1), 'Semesters', '/ALM/Semester', 'Manage Terms/Semesters', 1, UTC_TIMESTAMP(), 0, UTC_TIMESTAMP(), REPLACE(UUID(), '-', '')
                FROM dual
                WHERE NOT EXISTS (SELECT 1 FROM ErpPages WHERE PageUrl = '/ALM/Semester');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
