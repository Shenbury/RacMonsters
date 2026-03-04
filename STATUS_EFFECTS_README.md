# Status Effect System

This document explains the new status effect system added to the game and how to use it.

## Overview

The game now supports various status effects that can be applied during battle:

### Status Effect Types

1. **Damage Over Time (DoT)**
   - **Burn** 🔥 - Deals damage each turn
   - **Poison** ☠️ - Deals damage each turn
   - **Bleed** 🩸 - Deals damage each turn

2. **Stat Modifiers**
   - **Attack Up** ⚔️↑ / **Attack Down** ⚔️↓
   - **Defense Up** 🛡️↑ / **Defense Down** 🛡️↓
   - **Tech Attack Up** ⚡↑ / **Tech Attack Down** ⚡↓
   - **Tech Defense Up** 🔰↑ / **Tech Defense Down** 🔰↓
   - **Accuracy Up** 🎯↑ / **Accuracy Down** 🎯↓
   - **Evasion Up** 💨↑ / **Evasion Down** 💨↓

3. **Special Effects**
   - **Charging** ⚡ - Ability takes 2 turns to activate (but deals massive damage)
   - **Heal Block** 🚫 - Prevents opponent from healing
   - **Stunned** 💫 - Character cannot act for one turn
   - **Protected** ✨ - Character is protected from damage

## How It Works

### In Battle

1. **Start of Turn**: Damage over Time effects are processed first
2. **Action Phase**: Players select and execute abilities
3. **Status Application**: If an ability hits, it may apply status effects
4. **End of Turn**: Status effect durations are decremented, expired effects are removed

### Status Effect Properties

Each status effect has:
- **Type**: The kind of effect (Burn, Defense Down, etc.)
- **Duration**: Number of turns the effect lasts
- **Power**: For DoTs, the damage dealt per turn; for stat modifiers, not directly used
- **Modifier**: Multiplier for stat changes (e.g., 0.3 for 30% increase/decrease)
- **Apply Chance**: Probability the effect will be applied (0.0 to 1.0)
- **Apply To Self**: Whether the effect targets the user (buffs) or opponent (debuffs)

## Adding Abilities with Status Effects

### Example 1: Burn Attack

```csharp
new AbilityEntity 
{ 
    Id = 69,
    Name = "Flame Strike",
    Description = "A fiery attack that may burn the opponent",
    IsTech = false,
    IsHeal = false,
    Power = 8,
    Speed = 6,
    Accuracy = 0.85
}
```

Then in code, you would configure the status effect (Note: Status effects are currently configured in code, not in the database):

```csharp
var flameStrike = new Ability
{
    Id = 69,
    Name = "Flame Strike",
    Description = "A fiery attack that may burn the opponent",
    IsTech = false,
    IsHeal = false,
    Power = 8,
    Speed = 6,
    Accuracy = 0.85,
    StatusEffects = new List<StatusEffectApplication>
    {
        new StatusEffectApplication
        {
            Type = StatusEffectType.Burn,
            Duration = 3,
            Power = 5,  // 5 damage per turn
            ApplyChance = 0.5,  // 50% chance to apply
            ApplyToSelf = false  // Targets opponent
        }
    }
};
```

### Example 2: Self-Buff Attack

```csharp
var powerUp = new Ability
{
    Id = 70,
    Name = "Power Up",
    Description = "Increase your attack power",
    IsTech = true,
    IsHeal = false,
    Power = 0,  // No immediate damage
    Speed = 8,
    Accuracy = 1.0,
    StatusEffects = new List<StatusEffectApplication>
    {
        new StatusEffectApplication
        {
            Type = StatusEffectType.AttackUp,
            Duration = 3,
            Power = 0,
            Modifier = 0.5,  // 50% attack increase
            ApplyChance = 1.0,  // Always applies
            ApplyToSelf = true  // Targets self
        }
    }
};
```

### Example 3: Charging Move

```csharp
var solarBeam = new Ability
{
    Id = 71,
    Name = "Solar Beam",
    Description = "Charge for one turn, then unleash a devastating attack",
    IsTech = true,
    IsHeal = false,
    Power = 25,  // Very high power to compensate for charge time
    Speed = 5,
    Accuracy = 0.9,
    StatusEffects = new List<StatusEffectApplication>
    {
        new StatusEffectApplication
        {
            Type = StatusEffectType.Charging,
            Duration = 1,  // Charges for 1 turn
            RequiresCharging = true,
            ApplyToSelf = true
        }
    }
};
```

### Example 4: Heal Block

```csharp
var toxicStrike = new Ability
{
    Id = 72,
    Name = "Toxic Strike",
    Description = "Poison the opponent, preventing them from healing",
    IsTech = false,
    IsHeal = false,
    Power = 6,
    Speed = 7,
    Accuracy = 0.8,
    StatusEffects = new List<StatusEffectApplication>
    {
        new StatusEffectApplication
        {
            Type = StatusEffectType.Poison,
            Duration = 3,
            Power = 4,  // 4 damage per turn
            ApplyChance = 0.7,
            ApplyToSelf = false
        },
        new StatusEffectApplication
        {
            Type = StatusEffectType.HealBlock,
            Duration = 2,
            ApplyChance = 0.5,  // 50% chance to also block healing
            ApplyToSelf = false
        }
    }
};
```

## UI Display

Status effects are displayed on character cards during battle:
- Each active effect shows its icon and remaining duration
- Hovering over an effect shows its name and duration
- Effects appear below the character's stats

## Technical Implementation

### Backend

The status effect system is implemented across several layers:

1. **Models** (`StatusEffect.cs`, `StatusEffectType.cs`, `StatusEffectApplication.cs`)
2. **Character Extensions** (`Character.cs`) - Methods to calculate effective stats
3. **Battle Processing** (`BattleService.cs`) - Apply, process, and remove effects

### Frontend

TypeScript types and UI components:

1. **Types** (`types.ts`) - Status effect interfaces
2. **Component** (`MultiplayerBattle.tsx`) - Status effect display
3. **Styling** (`MultiplayerBattle.css`) - Status effect styling

## Future Enhancements

Potential improvements to the system:

1. **Database Storage**: Store status effect configurations in the database
2. **More Effect Types**: Add confusion, sleep, freeze, etc.
3. **Combo Effects**: Certain effects trigger special interactions
4. **Visual Effects**: Add animations for status effect application
5. **Sound Effects**: Audio feedback for status changes
6. **Status Effect Immunity**: Some characters could be immune to certain effects

## Example Ability Set for a Character

Here's a complete ability set using various status effects:

```csharp
// Character: Pyromancer
var abilities = new[]
{
    // Basic attack with burn chance
    new { Id = 73, Name = "Ember Strike", Power = 8, Burn = true },
    
    // High damage charging move
    new { Id = 74, Name = "Inferno", Power = 30, Charging = true },
    
    // Self-buff ability
    new { Id = 75, Name = "Battle Fury", Power = 0, AttackUp = true },
    
    // Heal with self-buff
    new { Id = 76, Name = "Meditation", Heal = 12, DefenseUp = true }
};
```

## Notes

- Status effects stack by refreshing duration and using the highest power value
- DoT effects are processed at the start of each turn
- Stat modifiers are applied when calculating damage
- Charging moves require the player to use the same ability two turns in a row (but it executes on the second turn)
- Multiple status effects can be applied by a single ability
