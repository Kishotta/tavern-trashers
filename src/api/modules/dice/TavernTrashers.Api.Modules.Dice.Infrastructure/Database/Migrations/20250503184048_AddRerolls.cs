using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRerolls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                schema: "dice",
                table: "rolls",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_rolls_parent_id",
                schema: "dice",
                table: "rolls",
                column: "parent_id");

            migrationBuilder.AddForeignKey(
                name: "fk_rolls_rolls_parent_id",
                schema: "dice",
                table: "rolls",
                column: "parent_id",
                principalSchema: "dice",
                principalTable: "rolls",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rolls_rolls_parent_id",
                schema: "dice",
                table: "rolls");

            migrationBuilder.DropIndex(
                name: "ix_rolls_parent_id",
                schema: "dice",
                table: "rolls");

            migrationBuilder.DropColumn(
                name: "parent_id",
                schema: "dice",
                table: "rolls");
        }
    }
}
