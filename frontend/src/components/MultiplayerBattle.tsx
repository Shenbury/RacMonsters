import { useState, useEffect } from 'react';
import { signalRService } from '../services/signalRService';
import type { BattleState, Character } from '../types';
import './MultiplayerBattle.css';

interface Props {
    battleId: number;
    opponentName: string;
    opponentCharacterId: number;
    initialIsMyTurn: boolean;
    playerCharacter: Character;
    onBattleEnd: (won: boolean) => void;
    onBackToLobby: () => void;
}

export function MultiplayerBattle({ 
    battleId, 
    opponentName, 
    opponentCharacterId,
    initialIsMyTurn, 
    playerCharacter,
    onBattleEnd,
    onBackToLobby
}: Props) {
    const [battleState, setBattleState] = useState<BattleState | null>(null);
    const [selectedAbility, setSelectedAbility] = useState<number | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [isWaitingForOpponent, setIsWaitingForOpponent] = useState(false);
    const [message, setMessage] = useState<string>('');
    const [opponentChar, setOpponentChar] = useState<Character | null>(null);
    const [playerChar, setPlayerChar] = useState<Character>(playerCharacter);
    const [battleLog, setBattleLog] = useState<string[]>(['Battle started!']);

    useEffect(() => {
        // Fetch opponent character data
        fetch(`/api/characters/${opponentCharacterId}`)
            .then(res => res.json())
            .then(data => setOpponentChar(data))
            .catch(err => console.error('Error fetching opponent character:', err));

        signalRService.onTurnProcessed((result: BattleState) => {
            console.log('Turn processed result:', result);

            setBattleState(result);

            // Update characters with the latest health values
            setPlayerChar(result.playerCharacter);
            setOpponentChar(result.opponentCharacter);

            setMessage(result.message || '');
            setBattleLog(prev => [result.message || 'Turn processed', ...prev].slice(0, 10));

            // Check if a round was actually processed
            if (result.lastRound != null) {
                // Round was processed - reset everything for next turn
                setIsProcessing(false);
                setIsWaitingForOpponent(false);
                setSelectedAbility(null);
            }
            // If lastRound is null, it's just an acknowledgment
            // The waiting state should remain true, so don't change it

            if (result.isGameOver) {
                const won = result.playerCharacter.currentHealth > 0;
                const gameOverMsg = won ? 'Victory! You won the battle!' : 'Defeat! Better luck next time.';
                setBattleLog(prev => [gameOverMsg, ...prev].slice(0, 10));
                setTimeout(() => onBattleEnd(won), 3000);
            }
        });

        signalRService.onOpponentReady((data: { battleId: number; message: string; opponentName: string }) => {
            console.log('Opponent ready:', data);
            setMessage(data.message);
            setBattleLog(prev => [data.message, ...prev].slice(0, 10));
        });

        signalRService.onTurnTimeout((msg: string) => {
            setMessage(msg);
            setBattleLog(prev => [msg, ...prev].slice(0, 10));
        });

        signalRService.onError((error: string) => {
            setMessage(`Error: ${error}`);
            setBattleLog(prev => [`Error: ${error}`, ...prev].slice(0, 10));
            setIsProcessing(false);
            setIsWaitingForOpponent(false);
        });

        return () => {
            signalRService.offTurnProcessed();
            signalRService.offOpponentReady();
            signalRService.offTurnTimeout();
            signalRService.offError();
        };
    }, [battleId, opponentCharacterId, onBattleEnd, playerCharacter.id]);

    const handleSelectAbility = async (abilityId: number) => {
        if (isProcessing || isWaitingForOpponent) return;

        setIsProcessing(true);
        setIsWaitingForOpponent(true);
        setSelectedAbility(abilityId);
        setMessage('Ability selected! Waiting for opponent...');

        try {
            await signalRService.selectAbility(battleId, abilityId);
            setIsProcessing(false);
        } catch (error) {
            console.error('Error selecting ability:', error);
            setMessage('Failed to use ability. Please try again.');
            setIsProcessing(false);
            setIsWaitingForOpponent(false);
            setSelectedAbility(null);
        }
    };

    const getHealthPercentage = (current: number, max: number) => {
        return (current / max) * 100;
    };

    const getHealthColor = (percentage: number) => {
        if (percentage > 60) return '#4CAF50';
        if (percentage > 30) return '#FFC107';
        return '#f44336';
    };

    if (!opponentChar) {
        return (
            <div className="multiplayer-battle loading">
                <div className="spinner"></div>
                <p>Loading battle...</p>
            </div>
        );
    }

    return (
        <div className="multiplayer-battle">
            <div className="battle-header">
                <h2>VS {opponentName}</h2>
                <div className={`turn-indicator ${isWaitingForOpponent ? 'waiting' : ''}`}>
                    {isWaitingForOpponent ? "⏳ Waiting for opponent..." : "⚔️ Select Your Ability"}
                </div>
            </div>

            {message && (
                <div className="battle-message">
                    {message}
                </div>
            )}

            <div className="battle-arena">
                {/* Player character */}
                <div className="character-display player">
                    <div className="character-label">You</div>
                    <img src={playerChar.imageUrl} alt={playerChar.name} />
                    <h3>{playerChar.name}</h3>
                    <div className="health-bar-container">
                        <div 
                            className="health-bar"
                            style={{
                                width: `${getHealthPercentage(playerChar.currentHealth, playerChar.maxHealth)}%`,
                                backgroundColor: getHealthColor(getHealthPercentage(playerChar.currentHealth, playerChar.maxHealth))
                            }}
                        >
                            <span className="health-bar-text">
                                {playerChar.currentHealth} / {playerChar.maxHealth}
                            </span>
                        </div>
                    </div>
                    <div className="character-stats-compact">
                        <div className="stat-compact">
                            <span className="stat-icon">⚔️</span>
                            <span>{playerChar.attack}</span>
                        </div>
                        <div className="stat-compact">
                            <span className="stat-icon">🛡️</span>
                            <span>{playerChar.defense}</span>
                        </div>
                        <div className="stat-compact">
                            <span className="stat-icon">⚡</span>
                            <span>{playerChar.techAttack}</span>
                        </div>
                    </div>
                </div>

                {/* VS indicator */}
                <div className="vs-indicator">VS</div>

                {/* Opponent character */}
                <div className="character-display opponent">
                    <div className="character-label">{opponentName}</div>
                    <img src={opponentChar.imageUrl} alt={opponentChar.name} />
                    <h3>{opponentChar.name}</h3>
                    <div className="health-bar-container">
                        <div 
                            className="health-bar"
                            style={{
                                width: `${getHealthPercentage(opponentChar.currentHealth, opponentChar.maxHealth)}%`,
                                backgroundColor: getHealthColor(getHealthPercentage(opponentChar.currentHealth, opponentChar.maxHealth))
                            }}
                        >
                            <span className="health-bar-text">
                                {opponentChar.currentHealth} / {opponentChar.maxHealth}
                            </span>
                        </div>
                    </div>
                    <div className="character-stats-compact">
                        <div className="stat-compact">
                            <span className="stat-icon">⚔️</span>
                            <span>{opponentChar.attack}</span>
                        </div>
                        <div className="stat-compact">
                            <span className="stat-icon">🛡️</span>
                            <span>{opponentChar.defense}</span>
                        </div>
                        <div className="stat-compact">
                            <span className="stat-icon">⚡</span>
                            <span>{opponentChar.techAttack}</span>
                        </div>
                    </div>
                </div>
            </div>

            {/* Battle log */}
            <div className="battle-log">
                <h4>Battle Log</h4>
                <div className="log-entries">
                    {battleLog.map((entry, idx) => (
                        <div key={idx} className="log-entry">
                            {entry}
                        </div>
                    ))}
                </div>
            </div>

            {/* Abilities panel */}
            {!battleState?.isGameOver && !isWaitingForOpponent && (
                <div className="abilities-panel">
                    <h3>Choose Your Ability</h3>
                    <div className="abilities-grid">
                        {playerChar.abilities.map(ability => (
                            <button
                                key={ability.id}
                                onClick={() => ability.id && handleSelectAbility(ability.id)}
                                className={`ability-button ${selectedAbility === ability.id ? 'selected processing' : ''}`}
                                disabled={isProcessing}
                            >
                                <div className="ability-header">
                                    <div className="ability-name">{ability.name}</div>
                                    <div className="ability-type-badge">
                                        {ability.isHeal ? '🔵' : ability.isTech ? '⚡' : '👊'}
                                    </div>
                                </div>
                                <div className="ability-stats">
                                    <span className="ability-stat">
                                        <strong>PWR:</strong> {ability.power}
                                    </span>
                                    <span className="ability-stat">
                                        <strong>SPD:</strong> {ability.speed}
                                    </span>
                                    <span className="ability-stat">
                                        <strong>ACC:</strong> {(ability.accuracy * 100).toFixed(0)}%
                                    </span>
                                </div>
                                {ability.description && (
                                    <div className="ability-description">{ability.description}</div>
                                )}
                                {selectedAbility === ability.id && isProcessing && (
                                    <div className="ability-processing">⏳ Selecting...</div>
                                )}
                            </button>
                        ))}
                    </div>
                    <div className="ability-actions">
                        <button 
                            onClick={onBackToLobby}
                            className="btn-forfeit"
                            disabled={isProcessing}
                        >
                            Forfeit Match
                        </button>
                    </div>
                </div>
            )}

            {isWaitingForOpponent && !battleState?.isGameOver && (
                <div className="waiting-message">
                    <div className="spinner small"></div>
                    <p>Waiting for {opponentName} to select their ability...</p>
                    <p className="selected-ability-info">You selected: {playerChar.abilities.find(a => a.id === selectedAbility)?.name}</p>
                    <button 
                        onClick={onBackToLobby}
                        className="btn-forfeit"
                    >
                        Forfeit Match
                    </button>
                </div>
            )}

            {battleState?.isGameOver && (
                <div className="game-over">
                    <h2>{playerChar.currentHealth > 0 ? '🏆 Victory!' : '💀 Defeat!'}</h2>
                    <p className="game-over-message">{playerChar.currentHealth > 0 ? 'You won the battle!' : 'Better luck next time!'}</p>
                    <p className="game-over-info">Returning to mode selection...</p>
                </div>
            )}
        </div>
    );
}
