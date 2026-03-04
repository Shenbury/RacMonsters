import { useState } from 'react';
import './TeamCharacterSelection.css';

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
    onSelectTeam: (team: Character[]) => void;
    onBack: () => void;
    onRefresh: () => void;
    title?: string;
    subtitle?: string;
}

export function TeamCharacterSelection({
    characters,
    loading,
    error,
    onSelectTeam,
    onBack,
    onRefresh,
    title = 'Choose your team (4 characters)',
    subtitle = 'Select 4 different characters for your team. You can switch between them during battle.'
}: Props) {
    const [filter, setFilter] = useState<string>('');
    const [selectedTeam, setSelectedTeam] = useState<Character[]>([]);
    const [hoveredChar, setHoveredChar] = useState<Character | null>(null);

    const handleCharacterClick = (char: Character) => {
        // Check if character is already in team
        const isSelected = selectedTeam.some(c => c.id === char.id);
        
        if (isSelected) {
            // Remove from team
            setSelectedTeam(selectedTeam.filter(c => c.id !== char.id));
        } else {
            // Add to team (max 4)
            if (selectedTeam.length < 4) {
                setSelectedTeam([...selectedTeam, char]);
            }
        }
    };

    const handleConfirm = () => {
        if (selectedTeam.length === 4) {
            onSelectTeam(selectedTeam);
        }
    };

    const filteredCharacters = characters.filter(c => 
        !filter || (c.name ?? '').toString().toLowerCase().includes(filter.toLowerCase())
    );

    const isCharacterSelected = (charId: number) => {
        return selectedTeam.some(c => c.id === charId);
    };

    const getSelectionOrder = (charId: number) => {
        const index = selectedTeam.findIndex(c => c.id === charId);
        return index >= 0 ? index + 1 : null;
    };

    return (
        <div className="team-character-selection">
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

            {/* Team roster display */}
            <div className="team-roster">
                <h3>Your Team ({selectedTeam.length}/4)</h3>
                <div className="team-slots">
                    {[0, 1, 2, 3].map(index => (
                        <div key={index} className={`team-slot ${selectedTeam[index] ? 'filled' : 'empty'}`}>
                            {selectedTeam[index] ? (
                                <>
                                    <div className="team-slot-image">
                                        {selectedTeam[index].imageUrl ? (
                                            <img src={selectedTeam[index].imageUrl} alt={selectedTeam[index].name} />
                                        ) : (
                                            <div className="placeholder-sprite">?</div>
                                        )}
                                    </div>
                                    <div className="team-slot-name">{selectedTeam[index].name}</div>
                                    <div className="team-slot-number">{index + 1}</div>
                                </>
                            ) : (
                                <>
                                    <div className="team-slot-empty-icon">+</div>
                                    <div className="team-slot-empty-text">Slot {index + 1}</div>
                                </>
                            )}
                        </div>
                    ))}
                </div>
                <button 
                    className={`btn btn-confirm ${selectedTeam.length === 4 ? 'ready' : 'disabled'}`}
                    onClick={handleConfirm}
                    disabled={selectedTeam.length !== 4}
                >
                    {selectedTeam.length === 4 ? 'Ready to Battle!' : `Select ${4 - selectedTeam.length} more character${4 - selectedTeam.length === 1 ? '' : 's'}`}
                </button>
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
                        {filteredCharacters.map((char) => {
                            const selected = isCharacterSelected(char.id);
                            const order = getSelectionOrder(char.id);
                            
                            return (
                                <div key={char.id} className="char-tile-wrapper">
                                    <button
                                        className={`char-tile ${selected ? 'selected' : ''}`}
                                        onClick={() => handleCharacterClick(char)}
                                        onMouseEnter={() => setHoveredChar(char)}
                                        onMouseLeave={() => setHoveredChar(null)}
                                        onFocus={() => setHoveredChar(char)}
                                        onBlur={() => setHoveredChar(null)}
                                    >
                                        {selected && order && (
                                            <div className="selection-badge">{order}</div>
                                        )}
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
                                </div>
                            );
                        })}
                    </div>

                    {hoveredChar && (
                        <div className="character-preview">
                            <h3>{hoveredChar.name}</h3>
                            <div className="preview-stats">
                                <div>HP: {hoveredChar.maxHealth}</div>
                                <div>Attack: {hoveredChar.attack}</div>
                                <div>Defense: {hoveredChar.defense}</div>
                                <div>Tech Attack: {hoveredChar.techAttack}</div>
                                <div>Tech Defense: {hoveredChar.techDefense}</div>
                            </div>
                            {hoveredChar.abilities && hoveredChar.abilities.length > 0 && (
                                <div className="preview-abilities">
                                    <h4>Abilities:</h4>
                                    <ul>
                                        {hoveredChar.abilities.map((ability, i) => (
                                            <li key={i}>
                                                <strong>{ability.name ?? ability.Name}</strong>
                                                {' - '}
                                                {ability.description ?? ability.Description}
                                            </li>
                                        ))}
                                    </ul>
                                </div>
                            )}
                        </div>
                    )}
                </>
            )}
        </div>
    );
}
