# RacMonsters Multiplayer Implementation Guide

This guide outlines the steps to add multiplayer functionality to RacMonsters.

## Overview

**Difficulty:** Medium  
**Estimated Time:** 23-38 hours  
**Approach:** Add separate multiplayer mode alongside existing single-player

---

## Phase 1: Infrastructure Setup (4-6 hours)

### 1.1 Install SignalR Package
```bash
dotnet add RacMonsters.Server package Microsoft.AspNetCore.SignalR
dotnet add frontend npm install @microsoft/signalr
```

### 1.2 Create Game Hub
**File:** `RacMonsters.Server/Hubs/GameHub.cs`
```csharp
using Microsoft.AspNetCore.SignalR;

namespace RacMonsters.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMatchmakingService _matchmaking;
        private readonly IBattleService _battleService;

        public GameHub(IMatchmakingService matchmaking, IBattleService battleService)
        {
            _matchmaking = matchmaking;
            _battleService = battleService;
        }

        public async Task JoinMatchmaking(string playerName, int characterId)
        {
            var connectionId = Context.ConnectionId;
            await _matchmaking.AddPlayerToQueue(connectionId, playerName, characterId);
        }

        public async Task SelectAbility(int battleId, int abilityId)
        {
            var connectionId = Context.ConnectionId;
            var result = await _battleService.ProcessPlayerMove(battleId, connectionId, abilityId);
            
            // Notify both players
            await Clients.Group($"battle-{battleId}").SendAsync("TurnProcessed", result);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _matchmaking.RemovePlayer(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
```

### 1.3 Register SignalR in Program.cs
**File:** `RacMonsters.Server/Program.cs`
```csharp
// Add after builder.Services configuration
builder.Services.AddSignalR();

// Add before app.Run()
app.MapHub<GameHub>("/gameHub");
```

---

## Phase 2: Database Schema Changes (2-3 hours)

### 2.1 Create Migration for Multiplayer Support

**Add new properties to BattleEntity:**
```csharp
// RacMonsters.Server/Data/Entities/BattleEntity.cs
public class BattleEntity
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int PlayerCharacterId { get; set; }
    public int OpponentCharacterId { get; set; }
    public bool IsPlayerTurn { get; set; }
    
    // NEW FIELDS FOR MULTIPLAYER
    public bool IsMultiplayer { get; set; }
    public string? Player1ConnectionId { get; set; }
    public string? Player2ConnectionId { get; set; }
    public string? CurrentTurnConnectionId { get; set; }
    public DateTime? TurnStartTime { get; set; }
    public int TurnTimeoutSeconds { get; set; } = 30;
    
    // Existing navigation properties...
}
```

### 2.2 Create Migration
```bash
dotnet ef migrations add AddMultiplayerSupport --project RacMonsters.Server
dotnet ef database update --project RacMonsters.Server
```

---

## Phase 3: Matchmaking Service (4-6 hours)

### 3.1 Create Matchmaking Models
**File:** `RacMonsters.Server/Models/MatchmakingPlayer.cs`
```csharp
namespace RacMonsters.Server.Models
{
    public class MatchmakingPlayer
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public int CharacterId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
```

### 3.2 Create Matchmaking Service Interface
**File:** `RacMonsters.Server/Services/Matchmaking/IMatchmakingService.cs`
```csharp
namespace RacMonsters.Server.Services.Matchmaking
{
    public interface IMatchmakingService
    {
        Task AddPlayerToQueue(string connectionId, string playerName, int characterId);
        Task RemovePlayer(string connectionId);
        Task<int> GetQueueSize();
    }
}
```

