import { useState, useEffect } from 'react';
import { signalRService } from '../services/signalRService';
import type { Character } from '../types';
import './MultiplayerLobby.css';

interface Props {
    playerName: string;
    selectedCharacter: Character;
    onMatchFound: (battleId: number, opponentName: string, opponentCharacterId: number, isMyTurn: boolean) => void;
    onBack: () => void;
}

export function MultiplayerLobby({ playerName, selectedCharacter, onMatchFound, onBack }: Props) {
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
            const characterId = typeof selectedCharacter.id === 'number' 
                ? selectedCharacter.id 
                : parseInt(String(selectedCharacter.id), 10);
            await signalRService.joinMatchmaking(playerName, characterId);
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
                <h1>Multiplayer Mode</h1>
                <div className="connecting">
                    <div className="spinner"></div>
                    <p>Connecting to multiplayer server...</p>
                </div>
            </div>
        );
    }

    return (
        <div className="multiplayer-lobby">
            <h1>Multiplayer Mode</h1>
            <p className="player-name">Playing as: <strong>{playerName}</strong></p>

            {error && (
                <div className="error-message">
                    {error}
                    <button onClick={() => setError(null)}>Dismiss</button>
                </div>
            )}

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
                        Change Character
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
