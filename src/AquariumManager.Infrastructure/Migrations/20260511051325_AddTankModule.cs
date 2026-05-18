using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTankModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FertilizerPresets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FertilizerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultDoseAmount = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    DefaultDoseUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NitratePerDose = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    PhosphatePerDose = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    PotassiumPerDose = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    IronPerDose = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerPresets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerPresets_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeLiters = table.Column<decimal>(type: "decimal(8,1)", nullable: false),
                    TankType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Substrate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Co2Injection = table.Column<bool>(type: "bit", nullable: false),
                    LightDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FilterDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HeaterSetpointCelsius = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tanks_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FertilizationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TankId = table.Column<int>(type: "int", nullable: false),
                    FertilizerPresetId = table.Column<int>(type: "int", nullable: true),
                    DosedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoseAmount = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    DoseUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FertilizerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstimatedNitratePpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    EstimatedPhosphatePpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    EstimatedPotassiumPpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    EstimatedIronPpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    IsScheduled = table.Column<bool>(type: "bit", nullable: false),
                    IsAdjustment = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizationLogs_FertilizerPresets_FertilizerPresetId",
                        column: x => x.FertilizerPresetId,
                        principalTable: "FertilizerPresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FertilizationLogs_Tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "Tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TankId = table.Column<int>(type: "int", nullable: false),
                    MaintenanceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WaterChangePercent = table.Column<int>(type: "int", nullable: true),
                    WaterChangeLiters = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReminderFrequencyDays = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceLogs_Tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "Tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TankPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TankId = table.Column<int>(type: "int", nullable: false),
                    TakenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LinkedLogType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LinkedLogId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TankPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TankPhotos_Tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "Tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TargetParameterRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TankId = table.Column<int>(type: "int", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(8,3)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(8,3)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetParameterRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetParameterRanges_Tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "Tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterParameterLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TankId = table.Column<int>(type: "int", nullable: false),
                    MeasuredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    pH = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    TemperatureCelsius = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    AmmoniaPpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    NitritePpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    NitratePpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    PhosphatePpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    PotassiumPpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    IronPpm = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    GeneralHardness = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    CarbonateHardness = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    TdsPpm = table.Column<int>(type: "int", nullable: true),
                    Co2Ppm = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    SalinityPpt = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterParameterLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterParameterLogs_Tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "Tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizationLogs_FertilizerPresetId",
                table: "FertilizationLogs",
                column: "FertilizerPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizationLogs_TankId",
                table: "FertilizationLogs",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerPresets_OwnerUserId",
                table: "FertilizerPresets",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceLogs_TankId",
                table: "MaintenanceLogs",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_TankPhotos_TankId",
                table: "TankPhotos",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_Tanks_OwnerUserId",
                table: "Tanks",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetParameterRanges_TankId",
                table: "TargetParameterRanges",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterParameterLogs_TankId",
                table: "WaterParameterLogs",
                column: "TankId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FertilizationLogs");

            migrationBuilder.DropTable(
                name: "MaintenanceLogs");

            migrationBuilder.DropTable(
                name: "TankPhotos");

            migrationBuilder.DropTable(
                name: "TargetParameterRanges");

            migrationBuilder.DropTable(
                name: "WaterParameterLogs");

            migrationBuilder.DropTable(
                name: "FertilizerPresets");

            migrationBuilder.DropTable(
                name: "Tanks");
        }
    }
}
