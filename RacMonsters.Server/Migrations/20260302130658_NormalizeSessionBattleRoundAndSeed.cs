using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RacMonsters.Server.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeSessionBattleRoundAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerAJson",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBJson",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "CharacterAJson",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "CharacterBJson",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "WinningCharacter",
                table: "Battles");

            migrationBuilder.AddColumn<int>(
                name: "PlayerAAbilityId",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerACharacterId",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerADamage",
                table: "Rounds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerAHealAmount",
                table: "Rounds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PlayerAHit",
                table: "Rounds",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerAResultMessage",
                table: "Rounds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerBAbilityId",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerBCharacterId",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerBDamage",
                table: "Rounds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerBHealAmount",
                table: "Rounds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PlayerBHit",
                table: "Rounds",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerBResultMessage",
                table: "Rounds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CharacterAId",
                table: "Battles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CharacterBId",
                table: "Battles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WinningCharacterId",
                table: "Battles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Abilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTech = table.Column<bool>(type: "bit", nullable: false),
                    IsHeal = table.Column<bool>(type: "bit", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    Accuracy = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    MaxHealth = table.Column<int>(type: "int", nullable: false),
                    CurrentHealth = table.Column<int>(type: "int", nullable: false),
                    Attack = table.Column<int>(type: "int", nullable: false),
                    Defense = table.Column<int>(type: "int", nullable: false),
                    TechAttack = table.Column<int>(type: "int", nullable: false),
                    TechDefense = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterAbilities",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    AbilityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAbilities", x => new { x.CharacterId, x.AbilityId });
                    table.ForeignKey(
                        name: "FK_CharacterAbilities_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterAbilities_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_PlayerAAbilityId",
                table: "Rounds",
                column: "PlayerAAbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_PlayerACharacterId",
                table: "Rounds",
                column: "PlayerACharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_PlayerBAbilityId",
                table: "Rounds",
                column: "PlayerBAbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_PlayerBCharacterId",
                table: "Rounds",
                column: "PlayerBCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CharacterAId",
                table: "Battles",
                column: "CharacterAId");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CharacterBId",
                table: "Battles",
                column: "CharacterBId");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_WinningCharacterId",
                table: "Battles",
                column: "WinningCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAbilities_AbilityId",
                table: "CharacterAbilities",
                column: "AbilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Battles_Characters_CharacterAId",
                table: "Battles",
                column: "CharacterAId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Battles_Characters_CharacterBId",
                table: "Battles",
                column: "CharacterBId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Battles_Characters_WinningCharacterId",
                table: "Battles",
                column: "WinningCharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Abilities_PlayerAAbilityId",
                table: "Rounds",
                column: "PlayerAAbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Abilities_PlayerBAbilityId",
                table: "Rounds",
                column: "PlayerBAbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Characters_PlayerACharacterId",
                table: "Rounds",
                column: "PlayerACharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Characters_PlayerBCharacterId",
                table: "Rounds",
                column: "PlayerBCharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Battles_Characters_CharacterAId",
                table: "Battles");

            migrationBuilder.DropForeignKey(
                name: "FK_Battles_Characters_CharacterBId",
                table: "Battles");

            migrationBuilder.DropForeignKey(
                name: "FK_Battles_Characters_WinningCharacterId",
                table: "Battles");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Abilities_PlayerAAbilityId",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Abilities_PlayerBAbilityId",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Characters_PlayerACharacterId",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Characters_PlayerBCharacterId",
                table: "Rounds");

            migrationBuilder.DropTable(
                name: "CharacterAbilities");

            migrationBuilder.DropTable(
                name: "Abilities");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_PlayerAAbilityId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_PlayerACharacterId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_PlayerBAbilityId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_PlayerBCharacterId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Battles_CharacterAId",
                table: "Battles");

            migrationBuilder.DropIndex(
                name: "IX_Battles_CharacterBId",
                table: "Battles");

            migrationBuilder.DropIndex(
                name: "IX_Battles_WinningCharacterId",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "PlayerAAbilityId",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerACharacterId",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerADamage",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerAHealAmount",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerAHit",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerAResultMessage",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBAbilityId",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBCharacterId",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBDamage",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBHealAmount",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBHit",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PlayerBResultMessage",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "CharacterAId",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "CharacterBId",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "WinningCharacterId",
                table: "Battles");

            migrationBuilder.AddColumn<string>(
                name: "PlayerAJson",
                table: "Rounds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlayerBJson",
                table: "Rounds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CharacterAJson",
                table: "Battles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CharacterBJson",
                table: "Battles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WinningCharacter",
                table: "Battles",
                type: "nvarchar(200)",
                nullable: true);
        }
    }
}
