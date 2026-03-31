using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDeathSavingThrows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "death_saving_throws",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_id = table.Column<Guid>(type: "uuid", nullable: false),
                    successes = table.Column<int>(type: "integer", nullable: false),
                    failures = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_death_saving_throws", x => x.id);
                    table.ForeignKey(
                        name: "fk_death_saving_throws_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "characters",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hit_points",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_id = table.Column<Guid>(type: "uuid", nullable: false),
                    base_max_hit_points = table.Column<int>(type: "integer", nullable: false),
                    current_hit_points = table.Column<int>(type: "integer", nullable: false),
                    temporary_hit_points = table.Column<int>(type: "integer", nullable: false),
                    max_hit_point_reduction = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hit_points", x => x.id);
                    table.ForeignKey(
                        name: "fk_hit_points_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "characters",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_death_saving_throws_character_id",
                schema: "characters",
                table: "death_saving_throws",
                column: "character_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_hit_points_character_id",
                schema: "characters",
                table: "hit_points",
                column: "character_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "death_saving_throws",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "hit_points",
                schema: "characters");
        }
    }
}
