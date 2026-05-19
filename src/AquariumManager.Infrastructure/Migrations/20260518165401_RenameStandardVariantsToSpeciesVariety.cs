using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquariumManager.Infrastructure.Migrations
{
    public partial class RenameStandardVariantsToSpeciesVariety : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE sv
                SET sv.VariantName = s.Variety
                FROM SpeciesVariants sv
                INNER JOIN Species s ON s.Id = sv.SpeciesId
                WHERE sv.VariantName = 'Standard'
                  AND s.Variety IS NOT NULL
                  AND LTRIM(RTRIM(s.Variety)) <> ''
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE sv
                SET sv.VariantName = 'Standard'
                FROM SpeciesVariants sv
                INNER JOIN Species s ON s.Id = sv.SpeciesId
                WHERE s.Variety IS NOT NULL
                  AND LTRIM(RTRIM(s.Variety)) <> ''
                  AND sv.VariantName = s.Variety
            ");
        }
    }
}
