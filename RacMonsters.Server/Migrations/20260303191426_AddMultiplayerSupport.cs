using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RacMonsters.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiplayerSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentTurnConnectionId",
                table: "Battles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiplayer",
                table: "Battles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Player1ConnectionId",
                table: "Battles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Player2ConnectionId",
                table: "Battles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TurnStartTime",
                table: "Battles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TurnTimeoutSeconds",
                table: "Battles",
                type: "int",
                nullable: false,
                defaultValue: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTurnConnectionId",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "IsMultiplayer",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "Player1ConnectionId",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "Player2ConnectionId",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "TurnStartTime",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "TurnTimeoutSeconds",
                table: "Battles");
        }
    }
}
