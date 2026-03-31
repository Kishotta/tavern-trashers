using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dice");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "dice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    table_name = table.Column<string>(type: "text", nullable: false),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    primary_key = table.Column<string>(type: "jsonb", maxLength: 500, nullable: false),
                    old_values = table.Column<string>(type: "jsonb", maxLength: 3000, nullable: true),
                    new_values = table.Column<string>(type: "jsonb", maxLength: 3000, nullable: true),
                    affected_columns = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inbox_message_consumers",
                schema: "dice",
                columns: table => new
                {
                    inbox_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_message_consumers", x => new { x.inbox_message_id, x.name });
                });

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "dice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "jsonb", maxLength: 3000, nullable: false),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message_consumers",
                schema: "dice",
                columns: table => new
                {
                    outbox_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_message_consumers", x => new { x.outbox_message_id, x.name });
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "dice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "jsonb", maxLength: 3000, nullable: false),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rolls",
                schema: "dice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    expression = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<int>(type: "integer", nullable: false),
                    minimum = table.Column<int>(type: "integer", nullable: false),
                    maximum = table.Column<int>(type: "integer", nullable: false),
                    average = table.Column<double>(type: "double precision", nullable: false),
                    rolled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    context_json = table.Column<string>(type: "jsonb", nullable: false),
                    raw_rolls = table.Column<string>(type: "jsonb", nullable: false),
                    kept_rolls = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rolls", x => x.id);
                    table.ForeignKey(
                        name: "fk_rolls_rolls_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "dice",
                        principalTable: "rolls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_rolls_parent_id",
                schema: "dice",
                table: "rolls",
                column: "parent_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "dice");

            migrationBuilder.DropTable(
                name: "inbox_message_consumers",
                schema: "dice");

            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "dice");

            migrationBuilder.DropTable(
                name: "outbox_message_consumers",
                schema: "dice");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "dice");

            migrationBuilder.DropTable(
                name: "rolls",
                schema: "dice");
        }
    }
}
