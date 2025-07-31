using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TariffApp.Migrations
{
    /// <inheritdoc />
    public partial class creditScoreConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "creditScoreDiscount",
                table: "Configuration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 1,
                column: "creditScoreDiscount",
                value: 400);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creditScoreDiscount",
                table: "Configuration");
        }
    }
}
