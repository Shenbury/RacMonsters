// Types for multiplayer functionality (Phase 5)

export interface Character {
    id: number;
    name: string;
    imageUrl: string;
    currentHealth: number;
    maxHealth: number;
    attack: number;
    defense: number;
    techAttack: number;
    techDefense: number;
    abilities: Ability[];
}

export interface Ability {
    id?: number;
    name: string;
    description: string;
    isTech: boolean;
    isHeal: boolean;
    power: number;
    speed: number;
    accuracy: number;
}

export interface Round {
    id: number;
    playerA: RoundAction;
    playerB: RoundAction;
}

export interface RoundAction {
    character: Character;
    ability?: Ability;
    hit?: boolean;
    resultMessage?: string;
    damage?: number;
    healAmount?: number;
}

export interface BattleState {
    battleId: number;
    playerCharacter: Character;
    opponentCharacter: Character;
    lastRound?: Round;
    isGameOver: boolean;
    winner?: string;
    message?: string;
}

export interface MatchmakingStatus {
    status: string;
    queueSize: number;
}
