using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TariffApp.Migrations
{
    /// <inheritdoc />
    public partial class config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefaultCurrency = table.Column<int>(type: "int", nullable: false),
                    MaxFeeLimit = table.Column<double>(type: "float", nullable: false),
                    PosFixedFee = table.Column<double>(type: "float", nullable: false),
                    PosPercentFee = table.Column<double>(type: "float", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Configuration",
                columns: new[] { "Id", "DefaultCurrency", "LastUpdated", "MaxFeeLimit", "PosFixedFee", "PosPercentFee" },
                values: new object[] { 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 120.0, 0.20000000000000001, 0.002 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");
        }
    }
}
