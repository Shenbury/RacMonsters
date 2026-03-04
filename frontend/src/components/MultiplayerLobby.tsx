import { useState, useEffect } from 'react';
import { signalRService } from '../services/signalRService';
import type { Character } from '../types';
import './MultiplayerLobby.css';

interface Props {
    playerName: string;
    selectedCharacter?: Character;
    selectedTeam?: Character[];
    isTeamBattle?: boolean;
    onMatchFound: (battleId: number, opponentName: string, opponentCharacterId: number, isMyTurn: boolean) => void;
    onBack: () => void;
}

export function MultiplayerLobby({ 
    playerName, 
    selectedCharacter, 
    selectedTeam,
    isTeamBattle = false,
    onMatchFound, 
    onBack 
}: Props) {
    const [isSearching, setIsSearching] = useState(false);
    const [queueSize, setQueueSize] = useState(0);
    const [status, setStatus] = useState("Ready to search");
    const [error, setError] = useState<string | null>(null);
    const [isConnecting, setIsConnecting] = useState(false);

    useEffect(() => {
        const setupSignalR = async () => {
            setIsConnecting(true);
            try {
                await signalRService.connect();

                signalRService.onMatchmakingStatus((status, size) => {
                    setStatus(status);
                    setQueueSize(size);
                });

                signalRService.onMatchFound((battleId, opponentName, opponentCharacterId, isMyTurn) => {
                    setIsSearching(false);
                    onMatchFound(battleId, opponentName, opponentCharacterId, isMyTurn);
                });

                signalRService.onMatchmakingError((error) => {
                    setError(error);
                    setIsSearching(false);
                });

                setIsConnecting(false);
            } catch (err) {
                console.error("Failed to connect to SignalR:", err);
                setError("Failed to connect to multiplayer server. Please refresh the page.");
                setIsConnecting(false);
            }
        };

        setupSignalR();

        return () => {
            signalRService.offMatchmakingStatus();
            signalRService.offMatchFound();
            signalRService.offMatchmakingError();
        };
    }, [onMatchFound]);

    const handleJoinQueue = async () => {
        setError(null);
        setIsSearching(true);
        setStatus("Joining matchmaking...");

        try {
            if (isTeamBattle && selectedTeam) {
                // Team battle matchmaking
                const characterIds = selectedTeam.map(char => 
                    typeof char.id === 'number' ? char.id : parseInt(String(char.id), 10)
                );
                await signalRService.joinTeamMatchmaking(playerName, characterIds);
            } else if (selectedCharacter) {
                // Standard 1v1 matchmaking
                const characterId = typeof selectedCharacter.id === 'number' 
                    ? selectedCharacter.id 
                    : parseInt(String(selectedCharacter.id), 10);
                await signalRService.joinMatchmaking(playerName, characterId);
            } else {
                throw new Error('No character or team selected');
            }
        } catch (err) {
            console.error("Error joining queue:", err);
            setError("Failed to join matchmaking. Please try again.");
            setIsSearching(false);
        }
    };

    const handleCancelSearch = () => {
        setIsSearching(false);
        setStatus("Ready to search");
    };

    if (isConnecting) {
        return (
            <div className="multiplayer-lobby">
                <h1>{isTeamBattle ? 'Team Battle Mode' : 'Multiplayer Mode'}</h1>
                <div className="connecting">
                    <div className="spinner"></div>
                    <p>Connecting to multiplayer server...</p>
                </div>
            </div>
        );
    }

    return (
        <div className="multiplayer-lobby">
            <h1>{isTeamBattle ? 'Team Battle Mode' : 'Multiplayer Mode'}</h1>
            <p className="player-name">Playing as: <strong>{playerName}</strong></p>

            {error && (
                <div className="error-message">
                    {error}
                    <button onClick={() => setError(null)}>Dismiss</button>
                </div>
            )}

            {isTeamBattle && selectedTeam ? (
                /* Team Battle Display */
                <div className="selected-team-display">
                    <h2>Your Team</h2>
                    <div className="team-grid">
                        {selectedTeam.map((character, idx) => (
                            <div key={idx} className="team-member-card">
                                <div className="member-number">{idx + 1}</div>
                                <img src={character.imageUrl} alt={character.name} />
                                <h4>{character.name}</h4>
                                <div className="member-stats-mini">
                                    <span>HP: {character.maxHealth}</span>
                                    <span>ATK: {character.attack}</span>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            ) : selectedCharacter ? (
                /* Standard 1v1 Display */
                <div className="selected-character-display">
                    <h2>Your Champion</h2>
                    <div className="champion-card">
                        <img src={selectedCharacter.imageUrl} alt={selectedCharacter.name} />
                        <h3>{selectedCharacter.name}</h3>
                        <div className="champion-stats">
                            <div className="stat-row">
                                <span className="stat-label">HP:</span>
                                <span className="stat-value">{selectedCharacter.maxHealth}</span>
                            </div>
                            <div className="stat-row">
                                <span className="stat-label">ATK:</span>
                                <span className="stat-value">{selectedCharacter.attack}</span>
                            </div>
                            <div className="stat-row">
                                <span className="stat-label">DEF:</span>
                                <span className="stat-value">{selectedCharacter.defense}</span>
                            </div>
                            <div className="stat-row">
                                <span className="stat-label">TECH:</span>
                                <span className="stat-value">{selectedCharacter.techAttack}</span>
                            </div>
                        </div>
                    </div>
                </div>
            ) : null}

            {!isSearching ? (
                <div className="lobby-actions">
                    <button 
                        onClick={handleJoinQueue}
                        disabled={!signalRService.isConnected()}
                        className="btn-primary btn-large"
                    >
                        {signalRService.isConnected() ? 'Find Match' : 'Connecting...'}
                    </button>
                    <button 
                        onClick={onBack}
                        className="btn-secondary"
                    >
                        {isTeamBattle ? 'Change Team' : 'Change Character'}
                    </button>
                </div>
            ) : (
                <div className="searching">
                    <div className="spinner"></div>
                    <h2>{status}</h2>
                    <p className="queue-info">Players in queue: {queueSize}</p>
                    <button 
                        onClick={handleCancelSearch}
                        className="btn-secondary"
                    >
                        Cancel Search
                    </button>
                </div>
            )}
        </div>
    );
}
