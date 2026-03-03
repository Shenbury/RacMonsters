import './GameModeSelection.css';

interface Props {
    playerName: string;
    onSelectMode: (mode: 'singleplayer' | 'multiplayer') => void;
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
                        <h2>Multiplayer</h2>
                        <p className="mode-description">
                            Challenge real players online<br/>
                            Test your strategy<br/>
                            Prove you're the best
                        </p>
                        <button className="btn mode-button">
                            Find Match
                        </button>
                    </div>
                </div>

                <div className="mode-hint">
                    <p>💡 Tip: Start with Single Player to learn the characters and abilities!</p>
                </div>
            </div>
        </div>
    );
}
