import './GameModeSelection.css';

interface Props {
    playerName: string;
    onSelectMode: (mode: 'singleplayer' | 'multiplayer' | 'teambattle') => void;
}

export function GameModeSelection({ playerName, onSelectMode }: Props) {
    return (
        <div className="game-mode-selection">
            <div className="mode-container">
                <h1 className="mode-title">Welcome, {playerName}!</h1>
                <p className="mode-subtitle">Choose your game mode</p>

                <div className="mode-cards">
                    <div 
                        className="mode-card singleplayer-card"
                        onClick={() => onSelectMode('singleplayer')}
                    >
                        <div className="mode-icon">
                            <svg viewBox="0 0 100 100" width="80" height="80">
                                <circle cx="50" cy="35" r="15" fill="#4CAF50" />
                                <path d="M 50 50 L 50 75 M 35 60 L 50 55 L 65 60 M 50 75 L 40 90 M 50 75 L 60 90" 
                                    stroke="#4CAF50" strokeWidth="5" strokeLinecap="round" fill="none" />
                            </svg>
                        </div>
                        <h2>Single Player</h2>
                        <p className="mode-description">
                            Battle against AI opponents<br/>
                            Climb the leaderboard<br/>
                            Practice your skills
                        </p>
                        <button className="btn mode-button">
                            Play Solo
                        </button>
                    </div>

                    <div 
                        className="mode-card multiplayer-card"
                        onClick={() => onSelectMode('multiplayer')}
                    >
                        <div className="mode-icon">
                            <svg viewBox="0 0 100 100" width="80" height="80">
                                <circle cx="35" cy="30" r="12" fill="#2196F3" />
                                <circle cx="65" cy="30" r="12" fill="#FF9800" />
                                <path d="M 35 42 L 35 65 M 25 52 L 35 47 L 45 52 M 35 65 L 28 78 M 35 65 L 42 78" 
                                    stroke="#2196F3" strokeWidth="4" strokeLinecap="round" fill="none" />
                                <path d="M 65 42 L 65 65 M 55 52 L 65 47 L 75 52 M 65 65 L 58 78 M 65 65 L 72 78" 
                                    stroke="#FF9800" strokeWidth="4" strokeLinecap="round" fill="none" />
                            </svg>
                        </div>
                        <h2>Multiplayer 1v1</h2>
                        <p className="mode-description">
                            Challenge real players online<br/>
                            Test your strategy<br/>
                            Prove you're the best
                        </p>
                        <button className="btn mode-button">
                            Find Match
                        </button>
                    </div>

                    <div 
                        className="mode-card teambattle-card"
                        onClick={() => onSelectMode('teambattle')}
                    >
                        <div className="mode-icon">
                            <svg viewBox="0 0 100 100" width="80" height="80">
                                <circle cx="25" cy="25" r="8" fill="#FF6B6B" />
                                <circle cx="50" cy="25" r="8" fill="#4ECDC4" />
                                <circle cx="75" cy="25" r="8" fill="#FFE66D" />
                                <circle cx="37.5" cy="50" r="8" fill="#95E1D3" />
                                <rect x="15" y="60" width="70" height="5" fill="#FF9F1C" rx="2" />
                                <text x="50" y="85" fontSize="20" fill="#FF9F1C" textAnchor="middle" fontWeight="bold">4v4</text>
                            </svg>
                        </div>
                        <h2>Team Battle 4v4</h2>
                        <p className="mode-description">
                            Build a team of 4 characters<br/>
                            Switch fighters mid-battle<br/>
                            Strategic team combat
                        </p>
                        <button className="btn mode-button team-button">
                            Team Battle
                        </button>
                    </div>
                </div>

                <div className="mode-hint">
                    <p>💡 Tip: Team Battle lets you switch between 4 characters during combat!</p>
                </div>
            </div>
        </div>
    );
}
