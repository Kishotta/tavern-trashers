﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

#nullable disable

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database.Migrations
{
    [DbContext(typeof(CampaignsDbContext))]
    partial class CampaignsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("campaigns")
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TavernTrashers.Api.Common.Infrastructure.Auditing.Audit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AffectedColumns")
                        .HasColumnType("text")
                        .HasColumnName("affected_columns");

                    b.Property<string>("NewValues")
                        .HasMaxLength(3000)
                        .HasColumnType("jsonb")
                        .HasColumnName("new_values");

                    b.Property<DateTime>("OccurredAtUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("occurred_at_utc");

                    b.Property<string>("OldValues")
                        .HasMaxLength(3000)
                        .HasColumnType("jsonb")
                        .HasColumnName("old_values");

                    b.Property<string>("PrimaryKey")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("jsonb")
                        .HasColumnName("primary_key");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("table_name");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_audit_logs");

                    b.ToTable("audit_logs", "campaigns");
                });

            modelBuilder.Entity("TavernTrashers.Api.Common.Infrastructure.Inbox.InboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(3000)
                        .HasColumnType("jsonb")
                        .HasColumnName("content");

                    b.Property<string>("Error")
                        .HasColumnType("text")
                        .HasColumnName("error");

                    b.Property<DateTime>("OccurredAtUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("occurred_at_utc");

                    b.Property<DateTime?>("ProcessedAtUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("processed_at_utc");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_inbox_messages");

                    b.ToTable("inbox_messages", "campaigns");
                });

            modelBuilder.Entity("TavernTrashers.Api.Common.Infrastructure.Inbox.InboxMessageConsumer", b =>
                {
                    b.Property<Guid>("InboxMessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("inbox_message_id");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("name");

                    b.HasKey("InboxMessageId", "Name")
                        .HasName("pk_inbox_message_consumers");

                    b.ToTable("inbox_message_consumers", "campaigns");
                });

            modelBuilder.Entity("TavernTrashers.Api.Common.Infrastructure.Outbox.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(3000)
                        .HasColumnType("jsonb")
                        .HasColumnName("content");

                    b.Property<string>("Error")
                        .HasColumnType("text")
                        .HasColumnName("error");

                    b.Property<DateTime>("OccurredAtUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("occurred_at_utc");

                    b.Property<DateTime?>("ProcessedAtUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("processed_at_utc");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_outbox_messages");

                    b.ToTable("outbox_messages", "campaigns");
                });

            modelBuilder.Entity("TavernTrashers.Api.Common.Infrastructure.Outbox.OutboxMessageConsumer", b =>
                {
                    b.Property<Guid>("OutboxMessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("outbox_message_id");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("name");

                    b.HasKey("OutboxMessageId", "Name")
                        .HasName("pk_outbox_message_consumers");

                    b.ToTable("outbox_message_consumers", "campaigns");
                });

            modelBuilder.Entity("TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.Campaign", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_campaigns");

                    b.ToTable("campaigns", "campaigns");
                });
#pragma warning restore 612, 618
        }
    }
}
