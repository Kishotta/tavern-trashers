using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "characters");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "characters",
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
                name: "character_classes",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_homebrew = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_classes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inbox_message_consumers",
                schema: "characters",
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
                schema: "characters",
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
                schema: "characters",
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
                schema: "characters",
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
                name: "resource_definitions",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resource_definitions", x => x.id);
                    table.ForeignKey(
                        name: "fk_resource_definitions_character_classes_character_class_id",
                        column: x => x.character_class_id,
                        principalSchema: "characters",
                        principalTable: "character_classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_levels",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_levels", x => x.id);
                    table.ForeignKey(
                        name: "fk_class_levels_character_classes_character_class_id",
                        column: x => x.character_class_id,
                        principalSchema: "characters",
                        principalTable: "character_classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_class_levels_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "characters",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_resources",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_amount = table.Column<int>(type: "integer", nullable: false),
                    max_amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_resources", x => x.id);
                    table.ForeignKey(
                        name: "fk_character_resources_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "characters",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_character_resources_resource_definitions_resource_definitio",
                        column: x => x.resource_definition_id,
                        principalSchema: "characters",
                        principalTable: "resource_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resource_allowance",
                schema: "characters",
                columns: table => new
                {
                    level = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    resource_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resource_allowance", x => new { x.resource_definition_id, x.level });
                    table.ForeignKey(
                        name: "fk_resource_allowance_resource_definitions_resource_definition",
                        column: x => x.resource_definition_id,
                        principalSchema: "characters",
                        principalTable: "resource_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
            
            migrationBuilder.CreateIndex(
                name: "ix_character_classes_name",
                schema: "characters",
                table: "character_classes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_character_resources_character_id_resource_definition_id",
                schema: "characters",
                table: "character_resources",
                columns: new[] { "character_id", "resource_definition_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_character_resources_resource_definition_id",
                schema: "characters",
                table: "character_resources",
                column: "resource_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_levels_character_class_id",
                schema: "characters",
                table: "class_levels",
                column: "character_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_levels_character_id_character_class_id",
                schema: "characters",
                table: "class_levels",
                columns: new[] { "character_id", "character_class_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_resource_definitions_character_class_id",
                schema: "characters",
                table: "resource_definitions",
                column: "character_class_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "character_resources",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "class_levels",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "inbox_message_consumers",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "outbox_message_consumers",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "resource_allowance",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "characters",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "resource_definitions",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "character_classes",
                schema: "characters");
        }
    }
}
