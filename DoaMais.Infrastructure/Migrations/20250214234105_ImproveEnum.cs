using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoaMais.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Donors",
                newName: "BiologicalSex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BiologicalSex",
                table: "Donors",
                newName: "Gender");
        }
    }
}
