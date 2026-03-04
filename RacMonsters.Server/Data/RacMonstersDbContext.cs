using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using RacMonsters.Server.Models;

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

        // normalized character references
        public int CharacterAId { get; set; }
        public CharacterEntity? CharacterA { get; set; }

        public int CharacterBId { get; set; }
        public CharacterEntity? CharacterB { get; set; }

        // optionally reference the winning character by id
        public int? WinningCharacterId { get; set; }
        public CharacterEntity? WinningCharacterEntity { get; set; }

        public ICollection<RoundEntity> Rounds { get; set; } = new List<RoundEntity>();

        // NEW FIELDS FOR MULTIPLAYER (Phase 2)
        public bool IsMultiplayer { get; set; }
        public string? Player1ConnectionId { get; set; }
        public string? Player2ConnectionId { get; set; }
        public string? CurrentTurnConnectionId { get; set; }
        public DateTime? TurnStartTime { get; set; }
        public int TurnTimeoutSeconds { get; set; } = 30;
    }

    public class RoundEntity
    {
        [Key]
        public int Id { get; set; }
        public int BattleId { get; set; }
        public BattleEntity? Battle { get; set; }

        // normalized action fields for Player A
        public int PlayerACharacterId { get; set; }
        public CharacterEntity? PlayerACharacter { get; set; }
        public int PlayerAAbilityId { get; set; }
        public AbilityEntity? PlayerAAbility { get; set; }
        public bool? PlayerAHit { get; set; }
        public string? PlayerAResultMessage { get; set; }
        public int? PlayerADamage { get; set; }
        public int? PlayerAHealAmount { get; set; }

        // normalized action fields for Player B
        public int PlayerBCharacterId { get; set; }
        public CharacterEntity? PlayerBCharacter { get; set; }
        public int PlayerBAbilityId { get; set; }
        public AbilityEntity? PlayerBAbility { get; set; }
        public bool? PlayerBHit { get; set; }
        public string? PlayerBResultMessage { get; set; }
        public int? PlayerBDamage { get; set; }
        public int? PlayerBHealAmount { get; set; }
    }

    public class LeaderboardEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // champion/character used by the player
        public string Character { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AbilityEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsTech { get; set; }
        public bool IsHeal { get; set; }
        public int Power { get; set; }
        public int Speed { get; set; }
        public double Accuracy { get; set; }

        public ICollection<CharacterAbilityEntity> CharacterAbilities { get; set; } = new List<CharacterAbilityEntity>();
        public ICollection<AbilityStatusEffectEntity> StatusEffects { get; set; } = new List<AbilityStatusEffectEntity>();
    }

    public class AbilityStatusEffectEntity
    {
        [Key]
        public int Id { get; set; }
        public int AbilityId { get; set; }
        public AbilityEntity? Ability { get; set; }

        public int StatusEffectType { get; set; }
        public int Duration { get; set; }
        public int Power { get; set; }
        public double Modifier { get; set; } = 1.0;
        public double ApplyChance { get; set; } = 1.0;
        public bool ApplyToSelf { get; set; } = false;
        public bool RequiresCharging { get; set; } = false;
    }

    public class CharacterEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int TechAttack { get; set; }
        public int TechDefense { get; set; }

        public ICollection<CharacterAbilityEntity> CharacterAbilities { get; set; } = new List<CharacterAbilityEntity>();
    }

    public class CharacterAbilityEntity
    {
        public int CharacterId { get; set; }
        public CharacterEntity? Character { get; set; }

        public int AbilityId { get; set; }
        public AbilityEntity? Ability { get; set; }
    }

    public class RacMonstersDbContext : DbContext
    {
        public RacMonstersDbContext(DbContextOptions<RacMonstersDbContext> options) : base(options)
        {
        }

        public DbSet<SessionEntity> Sessions { get; set; } = null!;
        public DbSet<BattleEntity> Battles { get; set; } = null!;
        public DbSet<RoundEntity> Rounds { get; set; } = null!;
        public DbSet<LeaderboardEntity> Leaderboard { get; set; } = null!;
        public DbSet<AbilityEntity> Abilities { get; set; } = null!;
        public DbSet<CharacterEntity> Characters { get; set; } = null!;
        public DbSet<CharacterAbilityEntity> CharacterAbilities { get; set; } = null!;
        public DbSet<AbilityStatusEffectEntity> AbilityStatusEffects { get; set; } = null!;

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
                eb.Property(e => e.WinningCharacterId).IsRequired(false);
                eb.HasOne(e => e.Session).WithMany(s => s.Battles).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
                eb.HasOne(e => e.CharacterA).WithMany().HasForeignKey(e => e.CharacterAId).OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(e => e.CharacterB).WithMany().HasForeignKey(e => e.CharacterBId).OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(e => e.WinningCharacterEntity).WithMany().HasForeignKey(e => e.WinningCharacterId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RoundEntity>(eb =>
            {
                eb.ToTable("Rounds");
                eb.HasKey(e => e.Id);
                eb.Property(e => e.PlayerAResultMessage).HasColumnType("nvarchar(max)");
                eb.Property(e => e.PlayerBResultMessage).HasColumnType("nvarchar(max)");
                eb.HasOne(e => e.Battle).WithMany(b => b.Rounds).HasForeignKey(e => e.BattleId).OnDelete(DeleteBehavior.Cascade);
                eb.HasOne(e => e.PlayerACharacter).WithMany().HasForeignKey(e => e.PlayerACharacterId).OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(e => e.PlayerBCharacter).WithMany().HasForeignKey(e => e.PlayerBCharacterId).OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(e => e.PlayerAAbility).WithMany().HasForeignKey(e => e.PlayerAAbilityId).OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(e => e.PlayerBAbility).WithMany().HasForeignKey(e => e.PlayerBAbilityId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AbilityEntity>(eb =>
            {
                eb.ToTable("Abilities");
                eb.HasKey(e => e.Id);
                eb.Property(e => e.Name).HasColumnType("nvarchar(200)");
                eb.Property(e => e.Description).HasColumnType("nvarchar(max)");
                eb.Property(e => e.IsTech);
                eb.Property(e => e.IsHeal);
                eb.Property(e => e.Power);
                eb.Property(e => e.Speed);
                eb.Property(e => e.Accuracy).HasColumnType("float");
            });

            modelBuilder.Entity<CharacterEntity>(eb =>
            {
                eb.ToTable("Characters");
                eb.HasKey(e => e.Id);
                eb.Property(e => e.Name).HasColumnType("nvarchar(200)");
                eb.Property(e => e.ImageUrl).HasColumnType("nvarchar(500)");
                eb.Property(e => e.MaxHealth);
                eb.Property(e => e.CurrentHealth);
                eb.Property(e => e.Attack);
                eb.Property(e => e.Defense);
                eb.Property(e => e.TechAttack);
                eb.Property(e => e.TechDefense);
            });

            modelBuilder.Entity<CharacterAbilityEntity>(eb =>
            {
                eb.ToTable("CharacterAbilities");
                eb.HasKey(e => new { e.CharacterId, e.AbilityId });
                eb.HasOne(ca => ca.Character).WithMany(c => c.CharacterAbilities).HasForeignKey(ca => ca.CharacterId).OnDelete(DeleteBehavior.Cascade);
                eb.HasOne(ca => ca.Ability).WithMany(a => a.CharacterAbilities).HasForeignKey(ca => ca.AbilityId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AbilityStatusEffectEntity>(eb =>
            {
                eb.ToTable("AbilityStatusEffects");
                eb.HasKey(e => e.Id);
                eb.HasOne(e => e.Ability).WithMany(a => a.StatusEffects).HasForeignKey(e => e.AbilityId).OnDelete(DeleteBehavior.Cascade);
                eb.Property(e => e.StatusEffectType).HasColumnType("int");
                eb.Property(e => e.Duration).HasColumnType("int");
                eb.Property(e => e.Power).HasColumnType("int");
                eb.Property(e => e.Modifier).HasColumnType("float");
                eb.Property(e => e.ApplyChance).HasColumnType("float");
                eb.Property(e => e.ApplyToSelf).HasColumnType("bit");
                eb.Property(e => e.RequiresCharging).HasColumnType("bit");
            });

            modelBuilder.Entity<LeaderboardEntity>(eb =>
            {
                eb.ToTable("Leaderboard");
                eb.HasKey(e => e.Id);
                eb.Property(e => e.Name).HasColumnType("nvarchar(200)");
                eb.Property(e => e.Character).HasColumnType("nvarchar(200)");
                eb.Property(e => e.Score).HasColumnType("int");
                eb.Property(e => e.Timestamp).HasColumnType("datetime2");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