### 3.3 Implement Matchmaking Service
**File:** `RacMonsters.Server/Services/Matchmaking/MatchmakingService.cs`
```csharp
using Microsoft.AspNetCore.SignalR;
using RacMonsters.Server.Hubs;
using System.Collections.Concurrent;

namespace RacMonsters.Server.Services.Matchmaking
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly ConcurrentQueue<MatchmakingPlayer> _queue = new();
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IBattleService _battleService;
        private readonly ILogger<MatchmakingService> _logger;

        public MatchmakingService(
            IHubContext<GameHub> hubContext, 
            IBattleService battleService,
            ILogger<MatchmakingService> logger)
        {
            _hubContext = hubContext;
            _battleService = battleService;
            _logger = logger;
        }

        public async Task AddPlayerToQueue(string connectionId, string playerName, int characterId)
        {
            var player = new MatchmakingPlayer
            {
                ConnectionId = connectionId,
                PlayerName = playerName,
                CharacterId = characterId
            };

            _queue.Enqueue(player);
            _logger.LogInformation($"Player {playerName} joined queue. Queue size: {_queue.Count}");

            await _hubContext.Clients.Client(connectionId).SendAsync("MatchmakingStatus", "Searching...", _queue.Count);

            await TryMatchPlayers();
        }

        private async Task TryMatchPlayers()
        {
            if (_queue.Count >= 2)
            {
                if (_queue.TryDequeue(out var player1) && _queue.TryDequeue(out var player2))
                {
                    _logger.LogInformation($"Match found: {player1.PlayerName} vs {player2.PlayerName}");

                    // Create multiplayer battle
                    var battleId = await _battleService.CreateMultiplayerBattle(
                        player1.ConnectionId, 
                        player1.CharacterId,
                        player2.ConnectionId,
                        player2.CharacterId
                    );

                    // Add both players to battle group
                    await _hubContext.Groups.AddToGroupAsync(player1.ConnectionId, $"battle-{battleId}");
                    await _hubContext.Groups.AddToGroupAsync(player2.ConnectionId, $"battle-{battleId}");

                    // Notify both players
                    await _hubContext.Clients.Client(player1.ConnectionId).SendAsync(
                        "MatchFound", 
                        battleId, 
                        player2.PlayerName,
                        true // isMyTurn
                    );
                    await _hubContext.Clients.Client(player2.ConnectionId).SendAsync(
                        "MatchFound", 
                        battleId, 
                        player1.PlayerName,
                        false // isMyTurn
                    );
                }
            }
        }

        public async Task RemovePlayer(string connectionId)
        {
            // Remove disconnected player from queue
            var newQueue = new ConcurrentQueue<MatchmakingPlayer>(
                _queue.Where(p => p.ConnectionId != connectionId)
            );
            
            // Replace queue
            while (_queue.TryDequeue(out _)) { }
            foreach (var player in newQueue)
            {
                _queue.Enqueue(player);
            }

            await Task.CompletedTask;
        }

        public Task<int> GetQueueSize()
        {
            return Task.FromResult(_queue.Count);
        }
    }
}
```

### 3.4 Register Service
**File:** `RacMonsters.Server/Extensions/InfrastructureExtensions.cs`
```csharp
// Add to ConfigureInfrastructure method
services.AddSingleton<IMatchmakingService, MatchmakingService>();
```

---

## Phase 4: Battle Service Modifications (5-7 hours)

### 4.1 Extend Battle Service
**File:** `RacMonsters.Server/Services/Battles/IBattleService.cs`
```csharp
// Add these methods to the interface
Task<int> CreateMultiplayerBattle(
    string player1ConnectionId, 
    int player1CharacterId,
    string player2ConnectionId,
    int player2CharacterId
);

Task<BattleResult> ProcessPlayerMove(int battleId, string connectionId, int abilityId);
Task<bool> IsPlayersTurn(int battleId, string connectionId);
Task HandlePlayerDisconnect(int battleId, string connectionId);
```

### 4.2 Implement Multiplayer Battle Logic
**File:** `RacMonsters.Server/Services/Battles/BattleService.cs`
```csharp
public async Task<int> CreateMultiplayerBattle(
    string player1ConnectionId, 
    int player1CharacterId,
    string player2ConnectionId,
    int player2CharacterId)
{
    // Create session for multiplayer
    var session = await _sessionRepository.CreateSessionAsync(
        player1CharacterId, 
        isMultiplayer: true
    );

    // Load characters
    var player1Character = await _characterRepository.GetByIdAsync(player1CharacterId);
    var player2Character = await _characterRepository.GetByIdAsync(player2CharacterId);

    // Create battle
    var battle = new BattleEntity
    {
        SessionId = session.Id,
        PlayerCharacterId = player1CharacterId,
        OpponentCharacterId = player2CharacterId,
        IsPlayerTurn = true,
        IsMultiplayer = true,
        Player1ConnectionId = player1ConnectionId,
        Player2ConnectionId = player2ConnectionId,
        CurrentTurnConnectionId = player1ConnectionId,
        TurnStartTime = DateTime.UtcNow
    };

    var battleId = await _battleRepository.CreateAsync(battle);
    return battleId;
}

public async Task<BattleResult> ProcessPlayerMove(int battleId, string connectionId, int abilityId)
{
    var battle = await _battleRepository.GetByIdAsync(battleId);
    
    // Validate it's the player's turn
    if (battle.CurrentTurnConnectionId != connectionId)
    {
        throw new InvalidOperationException("Not your turn!");
    }

    // Process move using existing battle logic
    var result = await ProcessMove(battleId, abilityId);

    // Switch turns
    battle.CurrentTurnConnectionId = battle.CurrentTurnConnectionId == battle.Player1ConnectionId
        ? battle.Player2ConnectionId
        : battle.Player1ConnectionId;
    battle.TurnStartTime = DateTime.UtcNow;

    await _battleRepository.UpdateAsync(battle);

    return result;
}

public async Task<bool> IsPlayersTurn(int battleId, string connectionId)
{
    var battle = await _battleRepository.GetByIdAsync(battleId);
    return battle.CurrentTurnConnectionId == connectionId;
}

public async Task HandlePlayerDisconnect(int battleId, string connectionId)
{
    var battle = await _battleRepository.GetByIdAsync(battleId);
    
    // Award victory to remaining player
    var winnerId = battle.Player1ConnectionId == connectionId 
        ? battle.Player2ConnectionId 
        : battle.Player1ConnectionId;

    // Update battle as forfeit
    await EndBattleByForfeit(battleId, winnerId);
}
```

