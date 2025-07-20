using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDieResultSizeToRoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add new jsonb columns
            migrationBuilder.AddColumn<string>(
                name: "raw_rolls_jsonb",
                schema: "dice",
                table: "rolls",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");
            migrationBuilder.AddColumn<string>(
                name: "kept_rolls_jsonb",
                schema: "dice",
                table: "rolls",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");

            // 2. Copy and transform data from int[] to jsonb (default Size: "unknown")
            migrationBuilder.Sql(@"
                UPDATE dice.rolls
                SET raw_rolls_jsonb = (
                    SELECT jsonb_agg(jsonb_build_object('Value', val, 'Size', 'unknown'))
                    FROM unnest(raw_rolls) AS val
                ),
                kept_rolls_jsonb = (
                    SELECT jsonb_agg(jsonb_build_object('Value', val, 'Size', 'unknown'))
                    FROM unnest(kept_rolls) AS val
                )
            ");

            // 3. Drop old columns
            migrationBuilder.DropColumn(
                name: "raw_rolls",
                schema: "dice",
                table: "rolls");
            migrationBuilder.DropColumn(
                name: "kept_rolls",
                schema: "dice",
                table: "rolls");

            // 4. Rename new columns
            migrationBuilder.RenameColumn(
                name: "raw_rolls_jsonb",
                schema: "dice",
                table: "rolls",
                newName: "raw_rolls");
            migrationBuilder.RenameColumn(
                name: "kept_rolls_jsonb",
                schema: "dice",
                table: "rolls",
                newName: "kept_rolls");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Add old integer[] columns
            migrationBuilder.AddColumn<int[]>(
                name: "raw_rolls_int",
                schema: "dice",
                table: "rolls",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
            migrationBuilder.AddColumn<int[]>(
                name: "kept_rolls_int",
                schema: "dice",
                table: "rolls",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            // 2. Copy and transform data from jsonb to int[]
            migrationBuilder.Sql(@"
                UPDATE dice.rolls
                SET raw_rolls_int = (
                    SELECT array_agg((elem->>'Value')::int)
                    FROM jsonb_array_elements(raw_rolls) AS elem
                ),
                kept_rolls_int = (
                    SELECT array_agg((elem->>'Value')::int)
                    FROM jsonb_array_elements(kept_rolls) AS elem
                )
            ");

            // 3. Drop new columns
            migrationBuilder.DropColumn(
                name: "raw_rolls",
                schema: "dice",
                table: "rolls");
            migrationBuilder.DropColumn(
                name: "kept_rolls",
                schema: "dice",
                table: "rolls");

            // 4. Rename old columns back
            migrationBuilder.RenameColumn(
                name: "raw_rolls_int",
                schema: "dice",
                table: "rolls",
                newName: "raw_rolls");
            migrationBuilder.RenameColumn(
                name: "kept_rolls_int",
                schema: "dice",
                table: "rolls",
                newName: "kept_rolls");
        }
    }
}
