using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StargateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceTablesWithDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DutyTitle",
                table: "AstronautDuty");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "AstronautDuty");

            migrationBuilder.AddColumn<int>(
                name: "DutyTitleId",
                table: "AstronautDuty",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RankId",
                table: "AstronautDuty",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DutyTitle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutyTitle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rank", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DutyTitle",
                columns: new[] { "Id", "CreatedDate", "Description", "IsActive", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unit commanding officer", true, "Commander" },
                    { 2, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Company-level command", true, "Captain" },
                    { 3, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Battalion-level command", true, "Major" },
                    { 4, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Platoon-level command", true, "Lieutenant" },
                    { 5, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Retired from active service", true, "RETIRED" }
                });

            migrationBuilder.InsertData(
                table: "Rank",
                columns: new[] { "Id", "Abbreviation", "CreatedDate", "IsActive", "Level", "Name" },
                values: new object[,]
                {
                    { 1, "2LT", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 1, "Second Lieutenant" },
                    { 2, "1LT", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 2, "First Lieutenant" },
                    { 3, "CPT", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 3, "Captain" },
                    { 4, "MAJ", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4, "Major" },
                    { 5, "LTC", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 5, "Lieutenant Colonel" },
                    { 6, "COL", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 6, "Colonel" }
                });

            // Migrate existing data from string columns to foreign key columns
            migrationBuilder.Sql(@"
                UPDATE AstronautDuty 
                SET RankId = CASE 
                    WHEN Rank = '1LT' THEN 2
                    WHEN Rank = 'CPT' THEN 3
                    WHEN Rank = 'MAJ' THEN 4
                    WHEN Rank = 'LTC' THEN 5
                    WHEN Rank = 'COL' THEN 6
                    ELSE 2
                END,
                DutyTitleId = CASE 
                    WHEN DutyTitle = 'Commander' THEN 1
                    WHEN DutyTitle = 'Captain' THEN 2
                    WHEN DutyTitle = 'Major' THEN 3
                    WHEN DutyTitle = 'Lieutenant' THEN 4
                    WHEN DutyTitle = 'RETIRED' THEN 5
                    ELSE 1
                END
                WHERE RankId = 0 OR DutyTitleId = 0;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_AstronautDuty_DutyTitleId",
                table: "AstronautDuty",
                column: "DutyTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_AstronautDuty_RankId",
                table: "AstronautDuty",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_DutyTitle_Title",
                table: "DutyTitle",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rank_Abbreviation",
                table: "Rank",
                column: "Abbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rank_Name",
                table: "Rank",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AstronautDuty_DutyTitle_DutyTitleId",
                table: "AstronautDuty",
                column: "DutyTitleId",
                principalTable: "DutyTitle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AstronautDuty_Rank_RankId",
                table: "AstronautDuty",
                column: "RankId",
                principalTable: "Rank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AstronautDuty_DutyTitle_DutyTitleId",
                table: "AstronautDuty");

            migrationBuilder.DropForeignKey(
                name: "FK_AstronautDuty_Rank_RankId",
                table: "AstronautDuty");

            migrationBuilder.DropTable(
                name: "DutyTitle");

            migrationBuilder.DropTable(
                name: "Rank");

            migrationBuilder.DropIndex(
                name: "IX_AstronautDuty_DutyTitleId",
                table: "AstronautDuty");

            migrationBuilder.DropIndex(
                name: "IX_AstronautDuty_RankId",
                table: "AstronautDuty");

            migrationBuilder.DropColumn(
                name: "DutyTitleId",
                table: "AstronautDuty");

            migrationBuilder.DropColumn(
                name: "RankId",
                table: "AstronautDuty");

            migrationBuilder.AddColumn<string>(
                name: "DutyTitle",
                table: "AstronautDuty",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rank",
                table: "AstronautDuty",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DutyTitle", "Rank" },
                values: new object[] { "Commander", "1LT" });
        }
    }
}