---

## Phase 5: Frontend Integration (6-8 hours)

### 5.1 Create SignalR Connection Service
**File:** `frontend/src/services/signalRService.ts`
```typescript
import * as signalR from "@microsoft/signalr";

class SignalRService {
    private connection: signalR.HubConnection | null = null;

    async connect(): Promise<void> {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/gameHub")
            .withAutomaticReconnect()
            .build();

        await this.connection.start();
        console.log("SignalR Connected");
    }

    async joinMatchmaking(playerName: string, characterId: number): Promise<void> {
        if (!this.connection) throw new Error("Not connected");
        await this.connection.invoke("JoinMatchmaking", playerName, characterId);
    }

    async selectAbility(battleId: number, abilityId: number): Promise<void> {
        if (!this.connection) throw new Error("Not connected");
        await this.connection.invoke("SelectAbility", battleId, abilityId);
    }

    onMatchmakingStatus(callback: (status: string, queueSize: number) => void): void {
        this.connection?.on("MatchmakingStatus", callback);
    }

    onMatchFound(callback: (battleId: number, opponentName: string, isMyTurn: boolean) => void): void {
        this.connection?.on("MatchFound", callback);
    }

    onTurnProcessed(callback: (result: any) => void): void {
        this.connection?.on("TurnProcessed", callback);
    }

    disconnect(): void {
        this.connection?.stop();
    }
}

export const signalRService = new SignalRService();
```

### 5.2 Create Multiplayer Component
**File:** `frontend/src/components/MultiplayerLobby.tsx`
```tsx
import { useState, useEffect } from 'react';
import { signalRService } from '../services/signalRService';
import { Character } from '../types';

interface Props {
    characters: Character[];
    playerName: string;
}

export function MultiplayerLobby({ characters, playerName }: Props) {
    const [selectedCharacter, setSelectedCharacter] = useState<number | null>(null);
    const [isSearching, setIsSearching] = useState(false);
    const [queueSize, setQueueSize] = useState(0);
    const [status, setStatus] = useState("Select a character to begin");

    useEffect(() => {
        signalRService.connect();

        signalRService.onMatchmakingStatus((status, size) => {
            setStatus(status);
            setQueueSize(size);
        });

        signalRService.onMatchFound((battleId, opponentName, isMyTurn) => {
            // Navigate to multiplayer battle screen
            window.location.href = `/multiplayer-battle/${battleId}?opponent=${opponentName}&myTurn=${isMyTurn}`;
        });

        return () => {
            signalRService.disconnect();
        };
    }, []);

    const handleJoinQueue = async () => {
        if (!selectedCharacter) return;
        
        setIsSearching(true);
        await signalRService.joinMatchmaking(playerName, selectedCharacter);
    };

    return (
        <div className="multiplayer-lobby">
            <h1>Multiplayer Mode</h1>
            
            {!isSearching ? (
                <>
                    <h2>Select Your Character</h2>
                    <div className="character-grid">
                        {characters.map(char => (
                            <div 
                                key={char.id}
                                className={`character-card ${selectedCharacter === char.id ? 'selected' : ''}`}
                                onClick={() => setSelectedCharacter(char.id)}
                            >
                                <img src={char.imageUrl} alt={char.name} />
                                <h3>{char.name}</h3>
                            </div>
                        ))}
                    </div>
                    <button 
                        onClick={handleJoinQueue}
                        disabled={!selectedCharacter}
                        className="btn-primary"
                    >
                        Find Match
                    </button>
                </>
            ) : (
                <div className="searching">
                    <div className="spinner"></div>
                    <h2>{status}</h2>
                    <p>Players in queue: {queueSize}</p>
                </div>
            )}
        </div>
    );
}
```

