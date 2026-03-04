using RacMonsters.Server.Repositories.Battles;
using RacMonsters.Server.Repositories.Characters;
using RacMonsters.Server.Repositories.Sessions;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Battles
{
    public class BattleService : IBattleService
    {
        private readonly IBattleRepository _battleRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger<BattleService> _logger;

        public BattleService(
            IBattleRepository battleRepository,
            ICharacterRepository characterRepository,
            ISessionRepository sessionRepository,
            ILogger<BattleService> logger)
        {
            _battleRepository = battleRepository;
            _characterRepository = characterRepository;
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        public async Task<Battle> CreateBattle(Battle battle)
        {
            // assume caller supplied characters or they are already resolved
            battle.Rounds ??= Array.Empty<Round>();
            return await _battleRepository.CreateBattle(battle);
        }

        public async Task<Battle?> GetBattle(int id)
        {
            return await _battleRepository.GetBattle(id);
        }

        // PHASE 4: Full multiplayer implementation
        public async Task<int> CreateMultiplayerBattle(
            string player1ConnectionId,
            int player1CharacterId,
            string player2ConnectionId,
            int player2CharacterId)
        {
            _logger.LogInformation($"Creating multiplayer battle: P1={player1CharacterId} vs P2={player2CharacterId}");

            // Create a new session for multiplayer
            var session = await _sessionRepository.CreateSession(new Session());

            // Load characters
            var player1Character = await _characterRepository.GetCharacter(player1CharacterId);
            var player2Character = await _characterRepository.GetCharacter(player2CharacterId);

            if (player1Character == null || player2Character == null)
            {
                throw new InvalidOperationException("One or both characters not found");
            }

            // Reset character health to max
            player1Character.CurrentHealth = player1Character.MaxHealth;
            player2Character.CurrentHealth = player2Character.MaxHealth;

            // Create multiplayer battle
            var battle = new Battle
            {
                CharacterA = player1Character,
                CharacterB = player2Character,
                Rounds = Array.Empty<Round>(),
                IsMultiplayer = true,
                Player1ConnectionId = player1ConnectionId,
                Player2ConnectionId = player2ConnectionId,
                CurrentTurnConnectionId = player1ConnectionId, // Player 1 goes first
                TurnStartTime = DateTime.UtcNow,
                TurnTimeoutSeconds = 30
            };

            var createdBattle = await _battleRepository.CreateBattle(battle);

            _logger.LogInformation($"Multiplayer battle {createdBattle.Id} created successfully");

            return createdBattle.Id;
        }

        public async Task<BattleResult> ProcessPlayerMove(int battleId, string connectionId, int abilityId)
        {
            var battle = await _battleRepository.GetBattle(battleId);

            if (battle == null)
            {
                throw new InvalidOperationException($"Battle {battleId} not found");
            }

            // Determine if this is player 1 or player 2
            var isPlayer1 = connectionId == battle.Player1ConnectionId;

            if (isPlayer1)
            {
                battle.Player1SelectedAbilityId = abilityId;
                battle.Player1Ready = true;
            }
            else
            {
                battle.Player2SelectedAbilityId = abilityId;
                battle.Player2Ready = true;
            }

            await _battleRepository.UpdateBattle(battle);

            // Reload battle to get the latest state (in case the other player also just selected)
            battle = await _battleRepository.GetBattle(battleId);

            if (battle == null)
            {
                throw new InvalidOperationException($"Battle {battleId} not found after update");
            }

            // Check if both players are ready
            if (battle.Player1Ready && battle.Player2Ready)
            {
                // Both players have selected, process the round
                return await ProcessSimultaneousRound(battle, connectionId);
            }
            else
            {
                // Return a waiting state
                var activeCharacter = isPlayer1 ? battle.CharacterA : battle.CharacterB;
                var opponentCharacter = isPlayer1 ? battle.CharacterB : battle.CharacterA;

                return new BattleResult
                {
                    BattleId = battleId,
                    PlayerCharacter = activeCharacter,
                    OpponentCharacter = opponentCharacter,
                    IsGameOver = false,
                    Message = "Waiting for opponent to select their ability...",
                    TriggeringPlayerConnectionId = connectionId,
                    TriggeringPlayerName = activeCharacter.Name,
                    TriggeringPlayerCharacter = activeCharacter
                };
            }
        }

        private async Task<BattleResult> ProcessSimultaneousRound(Battle battle, string? triggeringConnectionId = null)
        {
            var player1Character = battle.CharacterA;
            var player2Character = battle.CharacterB;
            var resultMessages = new List<string>();
            var statusMessages = new List<string>();

            // Process start-of-turn status effects (DoTs, etc.)
            StatusEffectProcessor.ProcessStartOfTurnEffects(player1Character, resultMessages);
            StatusEffectProcessor.ProcessStartOfTurnEffects(player2Character, resultMessages);

            // Check if either player is defeated by DoT
            bool battleOver = player1Character.CurrentHealth <= 0 || player2Character.CurrentHealth <= 0;

            if (!battleOver)
            {
                // Get the selected abilities
                var player1Ability = player1Character.Abilities.FirstOrDefault(a => a.Id == battle.Player1SelectedAbilityId);
                var player2Ability = player2Character.Abilities.FirstOrDefault(a => a.Id == battle.Player2SelectedAbilityId);

                if (player1Ability == null || player2Ability == null)
                {
                    throw new InvalidOperationException("One or both abilities not found");
                }

                // Determine turn order based on ability speed
                bool player1First = player1Ability.Speed >= player2Ability.Speed;

                var firstCharacter = player1First ? player1Character : player2Character;
                var firstAbility = player1First ? player1Ability : player2Ability;
                var secondCharacter = player1First ? player2Character : player1Character;
                var secondAbility = player1First ? player2Ability : player1Ability;

                // Process first action
                var firstResult = StatusEffectProcessor.ProcessAbilityWithStatusEffects(firstCharacter, firstAbility, secondCharacter);
                var firstMessage = StatusEffectProcessor.FormatAbilityResult(firstCharacter, firstAbility, secondCharacter, firstResult.damage, firstResult.heal, firstResult.hit);
                resultMessages.Add(firstMessage);
                statusMessages.AddRange(firstResult.statusMessages);

                // Check if second character is still alive
                battleOver = secondCharacter.CurrentHealth <= 0;
                var secondResult = (damage: 0, heal: 0, hit: false, statusMessages: new List<string>());

                if (!battleOver)
                {
                    // Process second action
                    secondResult = StatusEffectProcessor.ProcessAbilityWithStatusEffects(secondCharacter, secondAbility, firstCharacter);
                    var secondMessage = StatusEffectProcessor.FormatAbilityResult(secondCharacter, secondAbility, firstCharacter, secondResult.damage, secondResult.heal, secondResult.hit);
                    resultMessages.Add(secondMessage);
                    statusMessages.AddRange(secondResult.statusMessages);

                    battleOver = firstCharacter.CurrentHealth <= 0;
                }

                // Process end-of-turn effects (decrement durations, remove expired effects)
                StatusEffectProcessor.ProcessEndOfTurnEffects(player1Character, statusMessages);
                StatusEffectProcessor.ProcessEndOfTurnEffects(player2Character, statusMessages);

                resultMessages.AddRange(statusMessages);

                // Create round record with both actions
                var round = new Round
                {
                    PlayerA = new RoundAction
                    {
                        Character = player1Character,
                        Ability = player1Ability,
                        Hit = player1First ? firstResult.hit : secondResult.hit,
                        ResultMessage = player1First ? firstMessage : StatusEffectProcessor.FormatAbilityResult(player1Character, player1Ability, player2Character, secondResult.damage, secondResult.heal, secondResult.hit),
                        Damage = player1First ? firstResult.damage : secondResult.damage,
                        HealAmount = player1First ? firstResult.heal : secondResult.heal,
                        StatusEffectMessages = player1First ? firstResult.statusMessages : secondResult.statusMessages
                    },
                    PlayerB = new RoundAction
                    {
                        Character = player2Character,
                        Ability = player2Ability,
                        Hit = !player1First ? firstResult.hit : secondResult.hit,
                        ResultMessage = !player1First ? firstMessage : StatusEffectProcessor.FormatAbilityResult(player2Character, player2Ability, player1Character, secondResult.damage, secondResult.heal, secondResult.hit),
                        Damage = !player1First ? firstResult.damage : secondResult.damage,
                        HealAmount = !player1First ? firstResult.heal : secondResult.heal,
                        StatusEffectMessages = !player1First ? firstResult.statusMessages : secondResult.statusMessages
                    }
                };

                // Add round to battle
                var rounds = battle.Rounds.ToList();
                rounds.Add(round);
                battle.Rounds = rounds.ToArray();
            }

            // Check if battle is over
            string? winner = null;
            if (battleOver || player1Character.CurrentHealth <= 0 || player2Character.CurrentHealth <= 0)
            {
                battleOver = true;
                var winningCharacter = player1Character.CurrentHealth > 0 ? player1Character : player2Character;
                battle.WinningCharacter = winningCharacter.Name;
                winner = player1Character.CurrentHealth > 0 ? "player1" : "player2";
                _logger.LogInformation($"Battle {battle.Id} ended. Winner: {winningCharacter.Name}");
            }

            // Reset ready states for next turn
            battle.Player1Ready = false;
            battle.Player2Ready = false;
            battle.Player1SelectedAbilityId = null;
            battle.Player2SelectedAbilityId = null;
            battle.TurnStartTime = DateTime.UtcNow;

            // Update battle
            await _battleRepository.UpdateBattle(battle);

            // Determine triggering player info
            string? triggeringPlayerName = null;
            Character? triggeringPlayerCharacter = null;

            if (triggeringConnectionId != null)
            {
                var isTriggeringPlayer1 = triggeringConnectionId == battle.Player1ConnectionId;
                triggeringPlayerName = isTriggeringPlayer1 ? player1Character.Name : player2Character.Name;
                triggeringPlayerCharacter = isTriggeringPlayer1 ? player1Character : player2Character;
            }

            return new BattleResult
            {
                BattleId = battle.Id,
                PlayerCharacter = player1Character,
                OpponentCharacter = player2Character,
                LastRound = battle.Rounds.LastOrDefault(),
                IsGameOver = battleOver,
                Winner = winner,
                Message = string.Join(" | ", resultMessages),
                TriggeringPlayerConnectionId = triggeringConnectionId,
                TriggeringPlayerName = triggeringPlayerName,
                TriggeringPlayerCharacter = triggeringPlayerCharacter
            };
        }

        public async Task<bool> IsPlayersTurn(int battleId, string connectionId)
        {
            var battle = await _battleRepository.GetBattle(battleId);

            if (battle == null)
            {
                return false;
            }

            // In simultaneous mode, it's always the player's turn unless they've already selected
            var isPlayer1 = connectionId == battle.Player1ConnectionId;
            return isPlayer1 ? !battle.Player1Ready : !battle.Player2Ready;
        }

        public async Task HandlePlayerDisconnect(int battleId, string connectionId)
        {
            var battle = await _battleRepository.GetBattle(battleId);

            if (battle == null || battle.WinningCharacter != null)
            {
                return; // Battle doesn't exist or already ended
            }

            _logger.LogInformation($"Player {connectionId} disconnected from battle {battleId}");

            // Award victory to the remaining player
            var remainingPlayerConnection = battle.Player1ConnectionId == connectionId
                ? battle.Player2ConnectionId
                : battle.Player1ConnectionId;

            await EndBattleByForfeit(battleId, remainingPlayerConnection!);
        }

        public async Task EndBattleByForfeit(int battleId, string winnerConnectionId)
        {
            var battle = await _battleRepository.GetBattle(battleId);

            if (battle == null)
            {
                return;
            }

            var winningCharacter = winnerConnectionId == battle.Player1ConnectionId
                ? battle.CharacterA
                : battle.CharacterB;

            battle.WinningCharacter = winningCharacter.Name;

            _logger.LogInformation($"Battle {battleId} ended by forfeit. Winner: {winningCharacter.Name}");

            await _battleRepository.UpdateBattle(battle);
        }

        public async Task<List<Battle>> GetBattlesWithExpiredTurns()
        {
            // This will be fully implemented in Phase 6 (Turn Timeout System)
            return await _battleRepository.GetBattlesWithExpiredTurns();
        }

        public async Task ProcessAutoMove(int battleId)
        {
            _logger.LogInformation($"Processing auto-move for battle {battleId} due to timeout");

            var battle = await _battleRepository.GetBattle(battleId);

            if (battle == null || battle.WinningCharacter != null)
            {
                return;
            }

            try
            {
                // In simultaneous mode, check if one player hasn't selected yet
                // If one player is ready but the other timed out, auto-select for the timed-out player
                if (battle.Player1Ready && !battle.Player2Ready)
                {
                    // Player 2 timed out, auto-select ability
                    _logger.LogInformation($"Player 2 timed out in battle {battleId}, auto-selecting ability");
                    var player2Character = battle.CharacterB;
                    if (player2Character.Abilities.Length > 0)
                    {
                        var random = new Random();
                        var randomAbility = player2Character.Abilities[random.Next(player2Character.Abilities.Length)];
                        battle.Player2SelectedAbilityId = randomAbility.Id;
                        battle.Player2Ready = true;
                        await _battleRepository.UpdateBattle(battle);

                        // Now process the round since both are ready
                        var result = await ProcessSimultaneousRound(battle, null);
                        // Note: The GameHub would normally notify players, but since this is a timeout,
                        // the TurnTimeoutService should handle notifications
                    }
                }
                else if (!battle.Player1Ready && battle.Player2Ready)
                {
                    // Player 1 timed out, auto-select ability
                    _logger.LogInformation($"Player 1 timed out in battle {battleId}, auto-selecting ability");
                    var player1Character = battle.CharacterA;
                    if (player1Character.Abilities.Length > 0)
                    {
                        var random = new Random();
                        var randomAbility = player1Character.Abilities[random.Next(player1Character.Abilities.Length)];
                        battle.Player1SelectedAbilityId = randomAbility.Id;
                        battle.Player1Ready = true;
                        await _battleRepository.UpdateBattle(battle);

                        // Now process the round since both are ready
                        var result = await ProcessSimultaneousRound(battle, null);
                    }
                }
                else if (!battle.Player1Ready && !battle.Player2Ready)
                {
                    // Both players timed out, auto-select for both
                    _logger.LogInformation($"Both players timed out in battle {battleId}, auto-selecting abilities");
                    var random = new Random();

                    if (battle.CharacterA.Abilities.Length > 0)
                    {
                        var randomAbility1 = battle.CharacterA.Abilities[random.Next(battle.CharacterA.Abilities.Length)];
                        battle.Player1SelectedAbilityId = randomAbility1.Id;
                        battle.Player1Ready = true;
                    }

                    if (battle.CharacterB.Abilities.Length > 0)
                    {
                        var randomAbility2 = battle.CharacterB.Abilities[random.Next(battle.CharacterB.Abilities.Length)];
                        battle.Player2SelectedAbilityId = randomAbility2.Id;
                        battle.Player2Ready = true;
                    }

                    await _battleRepository.UpdateBattle(battle);

                    // Process the round
                    if (battle.Player1Ready && battle.Player2Ready)
                    {
                        var result = await ProcessSimultaneousRound(battle, null);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing auto-move for battle {battleId}");

                // Fallback: reset ready states and restart turn
                battle.Player1Ready = false;
                battle.Player2Ready = false;
                battle.Player1SelectedAbilityId = null;
                battle.Player2SelectedAbilityId = null;
                battle.TurnStartTime = DateTime.UtcNow;
                await _battleRepository.UpdateBattle(battle);
            }
        }

        // Team Battle Methods - Phase 2
        public async Task<int> CreateTeamBattle(
            string player1ConnectionId,
            List<int> player1CharacterIds,
            string player2ConnectionId,
            List<int> player2CharacterIds)
        {
            _logger.LogInformation($"Creating team battle: P1={string.Join(",", player1CharacterIds)} vs P2={string.Join(",", player2CharacterIds)}");

            if (player1CharacterIds.Count != 4 || player2CharacterIds.Count != 4)
            {
                throw new InvalidOperationException("Each team must have exactly 4 characters");
            }

            // Create a new session for team battle
            var session = await _sessionRepository.CreateSession(new Session());

            // Load all characters for both teams
            var team1Characters = new List<Character>();
            foreach (var charId in player1CharacterIds)
            {
                var character = await _characterRepository.GetCharacter(charId);
                if (character == null)
                {
                    throw new InvalidOperationException($"Character {charId} not found");
                }
                // Reset character health to max
                character.CurrentHealth = character.MaxHealth;
                character.ActiveStatusEffects.Clear();
                team1Characters.Add(character);
            }

            var team2Characters = new List<Character>();
            foreach (var charId in player2CharacterIds)
            {
                var character = await _characterRepository.GetCharacter(charId);
                if (character == null)
                {
                    throw new InvalidOperationException($"Character {charId} not found");
                }
                // Reset character health to max
                character.CurrentHealth = character.MaxHealth;
                character.ActiveStatusEffects.Clear();
                team2Characters.Add(character);
            }

            // Create team battle
            var battle = new Battle
            {
                Mode = BattleMode.TeamBattle,
                CharacterA = team1Characters[0], // Set first character for backward compatibility
                CharacterB = team2Characters[0],
                Team1Characters = team1Characters,
                Team2Characters = team2Characters,
                ActiveTeam1CharacterIndex = 0,
                ActiveTeam2CharacterIndex = 0,
                Rounds = Array.Empty<Round>(),
                IsMultiplayer = true,
                Player1ConnectionId = player1ConnectionId,
                Player2ConnectionId = player2ConnectionId,
                CurrentTurnConnectionId = player1ConnectionId,
                TurnStartTime = DateTime.UtcNow,
                TurnTimeoutSeconds = 30
            };

            var createdBattle = await _battleRepository.CreateBattle(battle);

            _logger.LogInformation($"Team battle {createdBattle.Id} created successfully");

            return createdBattle.Id;
        }

        public async Task<BattleResult> ProcessSwitchCharacter(int battleId, string connectionId, int newCharacterIndex)
        {
            var battle = await _battleRepository.GetBattle(battleId);

            if (battle == null)
            {
                throw new InvalidOperationException($"Battle {battleId} not found");
            }

            if (battle.Mode != BattleMode.TeamBattle)
            {
                throw new InvalidOperationException("Character switching is only available in team battles");
            }

            // Determine if this is player 1 or player 2
            var isPlayer1 = connectionId == battle.Player1ConnectionId;
            var team = isPlayer1 ? battle.Team1Characters : battle.Team2Characters;
            var currentIndex = isPlayer1 ? battle.ActiveTeam1CharacterIndex : battle.ActiveTeam2CharacterIndex;

            if (team == null || newCharacterIndex < 0 || newCharacterIndex >= team.Count)
            {
                throw new InvalidOperationException("Invalid character index");
            }

            if (newCharacterIndex == currentIndex)
            {
                throw new InvalidOperationException("Character is already active");
            }

            var targetCharacter = team[newCharacterIndex];
            if (targetCharacter.IsKnockedOut)
            {
                throw new InvalidOperationException("Cannot switch to a knocked out character");
            }

            // Update active character index
            if (isPlayer1)
            {
                battle.ActiveTeam1CharacterIndex = newCharacterIndex;
                battle.CharacterA = targetCharacter; // Update for backward compatibility
                battle.Player1Ready = true;
            }
            else
            {
                battle.ActiveTeam2CharacterIndex = newCharacterIndex;
                battle.CharacterB = targetCharacter;
                battle.Player2Ready = true;
            }

            await _battleRepository.UpdateBattle(battle);

            // Reload battle to get the latest state
            battle = await _battleRepository.GetBattle(battleId);

            if (battle == null)
            {
                throw new InvalidOperationException($"Battle {battleId} not found after update");
            }

            // Check if both players are ready
            if (battle.Player1Ready && battle.Player2Ready)
            {
                // Both players have made their choice, process the round
                return await ProcessTeamBattleRound(battle, connectionId);
            }
            else
            {
                // Return a waiting state
                var activeCharacter = battle.GetActiveTeam1Character();
                var opponentCharacter = battle.GetActiveTeam2Character();

                return new BattleResult
                {
                    BattleId = battleId,
                    PlayerCharacter = isPlayer1 ? activeCharacter : opponentCharacter,
                    OpponentCharacter = isPlayer1 ? opponentCharacter : activeCharacter,
                    IsGameOver = false,
                    Message = $"{targetCharacter.Name} is now active! Waiting for opponent...",
                    TriggeringPlayerConnectionId = connectionId,
                    TriggeringPlayerName = targetCharacter.Name,
                    TriggeringPlayerCharacter = targetCharacter
                };
            }
        }

        private async Task<BattleResult> ProcessTeamBattleRound(Battle battle, string? triggeringConnectionId = null)
        {
            var player1Character = battle.GetActiveTeam1Character();
            var player2Character = battle.GetActiveTeam2Character();
            var resultMessages = new List<string>();
            var statusMessages = new List<string>();

            // Process start-of-turn status effects (DoTs, etc.)
            StatusEffectProcessor.ProcessStartOfTurnEffects(player1Character, resultMessages);
            StatusEffectProcessor.ProcessStartOfTurnEffects(player2Character, resultMessages);

            // Check if either player is defeated by DoT
            bool roundOver = player1Character.IsKnockedOut || player2Character.IsKnockedOut;

            if (!roundOver)
            {
                // Get the selected abilities (if both used abilities instead of switching)
                var player1Ability = battle.Player1SelectedAbilityId.HasValue
                    ? player1Character.Abilities.FirstOrDefault(a => a.Id == battle.Player1SelectedAbilityId)
                    : null;
                var player2Ability = battle.Player2SelectedAbilityId.HasValue
                    ? player2Character.Abilities.FirstOrDefault(a => a.Id == battle.Player2SelectedAbilityId)
                    : null;

                // Process abilities if both players used abilities
                if (player1Ability != null && player2Ability != null)
                {
                    // Determine turn order based on ability speed
                    bool player1First = player1Ability.Speed >= player2Ability.Speed;

                    var firstCharacter = player1First ? player1Character : player2Character;
                    var firstAbility = player1First ? player1Ability : player2Ability;
                    var secondCharacter = player1First ? player2Character : player1Character;
                    var secondAbility = player1First ? player2Ability : player1Ability;

                    // Process first action
                    var firstResult = StatusEffectProcessor.ProcessAbilityWithStatusEffects(firstCharacter, firstAbility, secondCharacter);
                    var firstMessage = StatusEffectProcessor.FormatAbilityResult(firstCharacter, firstAbility, secondCharacter, firstResult.damage, firstResult.heal, firstResult.hit);
                    resultMessages.Add(firstMessage);
                    statusMessages.AddRange(firstResult.statusMessages);

                    // Check if second character is still alive
                    roundOver = secondCharacter.IsKnockedOut;
                    var secondResult = (damage: 0, heal: 0, hit: false, statusMessages: new List<string>());

                    if (!roundOver)
                    {
                        // Process second action
                        secondResult = StatusEffectProcessor.ProcessAbilityWithStatusEffects(secondCharacter, secondAbility, firstCharacter);
                        var secondMessage = StatusEffectProcessor.FormatAbilityResult(secondCharacter, secondAbility, firstCharacter, secondResult.damage, secondResult.heal, secondResult.hit);
                        resultMessages.Add(secondMessage);
                        statusMessages.AddRange(secondResult.statusMessages);

                        roundOver = firstCharacter.IsKnockedOut;
                    }

                    // Create round record with both actions
                    var round = new Round
                    {
                        PlayerA = new RoundAction
                        {
                            Type = ActionType.Ability,
                            Character = player1Character,
                            Ability = player1Ability,
                            Hit = player1First ? firstResult.hit : secondResult.hit,
                            ResultMessage = player1First ? firstMessage : StatusEffectProcessor.FormatAbilityResult(player1Character, player1Ability, player2Character, secondResult.damage, secondResult.heal, secondResult.hit),
                            Damage = player1First ? firstResult.damage : secondResult.damage,
                            HealAmount = player1First ? firstResult.heal : secondResult.heal,
                            StatusEffectMessages = player1First ? firstResult.statusMessages : secondResult.statusMessages
                        },
                        PlayerB = new RoundAction
                        {
                            Type = ActionType.Ability,
                            Character = player2Character,
                            Ability = player2Ability,
                            Hit = !player1First ? firstResult.hit : secondResult.hit,
                            ResultMessage = !player1First ? firstMessage : StatusEffectProcessor.FormatAbilityResult(player2Character, player2Ability, player1Character, secondResult.damage, secondResult.heal, secondResult.hit),
                            Damage = !player1First ? firstResult.damage : secondResult.damage,
                            HealAmount = !player1First ? firstResult.heal : secondResult.heal,
                            StatusEffectMessages = !player1First ? firstResult.statusMessages : secondResult.statusMessages
                        }
                    };

                    // Add round to battle
                    var rounds = battle.Rounds.ToList();
                    rounds.Add(round);
                    battle.Rounds = rounds.ToArray();
                }
                else if (player1Ability != null)
                {
                    // Only player 1 used an ability, player 2 switched
                    resultMessages.Add($"{player2Character.Name} switched in!");

                    var result = StatusEffectProcessor.ProcessAbilityWithStatusEffects(player1Character, player1Ability, player2Character);
                    var message = StatusEffectProcessor.FormatAbilityResult(player1Character, player1Ability, player2Character, result.damage, result.heal, result.hit);
                    resultMessages.Add(message);
                    statusMessages.AddRange(result.statusMessages);
                }
                else if (player2Ability != null)
                {
                    // Only player 2 used an ability, player 1 switched
                    resultMessages.Add($"{player1Character.Name} switched in!");

                    var result = StatusEffectProcessor.ProcessAbilityWithStatusEffects(player2Character, player2Ability, player1Character);
                    var message = StatusEffectProcessor.FormatAbilityResult(player2Character, player2Ability, player1Character, result.damage, result.heal, result.hit);
                    resultMessages.Add(message);
                    statusMessages.AddRange(result.statusMessages);
                }
                else
                {
                    // Both players switched
                    resultMessages.Add($"{player1Character.Name} and {player2Character.Name} switched in!");
                }

                // Process end-of-turn effects (decrement durations, remove expired effects)
                StatusEffectProcessor.ProcessEndOfTurnEffects(player1Character, statusMessages);
                StatusEffectProcessor.ProcessEndOfTurnEffects(player2Character, statusMessages);

                resultMessages.AddRange(statusMessages);
            }

            // Check if entire team is defeated
            bool battleOver = battle.IsTeam1Defeated() || battle.IsTeam2Defeated();
            string? winner = null;

            if (battleOver)
            {
                var winningTeam = battle.IsTeam1Defeated() ? "player2" : "player1";
                var winningCharacter = battle.IsTeam1Defeated() ? player2Character : player1Character;
                battle.WinningCharacter = winningCharacter.Name;
                winner = winningTeam;
                _logger.LogInformation($"Team battle {battle.Id} ended. Winner: Team {winner}");
            }

            // Reset ready states for next turn
            battle.Player1Ready = false;
            battle.Player2Ready = false;
            battle.Player1SelectedAbilityId = null;
            battle.Player2SelectedAbilityId = null;
            battle.TurnStartTime = DateTime.UtcNow;

            // Update battle
            await _battleRepository.UpdateBattle(battle);

            // Determine triggering player info
            string? triggeringPlayerName = null;
            Character? triggeringPlayerCharacter = null;

            if (triggeringConnectionId != null)
            {
                var isTriggeringPlayer1 = triggeringConnectionId == battle.Player1ConnectionId;
                triggeringPlayerName = isTriggeringPlayer1 ? player1Character.Name : player2Character.Name;
                triggeringPlayerCharacter = isTriggeringPlayer1 ? player1Character : player2Character;
            }

            return new BattleResult
            {
                BattleId = battle.Id,
                PlayerCharacter = player1Character,
                OpponentCharacter = player2Character,
                LastRound = battle.Rounds.LastOrDefault(),
                IsGameOver = battleOver,
                Winner = winner,
                Message = string.Join(" | ", resultMessages),
                TriggeringPlayerConnectionId = triggeringConnectionId,
                TriggeringPlayerName = triggeringPlayerName,
                TriggeringPlayerCharacter = triggeringPlayerCharacter
            };
        }
    }
}
