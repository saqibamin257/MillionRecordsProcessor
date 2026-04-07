using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Records.Migrations
{
    /// <inheritdoc />
    public partial class AddStagingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE TABLE Records_Staging (
            Id UNIQUEIDENTIFIER,
            Name NVARCHAR(100),
            Email NVARCHAR(200),
            Status BIT,
            ProcessedAt DATETIME NULL
           );
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE Records_Staging");
        }
    }
}