### 5.3 Create Multiplayer Battle Component
**File:** `frontend/src/components/MultiplayerBattle.tsx`
```tsx
import { useState, useEffect } from 'react';
import { signalRService } from '../services/signalRService';
import { BattleState } from '../types';

interface Props {
    battleId: number;
    opponentName: string;
    initialIsMyTurn: boolean;
}

export function MultiplayerBattle({ battleId, opponentName, initialIsMyTurn }: Props) {
    const [battleState, setBattleState] = useState<BattleState | null>(null);
    const [isMyTurn, setIsMyTurn] = useState(initialIsMyTurn);
    const [selectedAbility, setSelectedAbility] = useState<number | null>(null);

    useEffect(() => {
        signalRService.onTurnProcessed((result) => {
            setBattleState(result.battleState);
            setIsMyTurn(!isMyTurn);
            
            if (result.isGameOver) {
                // Handle game over
                alert(result.winner === "player" ? "You won!" : "You lost!");
            }
        });
    }, [isMyTurn]);

    const handleSelectAbility = async () => {
        if (!selectedAbility || !isMyTurn) return;
        
        await signalRService.selectAbility(battleId, selectedAbility);
        setSelectedAbility(null);
    };

    return (
        <div className="multiplayer-battle">
            <h2>VS {opponentName}</h2>
            <div className={`turn-indicator ${isMyTurn ? 'my-turn' : 'opponent-turn'}`}>
                {isMyTurn ? "Your Turn" : "Opponent's Turn"}
            </div>

            {/* Battle UI */}
            {battleState && (
                <div className="battle-arena">
                    {/* Player character */}
                    <div className="player-side">
                        <img src={battleState.playerCharacter.imageUrl} />
                        <div className="health-bar">
                            HP: {battleState.playerCharacter.currentHealth} / {battleState.playerCharacter.maxHealth}
                        </div>
                    </div>

                    {/* Opponent character */}
                    <div className="opponent-side">
                        <img src={battleState.opponentCharacter.imageUrl} />
                        <div className="health-bar">
                            HP: {battleState.opponentCharacter.currentHealth} / {battleState.opponentCharacter.maxHealth}
                        </div>
                    </div>
                </div>
            )}

            {/* Abilities */}
            {isMyTurn && battleState && (
                <div className="abilities">
                    <h3>Select Ability</h3>
                    {battleState.playerAbilities.map(ability => (
                        <button
                            key={ability.id}
                            onClick={() => setSelectedAbility(ability.id)}
                            className={selectedAbility === ability.id ? 'selected' : ''}
                        >
                            {ability.name}
                        </button>
                    ))}
                    <button 
                        onClick={handleSelectAbility}
                        disabled={!selectedAbility}
                        className="btn-primary"
                    >
                        Use Ability
                    </button>
                </div>
            )}
        </div>
    );
}
```

---

## Phase 6: Turn Timeout System (3-4 hours)

### 6.1 Create Background Service
**File:** `RacMonsters.Server/Services/TurnTimeoutService.cs`
```csharp
public class TurnTimeoutService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<TurnTimeoutService> _logger;

    public TurnTimeoutService(IServiceProvider services, ILogger<TurnTimeoutService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var battleService = scope.ServiceProvider.GetRequiredService<IBattleService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<GameHub>>();

            var expiredBattles = await battleService.GetBattlesWithExpiredTurns();

            foreach (var battle in expiredBattles)
            {
                // Force random move or forfeit
                await battleService.ProcessAutoMove(battle.Id);
                
                // Notify players
                await hubContext.Clients.Group($"battle-{battle.Id}")
                    .SendAsync("TurnTimeout", "Turn timeout - random move selected");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
```

### 6.2 Register Background Service
```csharp
// Program.cs
builder.Services.AddHostedService<TurnTimeoutService>();
```

---

## Phase 7: Testing & Polish (6-10 hours)

### 7.1 Test Cases
- [ ] Two players can successfully match
- [ ] Turn-based gameplay works correctly
- [ ] Disconnection handling
- [ ] Timeout handling
- [ ] Battle end state synchronization
- [ ] Multiple concurrent matches

### 7.2 UI Polish
- Loading animations
- Match countdown
- Victory/defeat screens
- Connection status indicators
- Reconnection logic

---

## Phase 8: Deployment Considerations

### 8.1 Update appsettings.json
```json
{
  "SignalR": {
    "KeepAliveInterval": "00:00:10",
    "HandshakeTimeout": "00:00:05"
  },
  "Multiplayer": {
    "TurnTimeoutSeconds": 30,
    "MaxPlayersInQueue": 100
  }
}
```

### 8.2 Azure SignalR Service (Optional for scale)
```bash
dotnet add package Microsoft.Azure.SignalR
```

---

## Quick Start: Local Multiplayer Alternative

For faster implementation, consider **local hot-seat multiplayer** first:
- No SignalR needed
- Players take turns on same device
- ~4-8 hours to implement
- Great for testing game balance

---

## Next Steps

1. Start with Phase 1 (Infrastructure)
2. Test each phase before moving to next
3. Consider local multiplayer first for faster MVP
4. Add online multiplayer once core mechanics work

## Useful Resources

- [SignalR Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
- [SignalR JavaScript Client](https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client)
- [Real-time Communication Patterns](https://learn.microsoft.com/en-us/azure/architecture/patterns/category/messaging)

---

**Good luck! 🎮**
