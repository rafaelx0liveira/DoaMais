using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoaMais.StockService.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeBloodStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RHFactor",
                table: "BloodStocks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BloodType",
                table: "BloodStocks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_BloodStocks_BloodType_RHFactor",
                table: "BloodStocks",
                columns: new[] { "BloodType", "RHFactor" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BloodStocks_BloodType_RHFactor",
                table: "BloodStocks");

            migrationBuilder.AlterColumn<string>(
                name: "RHFactor",
                table: "BloodStocks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "BloodType",
                table: "BloodStocks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
