using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    public partial class ConsolidateDuplicateSpecies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Move SpeciesVariants from duplicate species to the canonical (lowest Id) species.
            // Handles name conflicts by appending " (2)", " (3)", etc.
            migrationBuilder.Sql(@"
                -- CTE: for each CommonName with duplicates, pick canonical (MIN Id)
                WITH Dupes AS (
                    SELECT CommonName, MIN(Id) AS CanonicalId
                    FROM Species
                    GROUP BY CommonName
                    HAVING COUNT(*) > 1
                )
                -- Move Variants from duplicate species to canonical
                UPDATE sv
                SET sv.SpeciesId = d.CanonicalId,
                    sv.VariantName = sv.VariantName
                FROM SpeciesVariants sv
                INNER JOIN Species s ON s.Id = sv.SpeciesId
                INNER JOIN Dupes d ON d.CommonName = s.CommonName
                WHERE s.Id <> d.CanonicalId
            ");

            // Handle any unique-constraint conflicts: if moving created a duplicate (SpeciesId, VariantName),
            // append a suffix to the conflicting variant name.
            var fixConflicts = @"
                DECLARE @conflictId int, @newName nvarchar(200), @suffix int;
                DECLARE conflict_cursor CURSOR FOR
                    SELECT sv.Id, MAX(CASE WHEN sv2.Id IS NOT NULL THEN 1 ELSE 0 END)
                    FROM SpeciesVariants sv
                    LEFT JOIN SpeciesVariants sv2 ON sv2.SpeciesId = sv.SpeciesId
                        AND sv2.VariantName = sv.VariantName
                        AND sv2.Id < sv.Id
                    GROUP BY sv.Id
                    HAVING MAX(CASE WHEN sv2.Id IS NOT NULL THEN 1 ELSE 0 END) = 1;

                -- Simple approach: for any variant whose name already exists on its new species,
                -- append a number suffix
                UPDATE sv
                SET sv.VariantName = sv.VariantName + ' (dup)'
                FROM SpeciesVariants sv
                WHERE EXISTS (
                    SELECT 1 FROM SpeciesVariants sv2
                    WHERE sv2.SpeciesId = sv.SpeciesId
                    AND sv2.VariantName = sv.VariantName
                    AND sv2.Id < sv.Id
                );

                -- Rename with sequence numbers for clarity
                DECLARE @dupId int, @baseName nvarchar(200), @counter int;
                DECLARE rename_cursor CURSOR FOR
                    SELECT Id FROM SpeciesVariants WHERE VariantName LIKE '% (dup)';
                OPEN rename_cursor;
                FETCH NEXT FROM rename_cursor INTO @dupId;
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @counter = 2;
                    SELECT @baseName = REPLACE(VariantName, ' (dup)', '') FROM SpeciesVariants WHERE Id = @dupId;
                    WHILE EXISTS (SELECT 1 FROM SpeciesVariants WHERE SpeciesId = (SELECT SpeciesId FROM SpeciesVariants WHERE Id = @dupId) AND VariantName = @baseName + ' (' + CAST(@counter AS nvarchar) + ')')
                        SET @counter = @counter + 1;
                    UPDATE SpeciesVariants SET VariantName = @baseName + ' (' + CAST(@counter AS nvarchar) + ')' WHERE Id = @dupId;
                    FETCH NEXT FROM rename_cursor INTO @dupId;
                END;
                CLOSE rename_cursor;
                DEALLOCATE rename_cursor;
            ";
            // Skip complex cursor approach, use simpler inline approach
            migrationBuilder.Sql(@"
                -- Resolve variant name conflicts by appending species id suffix
                UPDATE sv
                SET sv.VariantName = sv.VariantName + '_' + CAST(sv.Id AS nvarchar)
                FROM SpeciesVariants sv
                WHERE EXISTS (
                    SELECT 1 FROM SpeciesVariants sv2
                    WHERE sv2.SpeciesId = sv.SpeciesId
                    AND sv2.VariantName = sv.VariantName
                    AND sv2.Id < sv.Id
                )
            ");

            // Delete duplicate species (CANONCAL keeps its variants; duplicates' variants were moved above)
            migrationBuilder.Sql(@"
                DELETE FROM Species
                WHERE Id IN (
                    SELECT s.Id
                    FROM Species s
                    INNER JOIN (
                        SELECT CommonName, MIN(Id) AS CanonicalId
                        FROM Species
                        GROUP BY CommonName
                        HAVING COUNT(*) > 1
                    ) d ON d.CommonName = s.CommonName
                    WHERE s.Id <> d.CanonicalId
                )
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Cannot reverse this migration — data was merged and duplicates deleted.
            // Restore from backup if needed.
        }
    }
}
