using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                schema: "campaigns",
                table: "campaigns",
                newName: "title");

            migrationBuilder.CreateTable(
                name: "campaign_members",
                schema: "campaigns",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    campaign_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campaign_members", x => new { x.campaign_id, x.player_id });
                    table.ForeignKey(
                        name: "fk_campaign_members_campaigns_campaign_id",
                        column: x => x.campaign_id,
                        principalSchema: "campaigns",
                        principalTable: "campaigns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_members",
                schema: "campaigns");

            migrationBuilder.RenameColumn(
                name: "title",
                schema: "campaigns",
                table: "campaigns",
                newName: "name");
        }
    }
}
