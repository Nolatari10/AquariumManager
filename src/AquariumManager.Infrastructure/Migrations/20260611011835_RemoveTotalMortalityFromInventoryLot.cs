using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTotalMortalityFromInventoryLot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalMortality",
                table: "InventoryLots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalMortality",
                table: "InventoryLots",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
