using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoaMais.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovingBloodStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodStocks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BloodStocks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BloodType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuantityML = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RHFactor = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodStocks", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodStocks_BloodType_RHFactor",
                table: "BloodStocks",
                columns: new[] { "BloodType", "RHFactor" },
                unique: true);
        }
    }
}
