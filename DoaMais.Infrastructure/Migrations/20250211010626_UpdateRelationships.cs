using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoaMais.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donors_Addresses_AddressId",
                table: "Donors");

            migrationBuilder.DropIndex(
                name: "IX_Donors_AddressId",
                table: "Donors");

            migrationBuilder.CreateIndex(
                name: "IX_Donors_AddressId",
                table: "Donors",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Donors_Addresses_AddressId",
                table: "Donors",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donors_Addresses_AddressId",
                table: "Donors");

            migrationBuilder.DropIndex(
                name: "IX_Donors_AddressId",
                table: "Donors");

            migrationBuilder.CreateIndex(
                name: "IX_Donors_AddressId",
                table: "Donors",
                column: "AddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Donors_Addresses_AddressId",
                table: "Donors",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
