using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RacMonsters.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusEffectsToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbilityStatusEffects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AbilityId = table.Column<int>(type: "int", nullable: false),
                    StatusEffectType = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    Modifier = table.Column<double>(type: "float", nullable: false),
                    ApplyChance = table.Column<double>(type: "float", nullable: false),
                    ApplyToSelf = table.Column<bool>(type: "bit", nullable: false),
                    RequiresCharging = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilityStatusEffects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbilityStatusEffects_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbilityStatusEffects_AbilityId",
                table: "AbilityStatusEffects",
                column: "AbilityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbilityStatusEffects");
        }
    }
}
