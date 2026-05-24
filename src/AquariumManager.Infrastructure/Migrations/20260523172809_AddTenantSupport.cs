using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Species_SpeciesId",
                table: "SaleItems");

            migrationBuilder.DropIndex(
                name: "IX_AlertConfigs_AlertType",
                table: "AlertConfigs");

            migrationBuilder.DeleteData(
                table: "AlertConfigs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "WaterParameterLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TargetParameterRanges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Tanks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TankPhotos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SpeciesVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Species",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SaleItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "MortalityRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "MaintenanceLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "InventoryLots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "InventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "FertilizerPresets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "FertilizationLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AlertConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.Sql("SET IDENTITY_INSERT Tenants ON; INSERT INTO Tenants (Id, Name, ContactInfo) VALUES (1, 'Default Store', 'Default store created by migration'); SET IDENTITY_INSERT Tenants OFF;");

            migrationBuilder.Sql("UPDATE Users SET TenantId = 1");
            migrationBuilder.Sql("UPDATE Species SET TenantId = 1");
            migrationBuilder.Sql("UPDATE SpeciesVariants SET TenantId = 1");
            migrationBuilder.Sql("UPDATE Suppliers SET TenantId = 1");
            migrationBuilder.Sql("UPDATE InventoryLots SET TenantId = 1");
            migrationBuilder.Sql("UPDATE InventoryItems SET TenantId = 1");
            migrationBuilder.Sql("UPDATE MortalityRecords SET TenantId = 1");
            migrationBuilder.Sql("UPDATE Sales SET TenantId = 1");
            migrationBuilder.Sql("UPDATE SaleItems SET TenantId = 1");
            migrationBuilder.Sql("UPDATE Tanks SET TenantId = 1");
            migrationBuilder.Sql("UPDATE WaterParameterLogs SET TenantId = 1");
            migrationBuilder.Sql("UPDATE MaintenanceLogs SET TenantId = 1");
            migrationBuilder.Sql("UPDATE FertilizationLogs SET TenantId = 1");
            migrationBuilder.Sql("UPDATE FertilizerPresets SET TenantId = 1");
            migrationBuilder.Sql("UPDATE TankPhotos SET TenantId = 1");
            migrationBuilder.Sql("UPDATE TargetParameterRanges SET TenantId = 1");
            migrationBuilder.Sql("UPDATE AlertConfigs SET TenantId = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertConfigs_TenantId_AlertType",
                table: "AlertConfigs",
                columns: new[] { "TenantId", "AlertType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Species_SpeciesId",
                table: "SaleItems",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Species_SpeciesId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_AlertConfigs_TenantId_AlertType",
                table: "AlertConfigs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "WaterParameterLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TargetParameterRanges");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Tanks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TankPhotos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SpeciesVariants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MortalityRecords");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MaintenanceLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryLots");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FertilizerPresets");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FertilizationLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AlertConfigs");

            migrationBuilder.InsertData(
                table: "AlertConfigs",
                columns: new[] { "Id", "AlertType", "IsEnabled", "ThresholdValue" },
                values: new object[] { 1, "HighMortalityRate", true, 15m });

            migrationBuilder.CreateIndex(
                name: "IX_AlertConfigs_AlertType",
                table: "AlertConfigs",
                column: "AlertType",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Species_SpeciesId",
                table: "SaleItems",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
