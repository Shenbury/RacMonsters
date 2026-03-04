-- Example Abilities with Status Effects
-- NOTE: Status effects are configured in code, not in the database
-- This script just adds the abilities to the database
-- You'll need to modify the CharacterRepository or add configuration code to set up status effects

-- Add new abilities
SET IDENTITY_INSERT [dbo].[Abilities] ON;

-- Burn abilities
INSERT INTO [dbo].[Abilities] (Id, Name, Description, IsTech, IsHeal, Power, Speed, Accuracy)
VALUES 
(69, 'Flame Strike', 'A fiery attack that burns the opponent for 3 turns', 0, 0, 8, 6, 0.85),
(70, 'Inferno Blast', 'Massive fire damage with guaranteed burn', 1, 0, 12, 4, 0.70);

-- Poison abilities
INSERT INTO [dbo].[Abilities] (Id, Name, Description, IsTech, IsHeal, Power, Speed, Accuracy)
VALUES 
(71, 'Toxic Strike', 'Poisons the enemy and blocks healing', 0, 0, 6, 7, 0.80),
(72, 'Venom Spray', 'Sprays venom causing poison damage over time', 1, 0, 8, 5, 0.75);

-- Stat buff abilities
INSERT INTO [dbo].[Abilities] (Id, Name, Description, IsTech, IsHeal, Power, Speed, Accuracy)
VALUES 
(73, 'Power Up', 'Increases your attack power for 3 turns', 1, 0, 0, 8, 1.0),
(74, 'Iron Defense', 'Hardens your defense for 3 turns', 1, 0, 0, 7, 1.0),
(75, 'Focus Energy', 'Increases accuracy for 2 turns', 0, 0, 0, 9, 1.0);

-- Stat debuff abilities
INSERT INTO [dbo].[Abilities] (Id, Name, Description, IsTech, IsHeal, Power, Speed, Accuracy)
VALUES 
(76, 'Intimidate', 'Lowers opponent attack power', 0, 0, 5, 7, 0.85),
(77, 'Armor Break', 'Shatters opponent defense', 0, 0, 8, 5, 0.75),
(78, 'Blind', 'Reduces opponent accuracy drastically', 1, 0, 3, 8, 0.90);

-- Charging moves (high power, takes 2 turns)
INSERT INTO [dbo].[Abilities] (Id, Name, Description, IsTech, IsHeal, Power, Speed, Accuracy)
VALUES 
(79, 'Solar Beam', 'Charge turn 1, unleash devastating beam turn 2', 1, 0, 30, 5, 0.90),
(80, 'Skull Bash', 'Charge turn 1, massive physical damage turn 2', 0, 0, 28, 6, 0.85),
(81, 'Hyper Beam', 'Ultimate charging attack with massive damage', 1, 0, 35, 4, 0.80);

-- Combo abilities (multiple status effects)
INSERT INTO [dbo].[Abilities] (Id, Name, Description, IsTech, IsHeal, Power, Speed, Accuracy)
VALUES 
(82, 'Dragon Rage', 'Burns and lowers defense', 1, 0, 10, 5, 0.75),
(83, 'Cursed Slash', 'Damages and reduces attack power', 0, 0, 11, 6, 0.70),
(84, 'War Cry', 'Boosts your attack and defense', 0, 0, 0, 8, 1.0);

-- Utility abilities
INSERT INTO [dbo].[Abilities] (Id, Name, Description, IsTech, IsHeal, Power, Speed, Accuracy)
VALUES 
(85, 'Smoke Screen', 'Increases your evasion', 0, 0, 0, 10, 1.0),
(86, 'Protect', 'Shield yourself from damage for 1 turn', 1, 0, 0, 9, 1.0),
(87, 'Recover Plus', 'Heal and boost defense', 0, 1, 15, 5, 0.95);

SET IDENTITY_INSERT [dbo].[Abilities] OFF;

-- Example character ability mappings
-- Assign these new abilities to a character (character 1 - Ashley)
INSERT INTO [dbo].[CharacterAbilities] (CharacterId, AbilityId)
VALUES 
(1, 69),  -- Flame Strike
(1, 73),  -- Power Up
(1, 76),  -- Intimidate
(1, 79);  -- Solar Beam

-- Another character example (character 2 - Banbury)
INSERT INTO [dbo].[CharacterAbilities] (CharacterId, AbilityId)
VALUES 
(2, 71),  -- Toxic Strike
(2, 74),  -- Iron Defense
(2, 77),  -- Armor Break
(2, 82);  -- Dragon Rage

PRINT 'Status effect abilities added successfully!';
PRINT 'Remember to configure status effects in code for these abilities.';
