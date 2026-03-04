-- Add Status Effects to Database
-- This script adds status effect configurations for abilities

-- First, add the new abilities with status effects
SET IDENTITY_INSERT [dbo].[Abilities] ON;

INSERT INTO [dbo].[Abilities] (Id, Name, Description, IsTech, IsHeal, Power, Speed, Accuracy)
VALUES 
-- Burn abilities
(69, 'Flame Strike', 'A fiery attack that burns the opponent for 3 turns', 0, 0, 8, 6, 0.85),
(70, 'Inferno Blast', 'Massive fire damage with guaranteed burn', 1, 0, 12, 4, 0.70),

-- Poison abilities
(71, 'Toxic Strike', 'Poisons the enemy and blocks healing', 0, 0, 6, 7, 0.80),
(72, 'Venom Spray', 'Sprays venom causing poison damage over time', 1, 0, 8, 5, 0.75),

-- Stat buff abilities
(73, 'Power Up', 'Increases your attack power for 3 turns', 1, 0, 0, 8, 1.0),
(74, 'Iron Defense', 'Hardens your defense for 3 turns', 1, 0, 0, 7, 1.0),
(75, 'Focus Energy', 'Increases accuracy for 2 turns', 0, 0, 0, 9, 1.0),

-- Stat debuff abilities
(76, 'Intimidate', 'Lowers opponent attack power', 0, 0, 5, 7, 0.85),
(77, 'Armor Break', 'Shatters opponent defense', 0, 0, 8, 5, 0.75),
(78, 'Blind', 'Reduces opponent accuracy drastically', 1, 0, 3, 8, 0.90),

-- Charging moves (high power, takes 2 turns)
(79, 'Solar Beam', 'Charge turn 1, unleash devastating beam turn 2', 1, 0, 30, 5, 0.90),
(80, 'Skull Bash', 'Charge turn 1, massive physical damage turn 2', 0, 0, 28, 6, 0.85),
(81, 'Hyper Beam', 'Ultimate charging attack with massive damage', 1, 0, 35, 4, 0.80),

-- Combo abilities (multiple status effects)
(82, 'Dragon Rage', 'Burns and lowers defense', 1, 0, 10, 5, 0.75),
(83, 'Cursed Slash', 'Damages and reduces attack power', 0, 0, 11, 6, 0.70),
(84, 'War Cry', 'Boosts your attack and defense', 0, 0, 0, 8, 1.0),

-- Utility abilities
(85, 'Smoke Screen', 'Increases your evasion', 0, 0, 0, 10, 1.0),
(86, 'Protect', 'Shield yourself from damage for 1 turn', 1, 0, 0, 9, 1.0),
(87, 'Recover Plus', 'Heal and boost defense', 0, 1, 15, 5, 0.95);

SET IDENTITY_INSERT [dbo].[Abilities] OFF;

-- Now add the status effect configurations
-- Status Effect Type IDs:
-- 0=Burn, 1=Poison, 2=Bleed
-- 3=AttackUp, 4=AttackDown, 5=DefenseUp, 6=DefenseDown
-- 7=TechAttackUp, 8=TechAttackDown, 9=TechDefenseUp, 10=TechDefenseDown
-- 11=AccuracyUp, 12=AccuracyDown, 13=EvasionUp, 14=EvasionDown
-- 15=Charging, 16=HealBlock, 17=Stunned, 18=Protected

INSERT INTO [dbo].[AbilityStatusEffects] (AbilityId, StatusEffectType, Duration, Power, Modifier, ApplyChance, ApplyToSelf, RequiresCharging)
VALUES
-- Ability 69 - Flame Strike (Burn)
(69, 0, 3, 5, 1.0, 0.5, 0, 0),

-- Ability 70 - Inferno Blast (Strong Burn)
(70, 0, 4, 7, 1.0, 1.0, 0, 0),

-- Ability 71 - Toxic Strike (Poison + Heal Block)
(71, 1, 3, 4, 1.0, 0.7, 0, 0),
(71, 16, 2, 0, 1.0, 0.5, 0, 0),

-- Ability 72 - Venom Spray (Strong Poison)
(72, 1, 4, 6, 1.0, 0.8, 0, 0),

