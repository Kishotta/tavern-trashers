using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRolls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rolls",
                schema: "dice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    expression = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<int>(type: "integer", nullable: false),
                    minimum = table.Column<int>(type: "integer", nullable: false),
                    maximum = table.Column<int>(type: "integer", nullable: false),
                    average = table.Column<double>(type: "double precision", nullable: false),
                    rolled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    context_json = table.Column<string>(type: "jsonb", nullable: false),
                    raw_rolls = table.Column<int[]>(type: "integer[]", nullable: false),
                    kept_rolls = table.Column<int[]>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rolls", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rolls",
                schema: "dice");
        }
    }
}
