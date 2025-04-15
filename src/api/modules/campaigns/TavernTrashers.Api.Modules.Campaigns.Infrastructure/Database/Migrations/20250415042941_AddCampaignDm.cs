using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignDm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "setting",
                schema: "campaigns",
                table: "campaigns",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "campaigns",
                table: "campaigns",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "campaign_player",
                schema: "campaigns",
                columns: table => new
                {
                    campaign_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dungeon_masters_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campaign_player", x => new { x.campaign_id, x.dungeon_masters_id });
                    table.ForeignKey(
                        name: "fk_campaign_player_campaigns_campaign_id",
                        column: x => x.campaign_id,
                        principalSchema: "campaigns",
                        principalTable: "campaigns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_campaign_player_players_dungeon_masters_id",
                        column: x => x.dungeon_masters_id,
                        principalSchema: "campaigns",
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_campaign_player_dungeon_masters_id",
                schema: "campaigns",
                table: "campaign_player",
                column: "dungeon_masters_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_player",
                schema: "campaigns");

            migrationBuilder.DropColumn(
                name: "setting",
                schema: "campaigns",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "campaigns",
                table: "campaigns");
        }
    }
}
