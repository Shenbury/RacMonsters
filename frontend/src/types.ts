// Types for multiplayer functionality (Phase 5)

export enum StatusEffectType {
    Burn = 0,
    Poison = 1,
    Bleed = 2,
    AttackUp = 3,
    AttackDown = 4,
    DefenseUp = 5,
    DefenseDown = 6,
    TechAttackUp = 7,
    TechAttackDown = 8,
    TechDefenseUp = 9,
    TechDefenseDown = 10,
    AccuracyUp = 11,
    AccuracyDown = 12,
    EvasionUp = 13,
    EvasionDown = 14,
    Charging = 15,
    HealBlock = 16,
    Stunned = 17,
    Protected = 18
}

export interface StatusEffect {
    type: StatusEffectType;
    name: string;
    description: string;
    duration: number;
    power: number;
    modifier: number;
    sourceAbilityName?: string;
    chargingAbilityId?: number;
}

export interface StatusEffectApplication {
    type: StatusEffectType;
    duration: number;
    power: number;
    modifier: number;
    applyChance: number;
    applyToSelf: boolean;
    requiresCharging: boolean;
}

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
    activeStatusEffects: StatusEffect[];
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
    statusEffects: StatusEffectApplication[];
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
    statusEffectMessages: string[];
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

