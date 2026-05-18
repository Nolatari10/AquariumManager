using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeSpeciesOptionalAndAddSpeciesName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLots_Species_SpeciesId",
                table: "InventoryLots");

            migrationBuilder.AlterColumn<int>(
                name: "SpeciesId",
                table: "InventoryLots",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SpeciesName",
                table: "InventoryLots",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE il
                SET il.SpeciesName = ISNULL(s.CommonName, N'')
                FROM InventoryLots il
                INNER JOIN Species s ON s.Id = il.SpeciesId;
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLots_Species_SpeciesId",
                table: "InventoryLots",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLots_Species_SpeciesId",
                table: "InventoryLots");

            migrationBuilder.DropColumn(
                name: "SpeciesName",
                table: "InventoryLots");

            migrationBuilder.AlterColumn<int>(
                name: "SpeciesId",
                table: "InventoryLots",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLots_Species_SpeciesId",
                table: "InventoryLots",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
