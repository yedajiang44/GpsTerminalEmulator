using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jt808TerminalEmulator.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Line",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 36, nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreateUserId = table.Column<string>(maxLength: 36, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Distance = table.Column<decimal>(nullable: false),
                    LocationCount = table.Column<int>(nullable: false),
                    Interval = table.Column<int>(nullable: false),
                    Speed = table.Column<double>(nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Line", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Terminal",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 36, nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreateUserId = table.Column<string>(maxLength: 36, nullable: true),
                    Sim = table.Column<string>(nullable: true),
                    LicensePlate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 36, nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreateUserId = table.Column<string>(maxLength: 36, nullable: true),
                    Logintude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    LineId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_Line_LineId",
                        column: x => x.LineId,
                        principalTable: "Line",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_LineId",
                table: "Location",
                column: "LineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Terminal");

            migrationBuilder.DropTable(
                name: "Line");
        }
    }
}
