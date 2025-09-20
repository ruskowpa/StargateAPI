using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StargateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddLoggingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Exception = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Method = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RequestId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AdditionalData = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    MachineName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Environment = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntry", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntry_LogLevel",
                table: "LogEntry",
                column: "LogLevel");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntry_LogLevel_Timestamp",
                table: "LogEntry",
                columns: new[] { "LogLevel", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntry_Source",
                table: "LogEntry",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntry_Timestamp",
                table: "LogEntry",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntry");
        }
    }
}
