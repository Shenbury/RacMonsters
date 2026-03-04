import { useState, useEffect } from 'react';
import { signalRService } from '../services/signalRService';
import type { Character } from '../types';
import './TeamMultiplayerBattle.css';

interface Props {
    battleId: number;
    opponentName: string;
    playerTeam: Character[];
    onBattleEnd: (won: boolean) => void;
    onBackToLobby: () => void;
}

export function TeamMultiplayerBattle({ 
    battleId, 
    opponentName, 
    playerTeam,
    onBattleEnd,
    onBackToLobby
}: Props) {
    const [activeCharIndex, setActiveCharIndex] = useState(0);
    const [opponentTeam, setOpponentTeam] = useState<Character[]>([]);
    const [opponentActiveIndex, setOpponentActiveIndex] = useState(0);
    const [selectedAbility, setSelectedAbility] = useState<number | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [isWaitingForOpponent, setIsWaitingForOpponent] = useState(false);
    const [message, setMessage] = useState<string>('');
    const [battleLog, setBattleLog] = useState<string[]>(['Team battle started!']);
    const [showSwitchMenu, setShowSwitchMenu] = useState(false);

    const activeChar = playerTeam[activeCharIndex];
    const opponentActiveChar = opponentTeam[opponentActiveIndex];

    useEffect(() => {
        // Initialize opponent team (placeholder - will be filled by SignalR events)
        // In a real implementation, this would come from the battle state
        setOpponentTeam(playerTeam.map(char => ({ ...char, id: char.id + 1000 })));

        signalRService.onTurnProcessed((result: any) => {
            console.log('Team turn processed:', result);
            console.log(setOpponentActiveIndex);

            setMessage(result.message || '');
            setBattleLog(prev => [result.message || 'Turn processed', ...prev].slice(0, 10));

            // Update team states from result
            if (result.playerTeam) {
                // playerTeam is an array - update the whole team
            }

            if (result.lastRound != null) {
                setIsProcessing(false);
                setIsWaitingForOpponent(false);
                setSelectedAbility(null);
                setShowSwitchMenu(false);
            }

            if (result.isGameOver) {
                const allPlayerDefeated = playerTeam.every(c => c.currentHealth <= 0);
                const won = !allPlayerDefeated;
                const gameOverMsg = won ? 'Victory! Your team won!' : 'Defeat! All your characters were defeated.';
                setBattleLog(prev => [gameOverMsg, ...prev].slice(0, 10));
                setTimeout(() => onBattleEnd(won), 3000);
            }
        });

        signalRService.onCharacterSwitched((data: any) => {
            console.log('Character switched:', data);
            setMessage('Character switched successfully!');
            setBattleLog(prev => [`You switched to ${data.newCharacterName}!`, ...prev].slice(0, 10));
            setIsProcessing(false);
            setShowSwitchMenu(false);
        });

        signalRService.onOpponentSwitched((data: any) => {
            console.log('Opponent switched:', data);
            setMessage('Opponent switched characters!');
            setBattleLog(prev => ['Opponent switched characters!', ...prev].slice(0, 10));
        });

        signalRService.onOpponentReady((data: { battleId: number; message: string; opponentName: string }) => {
            console.log('Opponent ready:', data);
            setMessage(data.message);
            setBattleLog(prev => [data.message, ...prev].slice(0, 10));
        });

        signalRService.onError((error: string) => {
            setMessage(`Error: ${error}`);
            setBattleLog(prev => [`Error: ${error}`, ...prev].slice(0, 10));
            setIsProcessing(false);
            setIsWaitingForOpponent(false);
            setShowSwitchMenu(false);
        });

        return () => {
            signalRService.offTurnProcessed();
            signalRService.offCharacterSwitched();
            signalRService.offOpponentSwitched();
            signalRService.offOpponentReady();
            signalRService.offError();
        };
    }, [battleId, playerTeam, onBattleEnd]);

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

    const handleSwitchCharacter = async (newIndex: number) => {
        if (isProcessing || isWaitingForOpponent || newIndex === activeCharIndex) return;

        const targetChar = playerTeam[newIndex];
        if (targetChar.currentHealth <= 0) {
            setMessage('Cannot switch to a knocked out character!');
            return;
        }

        setIsProcessing(true);
        setIsWaitingForOpponent(true);
        setMessage('Switching character... This will cost your turn!');

        try {
            await signalRService.switchCharacter(battleId, newIndex);
            setActiveCharIndex(newIndex);
        } catch (error) {
            console.error('Error switching character:', error);
            setMessage('Failed to switch character. Please try again.');
            setIsProcessing(false);
            setIsWaitingForOpponent(false);
        }
    };

    const getHealthPercentage = (current: number, max: number) => {
        return Math.max(0, (current / max) * 100);
    };

    const getHealthColor = (percentage: number) => {
        if (percentage > 60) return '#4CAF50';
        if (percentage > 30) return '#FFC107';
        return '#f44336';
    };

    return (
        <div className="team-multiplayer-battle">
            <div className="battle-header">
                <h2>Team Battle VS {opponentName}</h2>
                <div className={`turn-indicator ${isWaitingForOpponent ? 'waiting' : ''}`}>
                    {isWaitingForOpponent ? "⏳ Waiting for opponent..." : "⚔️ Select Ability or Switch"}
                </div>
            </div>

            {message && (
                <div className="battle-message">
                    {message}
                </div>
            )}

            {/* Team Rosters */}
            <div className="team-rosters">
                {/* Player Team */}
                <div className="team-roster player-roster">
                    <h3>Your Team</h3>
                    <div className="team-members">
                        {playerTeam.map((char, idx) => (
                            <div 
                                key={idx} 
                                className={`team-member ${idx === activeCharIndex ? 'active' : ''} ${char.currentHealth <= 0 ? 'defeated' : ''}`}
                            >
                                <div className="member-portrait">
                                    <img src={char.imageUrl} alt={char.name} />
                                    {idx === activeCharIndex && <div className="active-badge">ACTIVE</div>}
                                    {char.currentHealth <= 0 && <div className="defeated-overlay">K.O.</div>}
                                </div>
                                <div className="member-name">{char.name}</div>
                                <div className="member-hp-bar">
                                    <div 
                                        className="hp-fill"
                                        style={{
                                            width: `${getHealthPercentage(char.currentHealth, char.maxHealth)}%`,
                                            backgroundColor: getHealthColor(getHealthPercentage(char.currentHealth, char.maxHealth))
                                        }}
                                    />
                                    <span className="hp-text">{char.currentHealth}/{char.maxHealth}</span>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Opponent Team */}
                <div className="team-roster opponent-roster">
                    <h3>Opponent Team</h3>
                    <div className="team-members">
                        {opponentTeam.map((char, idx) => (
                            <div 
                                key={idx} 
                                className={`team-member ${idx === opponentActiveIndex ? 'active' : ''} ${char.currentHealth <= 0 ? 'defeated' : ''}`}
                            >
                                <div className="member-portrait">
                                    <img src={char.imageUrl} alt={char.name} />
                                    {idx === opponentActiveIndex && <div className="active-badge">ACTIVE</div>}
                                    {char.currentHealth <= 0 && <div className="defeated-overlay">K.O.</div>}
                                </div>
                                <div className="member-name">{char.name}</div>
                                <div className="member-hp-bar">
                                    <div 
                                        className="hp-fill"
                                        style={{
                                            width: `${getHealthPercentage(char.currentHealth, char.maxHealth)}%`,
                                            backgroundColor: getHealthColor(getHealthPercentage(char.currentHealth, char.maxHealth))
                                        }}
                                    />
                                    <span className="hp-text">{char.currentHealth}/{char.maxHealth}</span>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>

            {/* Active Characters Arena */}
            <div className="battle-arena">
                <div className="active-character player">
                    <img src={activeChar.imageUrl} alt={activeChar.name} className="char-sprite-large" />
                    <h3>{activeChar.name}</h3>
                </div>

                <div className="vs-indicator">VS</div>

                <div className="active-character opponent">
                    {opponentActiveChar && (
                        <>
                            <img src={opponentActiveChar.imageUrl} alt={opponentActiveChar.name} className="char-sprite-large" />
                            <h3>{opponentActiveChar.name}</h3>
                        </>
                    )}
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

            {/* Action Panel */}
            {!isWaitingForOpponent && activeChar.currentHealth > 0 && (
                <div className="action-panel">
                    <div className="action-buttons">
                        <button 
                            className="btn action-btn attack-btn"
                            onClick={() => setShowSwitchMenu(false)}
                            disabled={isProcessing}
                        >
                            ⚔️ Use Ability
                        </button>
                        <button 
                            className="btn action-btn switch-btn"
                            onClick={() => setShowSwitchMenu(!showSwitchMenu)}
                            disabled={isProcessing}
                        >
                            🔄 Switch Character (Costs Turn)
                        </button>
                    </div>

                    {!showSwitchMenu ? (
                        /* Abilities panel */
                        <div className="abilities-grid">
                            {activeChar.abilities.map(ability => (
                                <button
                                    key={ability.id}
                                    onClick={() => ability.id && handleSelectAbility(ability.id)}
                                    className={`ability-button ${selectedAbility === ability.id ? 'selected' : ''}`}
                                    disabled={isProcessing}
                                >
                                    <div className="ability-name">{ability.name}</div>
                                    <div className="ability-stats">
                                        <span>PWR: {ability.power}</span>
                                        <span>SPD: {ability.speed}</span>
                                        <span>ACC: {(ability.accuracy * 100).toFixed(0)}%</span>
                                    </div>
                                </button>
                            ))}
                        </div>
                    ) : (
                        /* Switch panel */
                        <div className="switch-panel">
                            <p className="switch-warning">⚠️ Switching will use your turn!</p>
                            <div className="switch-grid">
                                {playerTeam.map((char, idx) => (
                                    <button
                                        key={idx}
                                        onClick={() => handleSwitchCharacter(idx)}
                                        className={`switch-option ${idx === activeCharIndex ? 'current' : ''} ${char.currentHealth <= 0 ? 'defeated' : ''}`}
                                        disabled={isProcessing || idx === activeCharIndex || char.currentHealth <= 0}
                                    >
                                        <img src={char.imageUrl} alt={char.name} />
                                        <div className="switch-name">{char.name}</div>
                                        <div className="switch-hp">{char.currentHealth}/{char.maxHealth}</div>
                                        {idx === activeCharIndex && <div className="current-label">CURRENT</div>}
                                        {char.currentHealth <= 0 && <div className="defeated-label">K.O.</div>}
                                    </button>
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            )}

            <button 
                className="btn btn-back"
                onClick={onBackToLobby}
                style={{ marginTop: '20px' }}
            >
                ← Forfeit & Return to Lobby
            </button>
        </div>
    );
}
