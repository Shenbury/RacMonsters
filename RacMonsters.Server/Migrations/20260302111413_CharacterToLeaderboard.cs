using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RacMonsters.Server.Migrations
{
    /// <inheritdoc />
    public partial class CharacterToLeaderboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Character",
                table: "Leaderboard",
                type: "nvarchar(200)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Character",
                table: "Leaderboard");
        }
    }
}
