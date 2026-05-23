using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertConfigs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AlertConfigs",
                columns: new[] { "Id", "AlertType", "IsEnabled", "ThresholdValue" },
                values: new object[] { 1, "HighMortalityRate", true, 15m });

            migrationBuilder.CreateIndex(
                name: "IX_AlertConfigs_AlertType",
                table: "AlertConfigs",
                column: "AlertType",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertConfigs");
        }
    }
}
