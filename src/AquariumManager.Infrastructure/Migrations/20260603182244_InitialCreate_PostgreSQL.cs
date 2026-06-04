using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_PostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    AlertType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    CommonName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ScientificName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Variety = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MinPH = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    MaxPH = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    MinTemperature = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    MaxTemperature = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    CompatibilityNotes = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactInfo = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    VariantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeciesVariants_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesVariantId = table.Column<int>(type: "integer", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InitialQuantity = table.Column<int>(type: "integer", nullable: false),
                    DeadOnArrival = table.Column<int>(type: "integer", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    BatchNumber = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    TotalMortality = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryLots_SpeciesVariants_SpeciesVariantId",
                        column: x => x.SpeciesVariantId,
                        principalTable: "SpeciesVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLots_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesVariantId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleItems_SpeciesVariants_SpeciesVariantId",
                        column: x => x.SpeciesVariantId,
                        principalTable: "SpeciesVariants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaleItems_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FertilizerPresets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerUserId = table.Column<int>(type: "integer", nullable: true),
                    FertilizerType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DefaultDoseAmount = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    DefaultDoseUnit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NitratePerDose = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    PhosphatePerDose = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    PotassiumPerDose = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    IronPerDose = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    OwnerUserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeLiters = table.Column<decimal>(type: "numeric(8,1)", nullable: false),
                    TankType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Substrate = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Co2Injection = table.Column<bool>(type: "boolean", nullable: false),
                    LightDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FilterDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HeaterSetpointCelsius = table.Column<decimal>(type: "numeric(4,1)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "MortalityRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    InventoryLotId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Cause = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MortalityRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MortalityRecords_InventoryLots_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FertilizationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TankId = table.Column<int>(type: "integer", nullable: false),
                    FertilizerPresetId = table.Column<int>(type: "integer", nullable: true),
                    DosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DoseAmount = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    DoseUnit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FertilizerType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EstimatedNitratePpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    EstimatedPhosphatePpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    EstimatedPotassiumPpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    EstimatedIronPpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    IsScheduled = table.Column<bool>(type: "boolean", nullable: false),
                    IsAdjustment = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TankId = table.Column<int>(type: "integer", nullable: false),
                    MaintenanceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WaterChangePercent = table.Column<int>(type: "integer", nullable: true),
                    WaterChangeLiters = table.Column<decimal>(type: "numeric(8,2)", nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReminderFrequencyDays = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TankId = table.Column<int>(type: "integer", nullable: false),
                    TakenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LinkedLogType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LinkedLogId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TankId = table.Column<int>(type: "integer", nullable: false),
                    ParameterName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MinValue = table.Column<decimal>(type: "numeric(8,3)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "numeric(8,3)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TankId = table.Column<int>(type: "integer", nullable: false),
                    MeasuredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    pH = table.Column<decimal>(type: "numeric(4,2)", nullable: true),
                    TemperatureCelsius = table.Column<decimal>(type: "numeric(4,1)", nullable: true),
                    AmmoniaPpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    NitritePpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    NitratePpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    PhosphatePpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    PotassiumPpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    IronPpm = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    GeneralHardness = table.Column<decimal>(type: "numeric(5,1)", nullable: true),
                    CarbonateHardness = table.Column<decimal>(type: "numeric(5,1)", nullable: true),
                    TdsPpm = table.Column<int>(type: "integer", nullable: true),
                    Co2Ppm = table.Column<decimal>(type: "numeric(5,1)", nullable: true),
                    SalinityPpt = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.InsertData(
                table: "AlertConfigs",
                columns: new[] { "Id", "AlertType", "IsEnabled", "TenantId", "ThresholdValue" },
                values: new object[] { 1, "HighMortalityRate", true, 1, 15m });

            migrationBuilder.CreateIndex(
                name: "IX_AlertConfigs_TenantId_AlertType",
                table: "AlertConfigs",
                columns: new[] { "TenantId", "AlertType" },
                unique: true);

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
                name: "IX_InventoryItems_SpeciesId",
                table: "InventoryItems",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_SpeciesVariantId",
                table: "InventoryLots",
                column: "SpeciesVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_SupplierId",
                table: "InventoryLots",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceLogs_TankId",
                table: "MaintenanceLogs",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_MortalityRecords_InventoryLotId",
                table: "MortalityRecords",
                column: "InventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SpeciesId",
                table: "SaleItems",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SpeciesVariantId",
                table: "SaleItems",
                column: "SpeciesVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesVariants_SpeciesId_VariantName",
                table: "SpeciesVariants",
                columns: new[] { "SpeciesId", "VariantName" },
                unique: true);

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
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterParameterLogs_TankId",
                table: "WaterParameterLogs",
                column: "TankId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertConfigs");

            migrationBuilder.DropTable(
                name: "FertilizationLogs");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "MaintenanceLogs");

            migrationBuilder.DropTable(
                name: "MortalityRecords");

            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "TankPhotos");

            migrationBuilder.DropTable(
                name: "TargetParameterRanges");

            migrationBuilder.DropTable(
                name: "WaterParameterLogs");

            migrationBuilder.DropTable(
                name: "FertilizerPresets");

            migrationBuilder.DropTable(
                name: "InventoryLots");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Tanks");

            migrationBuilder.DropTable(
                name: "SpeciesVariants");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
