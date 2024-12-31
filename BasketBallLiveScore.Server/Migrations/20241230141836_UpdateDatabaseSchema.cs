using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketBallLiveScore.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Changes");

            migrationBuilder.DropTable(
                name: "Fouls");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Teams",
                newName: "TeamName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Players",
                newName: "Position");

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeamColor",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCaptain",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInGame",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Competition",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MatchNumber",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OvertimeDuration",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodDuration",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Periods",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamColor",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsCaptain",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsInGame",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Competition",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "MatchNumber",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "OvertimeDuration",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "PeriodDuration",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Periods",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "TeamName",
                table: "Teams",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Players",
                newName: "Name");

            migrationBuilder.CreateTable(
                name: "Changes",
                columns: table => new
                {
                    ChangeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerInId = table.Column<int>(type: "int", nullable: false),
                    PlayerOutId = table.Column<int>(type: "int", nullable: false),
                    QuarterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Changes", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_Changes_Players_PlayerInId",
                        column: x => x.PlayerInId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Changes_Players_PlayerOutId",
                        column: x => x.PlayerOutId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Changes_Quarters_QuarterId",
                        column: x => x.QuarterId,
                        principalTable: "Quarters",
                        principalColumn: "QuarterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fouls",
                columns: table => new
                {
                    FoulId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    QuarterId = table.Column<int>(type: "int", nullable: false),
                    FoulType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fouls", x => x.FoulId);
                    table.ForeignKey(
                        name: "FK_Fouls_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fouls_Quarters_QuarterId",
                        column: x => x.QuarterId,
                        principalTable: "Quarters",
                        principalColumn: "QuarterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Changes_PlayerInId",
                table: "Changes",
                column: "PlayerInId");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_PlayerOutId",
                table: "Changes",
                column: "PlayerOutId");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_QuarterId",
                table: "Changes",
                column: "QuarterId");

            migrationBuilder.CreateIndex(
                name: "IX_Fouls_PlayerId",
                table: "Fouls",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Fouls_QuarterId",
                table: "Fouls",
                column: "QuarterId");
        }
    }
}
