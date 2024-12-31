using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketBallLiveScore.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuarter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropColumn(
                name: "ScoreTeam1",
                table: "Quarters");

            migrationBuilder.DropColumn(
                name: "ScoreTeam2",
                table: "Quarters");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "QuarterDuration",
                table: "Quarters",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<int>(
                name: "Team2Id",
                table: "Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Team1Id",
                table: "Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuarterDuration",
                table: "Quarters");

            migrationBuilder.AddColumn<int>(
                name: "ScoreTeam1",
                table: "Quarters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScoreTeam2",
                table: "Quarters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Team2Id",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Team1Id",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.StaffId);
                    table.ForeignKey(
                        name: "FK_Staff_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staff_TeamId",
                table: "Staff",
                column: "TeamId");
        }
    }
}
