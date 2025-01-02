using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketBallLiveScore.Server.Migrations
{
    /// <inheritdoc />
    public partial class addEncoder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Encoder",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Encoder",
                table: "Matches");
        }
    }
}
