using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "campaign_id",
                schema: "characters",
                table: "characters",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<int>(
                name: "level",
                schema: "characters",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                schema: "characters",
                table: "characters",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateTable(
                name: "campaign_read_models",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campaign_read_models", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_characters_campaign_id",
                schema: "characters",
                table: "characters",
                column: "campaign_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_read_models",
                schema: "characters");

            migrationBuilder.DropIndex(
                name: "ix_characters_campaign_id",
                schema: "characters",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "campaign_id",
                schema: "characters",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "level",
                schema: "characters",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "owner_id",
                schema: "characters",
                table: "characters");
        }
    }
}
