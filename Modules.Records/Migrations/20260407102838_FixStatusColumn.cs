using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Records.Migrations
{
    /// <inheritdoc />
    public partial class FixStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE Records_Staging
            ALTER COLUMN Status NVARCHAR(50)
          ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE Records_Staging
            ALTER COLUMN Status BIT
         ");
        }
    }
}
