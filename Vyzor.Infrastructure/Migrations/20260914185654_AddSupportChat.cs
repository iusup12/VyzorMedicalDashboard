using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vyzor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportChats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMessageAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportChats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportChats_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SupportChatId = table.Column<int>(type: "integer", nullable: false),
                    SenderUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsFromSupport = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportChatMessages_SupportChats_SupportChatId",
                        column: x => x.SupportChatId,
                        principalTable: "SupportChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportChatMessages_CreatedAtUtc",
                table: "SupportChatMessages",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_SupportChatMessages_SenderUserId",
                table: "SupportChatMessages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportChatMessages_SupportChatId",
                table: "SupportChatMessages",
                column: "SupportChatId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportChats_LastMessageAtUtc",
                table: "SupportChats",
                column: "LastMessageAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_SupportChats_PatientId",
                table: "SupportChats",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportChatMessages");

            migrationBuilder.DropTable(
                name: "SupportChats");
        }
    }
}
