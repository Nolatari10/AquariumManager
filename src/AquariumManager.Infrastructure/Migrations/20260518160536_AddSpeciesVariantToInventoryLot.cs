using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    public partial class AddSpeciesVariantToInventoryLot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create SpeciesVariants table (skip if already exists from prior attempt)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SpeciesVariants')
                BEGIN
                    CREATE TABLE SpeciesVariants (
                        Id int NOT NULL IDENTITY(1,1),
                        SpeciesId int NOT NULL,
                        VariantName nvarchar(200) NOT NULL,
                        Notes nvarchar(max) NULL,
                        ImageUrl nvarchar(max) NULL,
                        CONSTRAINT PK_SpeciesVariants PRIMARY KEY (Id),
                        CONSTRAINT FK_SpeciesVariants_Species_SpeciesId FOREIGN KEY (SpeciesId) REFERENCES Species(Id) ON DELETE CASCADE
                    );

                    CREATE UNIQUE INDEX IX_SpeciesVariants_SpeciesId_VariantName ON SpeciesVariants (SpeciesId, VariantName);
                END
            ");

            // 2. Create Standard variant for every existing species (skip if already exists)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM SpeciesVariants WHERE VariantName = 'Standard')
                BEGIN
                    INSERT INTO SpeciesVariants (SpeciesId, VariantName)
                    SELECT Id, 'Standard' FROM Species
                    WHERE Id NOT IN (SELECT SpeciesId FROM SpeciesVariants WHERE VariantName = 'Standard');
                END
            ");

            // 3. Add SpeciesVariantId to InventoryLots (skip if already exists)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InventoryLots') AND name = 'SpeciesVariantId')
                BEGIN
                    ALTER TABLE InventoryLots ADD SpeciesVariantId int NULL;
                END
            ");

            // 4. Map existing lots to their species Standard variant
            migrationBuilder.Sql(@"
                UPDATE il
                SET il.SpeciesVariantId = sv.Id
                FROM InventoryLots il
                INNER JOIN SpeciesVariants sv ON sv.SpeciesId = il.SpeciesId AND sv.VariantName = 'Standard'
                WHERE il.SpeciesId IS NOT NULL AND il.SpeciesVariantId IS NULL
            ");

            // 5. Fallback: assign any remaining NULL lots to first available variant
            migrationBuilder.Sql(@"
                UPDATE InventoryLots
                SET SpeciesVariantId = (SELECT TOP 1 Id FROM SpeciesVariants ORDER BY Id)
                WHERE SpeciesVariantId IS NULL
            ");

            // 6. Make SpeciesVariantId non-nullable
            migrationBuilder.Sql(@"
                DECLARE @nullable bit;
                SELECT @nullable = is_nullable FROM sys.columns WHERE object_id = OBJECT_ID('InventoryLots') AND name = 'SpeciesVariantId';
                IF @nullable = 1
                BEGIN
                    ALTER TABLE InventoryLots ALTER COLUMN SpeciesVariantId int NOT NULL;
                END
            ");

            // 7. Drop old FK, index, and columns from InventoryLots
            var dropIndexSql = @"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InventoryLots_SpeciesId' AND object_id = OBJECT_ID('InventoryLots'))
                    DROP INDEX IX_InventoryLots_SpeciesId ON InventoryLots;
            ";
            migrationBuilder.Sql(dropIndexSql);

            var dropFkSql = @"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InventoryLots_Species_SpeciesId')
                    ALTER TABLE InventoryLots DROP CONSTRAINT FK_InventoryLots_Species_SpeciesId;
            ";
            migrationBuilder.Sql(dropFkSql);

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InventoryLots') AND name = 'SpeciesId')
                BEGIN
                    -- Drop default constraint if any
                    DECLARE @defName nvarchar(200);
                    SELECT @defName = name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('InventoryLots') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('InventoryLots'), 'SpeciesId', 'ColumnId');
                    IF @defName IS NOT NULL
                        EXEC('ALTER TABLE InventoryLots DROP CONSTRAINT [' + @defName + ']');
                    ALTER TABLE InventoryLots DROP COLUMN SpeciesId;
                END
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InventoryLots') AND name = 'SpeciesName')
                BEGIN
                    DECLARE @defName2 nvarchar(200);
                    SELECT @defName2 = name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('InventoryLots') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('InventoryLots'), 'SpeciesName', 'ColumnId');
                    IF @defName2 IS NOT NULL
                        EXEC('ALTER TABLE InventoryLots DROP CONSTRAINT [' + @defName2 + ']');
                    ALTER TABLE InventoryLots DROP COLUMN SpeciesName;
                END
            ");

            // 8. Add FK from InventoryLots -> SpeciesVariants (skip if exists)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InventoryLots_SpeciesVariantId' AND object_id = OBJECT_ID('InventoryLots'))
                    CREATE INDEX IX_InventoryLots_SpeciesVariantId ON InventoryLots (SpeciesVariantId);

                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InventoryLots_SpeciesVariants_SpeciesVariantId')
                    ALTER TABLE InventoryLots ADD CONSTRAINT FK_InventoryLots_SpeciesVariants_SpeciesVariantId
                    FOREIGN KEY (SpeciesVariantId) REFERENCES SpeciesVariants(Id);
            ");

            // 9. Add SpeciesVariantId to SaleItems (skip if exists)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SaleItems') AND name = 'SpeciesVariantId')
                    ALTER TABLE SaleItems ADD SpeciesVariantId int NULL;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SaleItems_SpeciesVariantId' AND object_id = OBJECT_ID('SaleItems'))
                    CREATE INDEX IX_SaleItems_SpeciesVariantId ON SaleItems (SpeciesVariantId);

                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SaleItems_SpeciesVariants_SpeciesVariantId')
                    ALTER TABLE SaleItems ADD CONSTRAINT FK_SaleItems_SpeciesVariants_SpeciesVariantId
                    FOREIGN KEY (SpeciesVariantId) REFERENCES SpeciesVariants(Id) ON DELETE NO ACTION;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop SaleItems FK/index/column
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SaleItems_SpeciesVariants_SpeciesVariantId')
                    ALTER TABLE SaleItems DROP CONSTRAINT FK_SaleItems_SpeciesVariants_SpeciesVariantId;
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SaleItems_SpeciesVariantId' AND object_id = OBJECT_ID('SaleItems'))
                    DROP INDEX IX_SaleItems_SpeciesVariantId ON SaleItems;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SaleItems') AND name = 'SpeciesVariantId')
                    ALTER TABLE SaleItems DROP COLUMN SpeciesVariantId;
            ");

            // Drop InventoryLots FK/index/column
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InventoryLots_SpeciesVariants_SpeciesVariantId')
                    ALTER TABLE InventoryLots DROP CONSTRAINT FK_InventoryLots_SpeciesVariants_SpeciesVariantId;
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InventoryLots_SpeciesVariantId' AND object_id = OBJECT_ID('InventoryLots'))
                    DROP INDEX IX_InventoryLots_SpeciesVariantId ON InventoryLots;
            ");

            // Re-add SpeciesId and SpeciesName columns
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InventoryLots') AND name = 'SpeciesId')
                    ALTER TABLE InventoryLots ADD SpeciesId int NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InventoryLots') AND name = 'SpeciesName')
                    ALTER TABLE InventoryLots ADD SpeciesName nvarchar(300) NOT NULL DEFAULT '';
            ");

            // Restore SpeciesId from SpeciesVariant
            migrationBuilder.Sql(@"
                UPDATE il
                SET il.SpeciesId = sv.SpeciesId,
                    il.SpeciesName = ISNULL(s.CommonName, N'')
                FROM InventoryLots il
                INNER JOIN SpeciesVariants sv ON sv.Id = il.SpeciesVariantId
                INNER JOIN Species s ON s.Id = sv.SpeciesId
            ");

            // Drop SpeciesVariantId from InventoryLots
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InventoryLots') AND name = 'SpeciesVariantId')
                    ALTER TABLE InventoryLots DROP COLUMN SpeciesVariantId;
            ");

            // Re-add FK/index
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InventoryLots') AND name = 'SpeciesId')
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InventoryLots_SpeciesId' AND object_id = OBJECT_ID('InventoryLots'))
                        CREATE INDEX IX_InventoryLots_SpeciesId ON InventoryLots (SpeciesId);
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InventoryLots_Species_SpeciesId')
                        ALTER TABLE InventoryLots ADD CONSTRAINT FK_InventoryLots_Species_SpeciesId
                        FOREIGN KEY (SpeciesId) REFERENCES Species(Id) ON DELETE SET NULL;
                END
            ");

            // Drop SpeciesVariants table and its index
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SpeciesVariants_SpeciesId_VariantName' AND object_id = OBJECT_ID('SpeciesVariants'))
                    DROP INDEX IX_SpeciesVariants_SpeciesId_VariantName ON SpeciesVariants;
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SpeciesVariants')
                BEGIN
                    ALTER TABLE SpeciesVariants DROP CONSTRAINT FK_SpeciesVariants_Species_SpeciesId;
                    DROP TABLE SpeciesVariants;
                END
            ");
        }
    }
}
