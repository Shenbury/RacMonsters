using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RacMonsters.Server.Data;

#nullable disable

namespace RacMonsters.Server.Migrations
{
    [DbContext(typeof(RacMonstersDbContext))]
    partial class SessionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("RacMonsters.Server.Data.SessionEntity", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");

                b.HasKey("Id");

                b.ToTable("Sessions");
            });

            modelBuilder.Entity("RacMonsters.Server.Data.BattleEntity", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");

                b.Property<int>("SessionId").HasColumnType("int");

                b.Property<string>("CharacterAJson").HasColumnType("nvarchar(max)");

                b.Property<string>("CharacterBJson").HasColumnType("nvarchar(max)");

                b.Property<string>("WinningCharacter").HasColumnType("nvarchar(200)");

                b.HasKey("Id");

                b.HasIndex("SessionId");

                b.ToTable("Battles");
            });

            modelBuilder.Entity("RacMonsters.Server.Data.RoundEntity", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");

                b.Property<int>("BattleId").HasColumnType("int");

                b.Property<string>("PlayerAJson").HasColumnType("nvarchar(max)");

                b.Property<string>("PlayerBJson").HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.HasIndex("BattleId");

                b.ToTable("Rounds");
            });

            modelBuilder.Entity("RacMonsters.Server.Data.BattleEntity", b =>
            {
                b.HasOne("RacMonsters.Server.Data.SessionEntity", "Session")
                    .WithMany("Battles")
                    .HasForeignKey("SessionId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("RacMonsters.Server.Data.RoundEntity", b =>
            {
                b.HasOne("RacMonsters.Server.Data.BattleEntity", "Battle")
                    .WithMany("Rounds")
                    .HasForeignKey("BattleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
        }
    }
}
