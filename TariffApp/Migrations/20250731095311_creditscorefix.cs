using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TariffApp.Migrations
{
    /// <inheritdoc />
    public partial class creditscorefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "creditScoreDiscount",
                table: "Configuration",
                newName: "CreditScoreDiscount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreditScoreDiscount",
                table: "Configuration",
                newName: "creditScoreDiscount");
        }
    }
}
