using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RacMonsters.Server.Data
{
    public class SessionEntity
    {
        [Key]
        public int Id { get; set; }
        public ICollection<BattleEntity> Battles { get; set; } = new List<BattleEntity>();
    }

    public class BattleEntity
    {
        [Key]
        public int Id { get; set; }
        public int SessionId { get; set; }
        public SessionEntity? Session { get; set; }

        // store characters as JSON to avoid complex relational mapping for characters
        public string CharacterAJson { get; set; } = string.Empty;
        public string CharacterBJson { get; set; } = string.Empty;
        public string? WinningCharacter { get; set; }

        public ICollection<RoundEntity> Rounds { get; set; } = new List<RoundEntity>();
    }

    public class RoundEntity
    {
        [Key]
        public int Id { get; set; }
        public int BattleId { get; set; }
        public BattleEntity? Battle { get; set; }

        // store actions as JSON to preserve action details
        public string PlayerAJson { get; set; } = string.Empty;
        public string PlayerBJson { get; set; } = string.Empty;
    }

    public class RacMonstersDbContext : DbContext
    {
        public RacMonstersDbContext(DbContextOptions<RacMonstersDbContext> options) : base(options)
        {
        }

        public DbSet<SessionEntity> Sessions { get; set; } = null!;
        public DbSet<BattleEntity> Battles { get; set; } = null!;
        public DbSet<RoundEntity> Rounds { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SessionEntity>(eb =>
            {
                eb.ToTable("Sessions");
                eb.HasKey(e => e.Id);
            });

            modelBuilder.Entity<BattleEntity>(eb =>
            {
                eb.ToTable("Battles");
                eb.HasKey(e => e.Id);
                eb.Property(e => e.CharacterAJson).HasColumnType("nvarchar(max)");
                eb.Property(e => e.CharacterBJson).HasColumnType("nvarchar(max)");
                eb.Property(e => e.WinningCharacter).HasColumnType("nvarchar(200)");
                eb.HasOne(e => e.Session).WithMany(s => s.Battles).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RoundEntity>(eb =>
            {
                eb.ToTable("Rounds");
                eb.HasKey(e => e.Id);
                eb.Property(e => e.PlayerAJson).HasColumnType("nvarchar(max)");
                eb.Property(e => e.PlayerBJson).HasColumnType("nvarchar(max)");
                eb.HasOne(e => e.Battle).WithMany(b => b.Rounds).HasForeignKey(e => e.BattleId).OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
