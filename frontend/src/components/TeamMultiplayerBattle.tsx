import { useState, useEffect } from 'react';
import { signalRService } from '../services/signalRService';
import type { Character } from '../types';
import './TeamMultiplayerBattle.css';

interface Props {
    battleId: number;
    playerName: string;
    opponentName: string;
    playerTeam: Character[];
    opponentTeam?: Character[];
    onBattleEnd: (won: boolean) => void;
    onBackToLobby: () => void;
}

export function TeamMultiplayerBattle({ 
    battleId, 
    playerName,
    opponentName, 
    playerTeam: initialPlayerTeam,
    opponentTeam: initialOpponentTeam,
    onBattleEnd,
    onBackToLobby
}: Props) {
    const [playerTeam, setPlayerTeam] = useState<Character[]>(initialPlayerTeam);
    const [activeCharIndex, setActiveCharIndex] = useState(0);
    const [opponentTeam, setOpponentTeam] = useState<Character[]>(initialOpponentTeam || []);
    const [opponentActiveIndex, setOpponentActiveIndex] = useState(0);
    const [selectedAbility, setSelectedAbility] = useState<number | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [isWaitingForOpponent, setIsWaitingForOpponent] = useState(false);
    const [message, setMessage] = useState<string>('');
    const [battleLog, setBattleLog] = useState<string[]>(['Team battle started!']);
    const [showSwitchMenu, setShowSwitchMenu] = useState(false);

    useEffect(() => {
        // If opponent team was not provided, initialize as placeholder
        // In a real scenario, this should come from the server via TurnProcessed
        if (!initialOpponentTeam || initialOpponentTeam.length === 0) {
            console.warn('No opponent team provided, using placeholder data');
            setOpponentTeam(initialPlayerTeam.map(char => ({ ...char, id: char.id + 1000 })));
        } else {
            console.log('Opponent team received:', initialOpponentTeam);
            // Log each character's imageUrl to debug
            initialOpponentTeam.forEach((char, idx) => {
                console.log(`Opponent char ${idx}:`, char.name, 'imageUrl:', char.imageUrl);
            });
        }

        signalRService.onTurnProcessed((result: any) => {
            console.log('Team turn processed:', result);
            console.log('Received playerTeam:', result.playerTeam);
            console.log('Received opponentTeam:', result.opponentTeam);

            setMessage(result.message || '');
            setBattleLog(prev => [result.message || 'Turn processed', ...prev].slice(0, 10));

            // Update team states from result
            if (result.playerTeam && Array.isArray(result.playerTeam)) {
                console.log('Updating player team:', result.playerTeam);
                setPlayerTeam(result.playerTeam);

                // Update active character index from server
                if (result.playerActiveIndex !== undefined) {
                    setActiveCharIndex(result.playerActiveIndex);
                } else {
                    // Fallback: Find the first alive character if current is defeated
                    const currentChar = result.playerTeam[activeCharIndex];
                    if (!currentChar || currentChar.currentHealth <= 0) {
                        const aliveIndex = result.playerTeam.findIndex((char: Character) => char.currentHealth > 0);
                        if (aliveIndex >= 0) {
                            setActiveCharIndex(aliveIndex);
                        }
                    }
                }
            }

            if (result.opponentTeam && Array.isArray(result.opponentTeam)) {
                console.log('Updating opponent team:', result.opponentTeam);
                setOpponentTeam(result.opponentTeam);

                // Update opponent active index from server
                if (result.opponentActiveIndex !== undefined) {
                    setOpponentActiveIndex(result.opponentActiveIndex);
                } else {
                    // Fallback: Find the first alive character if current is defeated
                    const currentChar = result.opponentTeam[opponentActiveIndex];
                    if (!currentChar || currentChar.currentHealth <= 0) {
                        const aliveIndex = result.opponentTeam.findIndex((char: Character) => char.currentHealth > 0);
                        if (aliveIndex >= 0) {
                            setOpponentActiveIndex(aliveIndex);
                        }
                    }
                }
            }

            if (result.lastRound != null) {
                setIsProcessing(false);
                setIsWaitingForOpponent(false);
                setSelectedAbility(null);
                setShowSwitchMenu(false);
            }

            if (result.isGameOver) {
                const allPlayerDefeated = result.playerTeam?.every((c: Character) => c.currentHealth <= 0) ?? true;
                const won = !allPlayerDefeated;
                const gameOverMsg = won ? 'Victory! Your team won!' : 'Defeat! All your characters were defeated.';
                setBattleLog(prev => [gameOverMsg, ...prev].slice(0, 10));

                // Clean up battle connection before ending
                signalRService.leaveBattle(battleId).catch(err => {
                    console.error('Error leaving battle:', err);
                });

                setTimeout(() => onBattleEnd(won), 3000);
            }
        });

        signalRService.onCharacterSwitched((data: any) => {
            console.log('Character switched:', data);
            setMessage('Character switched successfully!');
            setBattleLog(prev => [`You switched to ${data.newCharacterName}!`, ...prev].slice(0, 10));

            // Update the active character index from server confirmation
            if (data.newCharacterIndex !== undefined) {
                setActiveCharIndex(data.newCharacterIndex);
            }

            setIsProcessing(false);
            setIsWaitingForOpponent(false); // Reset waiting state
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
            // Don't update activeCharIndex here - wait for server confirmation via onTurnProcessed or onCharacterSwitched
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

    // Compute active characters from current state
    const activeChar = playerTeam[activeCharIndex];
    const opponentActiveChar = opponentTeam[opponentActiveIndex];

    // Safety check
    if (!activeChar) {
        return (
            <div className="team-multiplayer-battle loading">
                <div className="spinner"></div>
                <p>Loading battle...</p>
            </div>
        );
    }

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
                    <h3>Your Team - {playerName}</h3>
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

                                {/* Hover Tooltip */}
                                <div className="member-tooltip">
                                    <div className="tooltip-header">
                                        <h4>{char.name}</h4>
                                    </div>
                                    <div className="tooltip-stats">
                                        <div className="stat-row">
                                            <span className="stat-label">⚔️ Attack:</span>
                                            <span className="stat-value">{char.attack}</span>
                                        </div>
                                        <div className="stat-row">
                                            <span className="stat-label">🛡️ Defense:</span>
                                            <span className="stat-value">{char.defense}</span>
                                        </div>
                                        <div className="stat-row">
                                            <span className="stat-label">⚡ Tech Attack:</span>
                                            <span className="stat-value">{char.techAttack}</span>
                                        </div>
                                        <div className="stat-row">
                                            <span className="stat-label">✨ Tech Defense:</span>
                                            <span className="stat-value">{char.techDefense}</span>
                                        </div>
                                    </div>
                                    <div className="tooltip-abilities">
                                        <h5>Abilities:</h5>
                                        {char.abilities.map((ability, abilityIdx) => (
                                            <div key={abilityIdx} className="tooltip-ability">
                                                <div className="tooltip-ability-name">{ability.name}</div>
                                                <div className="tooltip-ability-stats">
                                                    PWR: {ability.power} | SPD: {ability.speed} | ACC: {(ability.accuracy * 100).toFixed(0)}%
                                                </div>
                                                <div className="tooltip-ability-desc">{ability.description}</div>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Opponent Team */}
                <div className="team-roster opponent-roster">
                    <h3>Opponent Team - {opponentName}</h3>
                    <div className="team-members">
                        {opponentTeam && opponentTeam.length > 0 ? opponentTeam.map((char, idx) => {
                            // Debug log for each character
                            if (!char.imageUrl) {
                                console.warn(`Opponent character ${char.name} missing imageUrl:`, char);
                            }

                            return (
                                <div 
                                    key={idx} 
                                    className={`team-member ${idx === opponentActiveIndex ? 'active' : ''} ${char.currentHealth <= 0 ? 'defeated' : ''}`}
                                >
                                    <div className="member-portrait">
                                        {char.imageUrl ? (
                                            <img 
                                                src={char.imageUrl} 
                                                alt={char.name} 
                                                onError={(e) => {
                                                    console.error('Failed to load image for', char.name, ':', char.imageUrl);
                                                    // Replace with placeholder instead of hiding
                                                    const target = e.currentTarget;
                                                    target.style.display = 'none';
                                                    if (target.parentElement) {
                                                        const placeholder = document.createElement('div');
                                                        placeholder.className = 'image-placeholder';
                                                        placeholder.textContent = char.name?.charAt(0) || '?';
                                                        target.parentElement.appendChild(placeholder);
                                                    }
                                                }} 
                                            />
                                        ) : (
                                            <div className="image-placeholder">{char.name?.charAt(0) || '?'}</div>
                                        )}
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

                                    {/* Hover Tooltip */}
                                    <div className="member-tooltip">
                                        <div className="tooltip-header">
                                            <h4>{char.name}</h4>
                                        </div>
                                        <div className="tooltip-stats">
                                            <div className="stat-row">
                                                <span className="stat-label">⚔️ Attack:</span>
                                                <span className="stat-value">{char.attack}</span>
                                            </div>
                                            <div className="stat-row">
                                                <span className="stat-label">🛡️ Defense:</span>
                                                <span className="stat-value">{char.defense}</span>
                                            </div>
                                            <div className="stat-row">
                                                <span className="stat-label">⚡ Tech Attack:</span>
                                                <span className="stat-value">{char.techAttack}</span>
                                            </div>
                                            <div className="stat-row">
                                                <span className="stat-label">✨ Tech Defense:</span>
                                                <span className="stat-value">{char.techDefense}</span>
                                            </div>
                                        </div>
                                        <div className="tooltip-abilities">
                                            <h5>Abilities:</h5>
                                            {char.abilities.map((ability, abilityIdx) => (
                                                <div key={abilityIdx} className="tooltip-ability">
                                                    <div className="tooltip-ability-name">{ability.name}</div>
                                                    <div className="tooltip-ability-stats">
                                                        PWR: {ability.power} | SPD: {ability.speed} | ACC: {(ability.accuracy * 100).toFixed(0)}%
                                                    </div>
                                                    <div className="tooltip-ability-desc">{ability.description}</div>
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                </div>
                            );
                        }) : (
                            <div className="loading-opponent">Loading opponent team...</div>
                        )}
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

            {/* Active Character Details */}
            <div className="active-character-details">
                <div className="player-details">
                    <h4>Your Active Character</h4>
                    <div className="char-detail-card">
                        <div className="char-detail-name">{activeChar.name}</div>
                        <div className="char-detail-hp">
                            <span className="hp-label">HP:</span>
                            <div className="hp-bar-detail">
                                <div 
                                    className="hp-fill-detail"
                                    style={{
                                        width: `${getHealthPercentage(activeChar.currentHealth, activeChar.maxHealth)}%`,
                                        backgroundColor: getHealthColor(getHealthPercentage(activeChar.currentHealth, activeChar.maxHealth))
                                    }}
                                />
                            </div>
                            <span className="hp-text-detail">{activeChar.currentHealth}/{activeChar.maxHealth}</span>
                        </div>
                        {activeChar.activeStatusEffects && activeChar.activeStatusEffects.length > 0 && (
                            <div className="status-effects-detail">
                                <span className="status-label">Status:</span>
                                <div className="status-icons">
                                    {activeChar.activeStatusEffects.map((effect, idx) => (
                                        <div key={idx} className="status-badge" title={effect.name}>
                                            {effect.name} ({effect.duration})
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>
                </div>
                <div className="opponent-details">
                    <h4>Opponent Active Character</h4>
                    {opponentActiveChar && (
                        <div className="char-detail-card">
                            <div className="char-detail-name">{opponentActiveChar.name}</div>
                            <div className="char-detail-hp">
                                <span className="hp-label">HP:</span>
                                <div className="hp-bar-detail">
                                    <div 
                                        className="hp-fill-detail"
                                        style={{
                                            width: `${getHealthPercentage(opponentActiveChar.currentHealth, opponentActiveChar.maxHealth)}%`,
                                            backgroundColor: getHealthColor(getHealthPercentage(opponentActiveChar.currentHealth, opponentActiveChar.maxHealth))
                                        }}
                                    />
                                </div>
                                <span className="hp-text-detail">{opponentActiveChar.currentHealth}/{opponentActiveChar.maxHealth}</span>
                            </div>
                            {opponentActiveChar.activeStatusEffects && opponentActiveChar.activeStatusEffects.length > 0 && (
                                <div className="status-effects-detail">
                                    <span className="status-label">Status:</span>
                                    <div className="status-icons">
                                        {opponentActiveChar.activeStatusEffects.map((effect, idx) => (
                                            <div key={idx} className="status-badge" title={effect.name}>
                                                {effect.name} ({effect.duration})
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            )}
                        </div>
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
                                    <div className="ability-description">{ability.description}</div>
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
