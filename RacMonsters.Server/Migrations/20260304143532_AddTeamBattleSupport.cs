using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RacMonsters.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamBattleSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerAActionType",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerASwitchToIndex",
                table: "Rounds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerBActionType",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerBSwitchToIndex",
                table: "Rounds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActiveTeam1CharacterIndex",
                table: "Battles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActiveTeam2CharacterIndex",
                table: "Battles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BattleMode",
                table: "Battles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Team1CharacterIds",
                table: "Battles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Team2CharacterIds",
                table: "Battles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerAActionType",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerASwitchToIndex",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBActionType",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBSwitchToIndex",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "ActiveTeam1CharacterIndex",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "ActiveTeam2CharacterIndex",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "BattleMode",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "Team1CharacterIds",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "Team2CharacterIds",
                table: "Battles");
        }
    }
}
