import { useState } from 'react';
import './CharacterSelection.css';

interface Ability {
    name?: string;
    Name?: string;
    power?: number;
    Power?: number;
    description?: string;
    Description?: string;
    isTech?: boolean;
    IsTech?: boolean;
    isHeal?: boolean;
    IsHeal?: boolean;
    accuracy?: number;
    Accuracy?: number;
    id?: number;
}

interface Character {
    id: number;
    name?: string;
    imageUrl?: string;
    currentHealth?: number;
    maxHealth?: number;
    attack?: number;
    defense?: number;
    techAttack?: number;
    techDefense?: number;
    abilities?: Ability[];
    [key: string]: any;
}

interface Props {
    characters: Character[];
    loading: boolean;
    error: string | null;
    onSelectCharacter: (character: Character) => void;
    onBack: () => void;
    onRefresh: () => void;
    title?: string;
    subtitle?: string;
}

export function CharacterSelection({
    characters,
    loading,
    error,
    onSelectCharacter,
    onBack,
    onRefresh,
    title = 'Choose your champion',
    subtitle
}: Props) {
    const [filter, setFilter] = useState<string>('');
    const [hoveredChar, setHoveredChar] = useState<Character | null>(null);
    const [selectedChar, setSelectedChar] = useState<Character | null>(null);

    const handleCharacterClick = (char: Character) => {
        setSelectedChar(char);
    };

    const handleConfirm = () => {
        if (selectedChar) {
            onSelectCharacter(selectedChar);
        }
    };

    const filteredCharacters = characters.filter(c => 
        !filter || (c.name ?? '').toString().toLowerCase().includes(filter.toLowerCase())
    );

    return (
        <div className="character-selection">
            <div className="selection-header">
                <div className="selection-title-row">
                    <h2 className="selection-title">{title}</h2>
                    <button 
                        className="btn btn-back"
                        onClick={onBack}
                    >
                        ← Back
                    </button>
                </div>
                {subtitle && <p className="selection-subtitle">{subtitle}</p>}
                
                <div className="selection-controls">
                    <input 
                        aria-label="Filter characters" 
                        placeholder="Search characters..." 
                        value={filter} 
                        onChange={e => setFilter(e.target.value)}
                        className="search-input"
                    />
                    <button 
                        className="btn btn-refresh" 
                        onClick={onRefresh} 
                        disabled={loading}
                        aria-label="Reload characters"
                    >
                        {loading ? 'Loading...' : 'Reload'}
                    </button>
                </div>
            </div>

            {loading ? (
                <div className="loading-skeleton-grid">
                    <div className="skeleton-tile" />
                    <div className="skeleton-tile" />
                    <div className="skeleton-tile" />
                    <div className="skeleton-tile" />
                    <div className="skeleton-tile" />
                    <div className="skeleton-tile" />
                </div>
            ) : error ? (
                <div className="error-banner">
                    Failed to load characters: {error}
                </div>
            ) : (
                <>
                    <div className="character-grid">
                        {filteredCharacters.map((char) => (
                            <div key={char.id} className="char-tile-wrapper">
                                <button
                                    className={`char-tile ${selectedChar?.id === char.id ? 'selected' : ''}`}
                                    onClick={() => handleCharacterClick(char)}
                                    onMouseEnter={() => setHoveredChar(char)}
                                    onMouseLeave={() => setHoveredChar(null)}
                                    onFocus={() => setHoveredChar(char)}
                                    onBlur={() => setHoveredChar(null)}
                                >
                                    <div className="char-sprite">
                                        {char.imageUrl ? (
                                            <img src={char.imageUrl} alt={char.name} />
                                        ) : (
                                            <svg width="64" height="64" viewBox="0 0 100 100">
                                                <circle cx="50" cy="50" r="44" fill="#fff" />
                                                <circle cx="50" cy="40" r="22" fill="#ff9f1c" />
                                            </svg>
                                        )}
                                    </div>
                                    <div className="char-name">{char.name}</div>
                                    <div className="char-stats">
                                        HP: {char.maxHealth ?? '--'} | ATK: {char.attack ?? '--'}
                                    </div>
                                    <div className="char-stats">
                                        DEF: {char.defense ?? '--'} | SPD: {char.techAttack ?? '--'}
                                    </div>
                                </button>

                                {hoveredChar?.id === char.id && char.abilities && char.abilities.length > 0 && (
                                    <div className="hover-abilities" aria-hidden="true">
                                        <div className="hover-abilities-title">Abilities</div>
                                        <ul>
                                            {char.abilities.map((ab: Ability, idx: number) => {
                                                const name = ab.name ?? ab.Name ?? 'Ability';
                                                const power = ab.power ?? ab.Power;
                                                return (
                                                    <li key={idx} className="hover-ability-item">
                                                        <span className="hover-ability-name">
                                                            {name} {power ? `(${power})` : ''}
                                                        </span>
                                                    </li>
                                                );
                                            })}
                                        </ul>
                                    </div>
                                )}
                            </div>
                        ))}
                    </div>

                    {selectedChar && (
                        <div className="selected-character-preview">
                            <div className="preview-content">
                                <div className="preview-image">
                                    {selectedChar.imageUrl ? (
                                        <img src={selectedChar.imageUrl} alt={selectedChar.name} />
                                    ) : (
                                        <svg width="100" height="100" viewBox="0 0 100 100">
                                            <circle cx="50" cy="50" r="44" fill="#fff" />
                                            <circle cx="50" cy="40" r="22" fill="#ff9f1c" />
                                        </svg>
                                    )}
                                </div>
                                <div className="preview-info">
                                    <h3>{selectedChar.name}</h3>
                                    <div className="preview-stats">
                                        <div className="stat-item">
                                            <span className="stat-label">Health:</span>
                                            <span className="stat-value">{selectedChar.maxHealth}</span>
                                        </div>
                                        <div className="stat-item">
                                            <span className="stat-label">Attack:</span>
                                            <span className="stat-value">{selectedChar.attack}</span>
                                        </div>
                                        <div className="stat-item">
                                            <span className="stat-label">Defense:</span>
                                            <span className="stat-value">{selectedChar.defense}</span>
                                        </div>
                                        <div className="stat-item">
                                            <span className="stat-label">Tech Attack:</span>
                                            <span className="stat-value">{selectedChar.techAttack}</span>
                                        </div>
                                        <div className="stat-item">
                                            <span className="stat-label">Tech Defense:</span>
                                            <span className="stat-value">{selectedChar.techDefense}</span>
                                        </div>
                                    </div>
                                    {selectedChar.abilities && selectedChar.abilities.length > 0 && (
                                        <div className="preview-abilities">
                                            <h4>Abilities:</h4>
                                            <ul>
                                                {selectedChar.abilities.map((ab, idx) => {
                                                    const name = ab.name ?? ab.Name ?? 'Ability';
                                                    const power = ab.power ?? ab.Power;
                                                    const desc = ab.description ?? ab.Description;
                                                    const isTech = ab.isTech ?? ab.IsTech;
                                                    const isHeal = ab.isHeal ?? ab.IsHeal;
                                                    return (
                                                        <li key={idx}>
                                                            <strong>{name}</strong> 
                                                            {power ? ` - Power: ${power}` : ''}
                                                            {isTech ? ' (Tech)' : isHeal ? ' (Heal)' : ' (Physical)'}
                                                            {desc && <div className="ability-description">{desc}</div>}
                                                        </li>
                                                    );
                                                })}
                                            </ul>
                                        </div>
                                    )}
                                </div>
                            </div>
                            <button 
                                className="btn btn-confirm"
                                onClick={handleConfirm}
                            >
                                Confirm Selection
                            </button>
                        </div>
                    )}
                </>
            )}
        </div>
    );
}
