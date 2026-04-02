using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSpellSlotPools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "spell_slot_pools",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spell_slot_pools", x => x.id);
                    table.ForeignKey(
                        name: "fk_spell_slot_pools_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "characters",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spell_slot_levels",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    spell_slot_pool_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    current_uses = table.Column<int>(type: "integer", nullable: false),
                    max_uses = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spell_slot_levels", x => x.id);
                    table.ForeignKey(
                        name: "fk_spell_slot_levels_spell_slot_pools_spell_slot_pool_id",
                        column: x => x.spell_slot_pool_id,
                        principalSchema: "characters",
                        principalTable: "spell_slot_pools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_spell_slot_levels_spell_slot_pool_id",
                schema: "characters",
                table: "spell_slot_levels",
                column: "spell_slot_pool_id");

            migrationBuilder.CreateIndex(
                name: "ix_spell_slot_pools_character_id",
                schema: "characters",
                table: "spell_slot_pools",
                column: "character_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "spell_slot_levels",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "spell_slot_pools",
                schema: "characters");
        }
    }
}