-- Ability 73 - Power Up (Attack Buff)
(73, 3, 3, 0, 0.5, 1.0, 1, 0),

-- Ability 74 - Iron Defense (Defense Buff)
(74, 5, 3, 0, 0.5, 1.0, 1, 0),

-- Ability 75 - Focus Energy (Accuracy Buff)
(75, 11, 2, 0, 0.3, 1.0, 1, 0),

-- Ability 76 - Intimidate (Attack Debuff)
(76, 4, 3, 0, 0.3, 0.85, 0, 0),

-- Ability 77 - Armor Break (Defense Debuff)
(77, 6, 3, 0, 0.4, 0.75, 0, 0),

-- Ability 78 - Blind (Accuracy Debuff)
(78, 12, 2, 0, 0.4, 0.9, 0, 0),

-- Ability 79 - Solar Beam (Charging)
(79, 15, 1, 0, 1.0, 1.0, 1, 1),

-- Ability 80 - Skull Bash (Charging)
(80, 15, 1, 0, 1.0, 1.0, 1, 1),

-- Ability 81 - Hyper Beam (Charging)
(81, 15, 1, 0, 1.0, 1.0, 1, 1),

-- Ability 82 - Dragon Rage (Burn + Defense Down)
(82, 0, 3, 5, 1.0, 0.6, 0, 0),
(82, 6, 2, 0, 0.3, 0.5, 0, 0),

-- Ability 83 - Cursed Slash (Bleed + Attack Down)
(83, 2, 3, 6, 1.0, 0.7, 0, 0),
(83, 4, 2, 0, 0.25, 0.6, 0, 0),

-- Ability 84 - War Cry (Attack Up + Defense Up)
(84, 3, 3, 0, 0.4, 1.0, 1, 0),
(84, 5, 3, 0, 0.3, 1.0, 1, 0),

-- Ability 85 - Smoke Screen (Evasion Up)
(85, 13, 2, 0, 0.2, 1.0, 1, 0),

-- Ability 86 - Protect (Protected)
(86, 18, 1, 0, 1.0, 1.0, 1, 0),

-- Ability 87 - Recover Plus (Defense Up with heal)
(87, 5, 2, 0, 0.3, 1.0, 1, 0),

-- Add burn effect to existing Ability 26 - Spilled Tea
(26, 0, 3, 4, 1.0, 0.6, 0, 0),

-- Add strong burn to existing Ability 65 - Feuer frei!
(65, 0, 4, 6, 1.0, 0.8, 0, 0);

-- Assign new abilities to characters
-- Character 1 (Ashley) - Gets burn and charging abilities
INSERT INTO [dbo].[CharacterAbilities] (CharacterId, AbilityId)
VALUES 
(1, 69),  -- Flame Strike
(1, 73),  -- Power Up
(1, 76),  -- Intimidate
(1, 79);  -- Solar Beam

-- Character 2 (Banbury) - Gets poison and debuff abilities
DELETE FROM [dbo].[CharacterAbilities] WHERE CharacterId = 2;
INSERT INTO [dbo].[CharacterAbilities] (CharacterId, AbilityId)
VALUES 
(2, 71),  -- Toxic Strike
(2, 74),  -- Iron Defense
(2, 77),  -- Armor Break
(2, 82);  -- Dragon Rage

-- Character 3 (Beattie) - Gets utility and combo abilities
DELETE FROM [dbo].[CharacterAbilities] WHERE CharacterId = 3;
INSERT INTO [dbo].[CharacterAbilities] (CharacterId, AbilityId)
VALUES 
(3, 83),  -- Cursed Slash
(3, 84),  -- War Cry
(3, 85),  -- Smoke Screen
(3, 87);  -- Recover Plus

-- Character 4 (Faisal) - Gets charging and buff abilities
DELETE FROM [dbo].[CharacterAbilities] WHERE CharacterId = 4;
INSERT INTO [dbo].[CharacterAbilities] (CharacterId, AbilityId)
VALUES 
(4, 73),  -- Power Up
(4, 78),  -- Blind
(4, 80),  -- Skull Bash
(4, 86);  -- Protect

PRINT 'Status effects added to database successfully!';
PRINT 'Abilities 69-87 have been added with status effect configurations.';
PRINT 'Characters 1-4 have been updated with new abilities.';
