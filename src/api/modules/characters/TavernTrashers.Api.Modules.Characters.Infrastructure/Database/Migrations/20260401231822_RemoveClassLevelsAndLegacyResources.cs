using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClassLevelsAndLegacyResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_character_resources_characters_character_id",
                schema: "characters",
                table: "character_resources");

            migrationBuilder.DropForeignKey(
                name: "fk_class_levels_characters_character_id",
                schema: "characters",
                table: "class_levels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "fk_character_resources_characters_character_id",
                schema: "characters",
                table: "character_resources",
                column: "character_id",
                principalSchema: "characters",
                principalTable: "characters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_class_levels_characters_character_id",
                schema: "characters",
                table: "class_levels",
                column: "character_id",
                principalSchema: "characters",
                principalTable: "characters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
