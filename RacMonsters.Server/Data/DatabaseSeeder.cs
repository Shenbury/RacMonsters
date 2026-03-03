using Microsoft.EntityFrameworkCore;

namespace RacMonsters.Server.Data
{
    public class DatabaseSeeder
    {
        private readonly RacMonstersDbContext _db;

        public DatabaseSeeder(RacMonstersDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            // Abilities
            if (! _db.Abilities.Any())
            {
                var abilities = new[]
                {
                    new AbilityEntity { Id = 1, Name = "GOAL!", Description = "Kick your opponent in the nuts and celebrate", IsTech = false, IsHeal = false, Power = 6, Speed = 8, Accuracy = 0.9 },
                    new AbilityEntity { Id = 2, Name = "Hulk Smash", Description = "A powerful smash attack.", IsTech = true, IsHeal = false, Power = 11, Speed = 4, Accuracy = 0.85 },
                    new AbilityEntity { Id = 3, Name = "Cheeky Cuppa", Description = "Restore some health with a cuppa.", IsTech = true, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 4, Name = "Story Time", Description = "Force your opponent to read a badly worded user story", IsTech = true, IsHeal = false, Power = 13, Speed = 2, Accuracy = 0.6 },
                    new AbilityEntity { Id = 5, Name = "MicroTalk", Description = "Talks the praises of microsoft.", IsTech = true, IsHeal = false, Power = 7, Speed = 8, Accuracy = 0.9 },
                    new AbilityEntity { Id = 6, Name = "PR Review", Description = "Nick reviews your PR and call it Bollocks", IsTech = true, IsHeal = false, Power = 11, Speed = 4, Accuracy = 0.85 },
                    new AbilityEntity { Id = 7, Name = "Hot Choccy", Description = "Restore some health with a Hot Choccy.", IsTech = true, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 8, Name = "Jaguar", Description = "Becomes a jaguar and speeds at you at 120MPH", IsTech = false, IsHeal = false, Power = 13, Speed = 2, Accuracy = 0.6 },
                    new AbilityEntity { Id = 9, Name = "Home DIY", Description = "Forces you to assist with DIY causing an accident with a saw", IsTech = false, IsHeal = false, Power = 6, Speed = 8, Accuracy = 0.9 },
                    new AbilityEntity { Id = 10, Name = "Hyrox", Description = "Join a hyrox session leaving you delapadated", IsTech = false, IsHeal = false, Power = 9, Speed = 4, Accuracy = 0.85 },
                    new AbilityEntity { Id = 11, Name = "Mushroom Munch", Description = "Restore some health with a shroom.", IsTech = false, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 12, Name = "New FE Package", Description = "Preaches about a new FE Package derailing your workflow", IsTech = true, IsHeal = false, Power = 9, Speed = 2, Accuracy = 0.75 },
                    new AbilityEntity { Id = 13, Name = "Homebrew Tea", Description = "Drink some hombrew tea", IsTech = false, IsHeal = true, Power = 9, Speed = 7, Accuracy = 0.85 },
                    new AbilityEntity { Id = 14, Name = "Kebabby", Description = "Throws a kebab stick at you attempting to poke your eye out", IsTech = false, IsHeal = false, Power = 8, Speed = 7, Accuracy = 0.85 },
                    new AbilityEntity { Id = 15, Name = "NPM Critical Dependency", Description = "You discover a vulnerability in your NPM dependencies", IsTech = true, IsHeal = false, Power = 10, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 16, Name = "Bad Horror Movie", Description = "Forces you to watch a terrible horror movie", IsTech = false, IsHeal = false, Power = 9, Speed = 2, Accuracy = 0.75 },
                    new AbilityEntity { Id = 17, Name = "User Research", Description = "Conducts user research to disprove your theory", IsTech = true, IsHeal = false, Power = 10, Speed = 7, Accuracy = 0.9 },
                    new AbilityEntity { Id = 18, Name = "Throw Miniature", Description = "Throws a freshly painted warhammer 40k space marine at your eyeball", IsTech = false, IsHeal = false, Power = 11, Speed = 7, Accuracy = 0.85 },
                    new AbilityEntity { Id = 19, Name = "Puff Puff Vape", Description = "Blows a phat cloud from his vape", IsTech = true, IsHeal = true, Power = 12, Speed = 5, Accuracy = 0.8 },
                    new AbilityEntity { Id = 20, Name = "Force feed dog turd", Description = "Force feed dog turd from this shoe into the opponents mouth", IsTech = false, IsHeal = false, Power = 14, Speed = 2, Accuracy = 0.6 },
                    new AbilityEntity { Id = 21, Name = "Notes for Monday", Description = "Leaves a large stack of notes for the opponent to sort out on monday", IsTech = true, IsHeal = false, Power = 9, Speed = 7, Accuracy = 0.75 },
                    new AbilityEntity { Id = 22, Name = "Scratch one Grub", Description = "Pulls out a chainsaw swings it at his opponent", IsTech = false, IsHeal = false, Power = 20, Speed = 3, Accuracy = 0.3 },
                    new AbilityEntity { Id = 23, Name = "Rugby Tackle", Description = "Takedown the enemy with a rugby tackle", IsTech = false, IsHeal = false, Power = 9, Speed = 5, Accuracy = 0.75 },
                    new AbilityEntity { Id = 24, Name = "Coffee", Description = "Drinks a cup of coffee to regain focus and energy", IsTech = false, IsHeal = true, Power = 11, Speed = 4, Accuracy = 0.8 },
                    new AbilityEntity { Id = 25, Name = "Beliggerent Banjo", Description = "Plays a newly discovered band's beliggerent banjo to inflict ear pain on the opponent", IsTech = false, IsHeal = false, Power = 9, Speed = 7, Accuracy = 0.75 },
                    new AbilityEntity { Id = 26, Name = "Spilled Tea", Description = "Spills hot tea on the opponent causing burns", IsTech = false, IsHeal = false, Power = 10, Speed = 3, Accuracy = 0.7 },
                    new AbilityEntity { Id = 27, Name = "Take hat off", Description = "Takes his hat off revealing embracing his baldness", IsTech = false, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.85 },
                    new AbilityEntity { Id = 28, Name = "3 Amigos", Description = "Call upon 2 of your team to have a 3 amigos and decide the best way to defeat the opponent", IsTech = true, IsHeal = false, Power = 12, Speed = 4, Accuracy = 0.75 },
                    new AbilityEntity { Id = 29, Name = "Datalake", Description = "Drown the user in data so they cant breathe", IsTech = true, IsHeal = false, Power = 11, Speed = 3, Accuracy = 0.65 },
                    new AbilityEntity { Id = 30, Name = "Ah huh huh", Description = "In full Johnny Bravo fashion he steals your girl", IsTech = false, IsHeal = false, Power = 11, Speed = 5, Accuracy = 0.75 },
                    new AbilityEntity { Id = 31, Name = "Snowflake", Description = "Throws a snowflake data query at you at lighting speed", IsTech = true, IsHeal = false, Power = 8, Speed = 9, Accuracy = 0.8 },
                    new AbilityEntity { Id = 32, Name = "Energy Drink", Description = "Chugs an energy drink to regain stamina", IsTech = false, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.8 },
                    new AbilityEntity { Id = 33, Name = "P1", Description = "Alert of a P1 bringing everyone to work on their day off", IsTech = true, IsHeal = false, Power = 15, Speed = 3, Accuracy = 0.55 },
                    new AbilityEntity { Id = 34, Name = "Last Minute Request", Description = "Request a last minute change causing you to work overtime", IsTech = true, IsHeal = false, Power = 9, Speed = 5, Accuracy = 0.75 },
                    new AbilityEntity { Id = 35, Name = "Eat Smoked Meat", Description = "Eats delicious smoked meat to restore energy", IsTech = false, IsHeal = true, Power = 10, Speed = 6, Accuracy = 0.8 },
                    new AbilityEntity { Id = 36, Name = "Patrol Smash", Description = "Smashes through your car with a patrol van", IsTech = false, IsHeal = false, Power = 14, Speed = 2, Accuracy = 0.65 },
                    new AbilityEntity { Id = 37, Name = "PBomb", Description = "Created a prototype bomb in 2 hours", IsTech = true, IsHeal = false, Power = 12, Speed = 6, Accuracy = 0.75 },
                    new AbilityEntity { Id = 38, Name = "Lie in", Description = "Have a nice lie in", IsTech = false, IsHeal = true, Power = 12, Speed = 4, Accuracy = 0.7 },
                    new AbilityEntity { Id = 39, Name = "Your Mom", Description = "Tells a your mom joke", IsTech = false, IsHeal = false, Power = 9, Speed = 5, Accuracy = 0.75 },
                    new AbilityEntity { Id = 40, Name = "Do the Washing", Description = "Forces opponent into doing his washing through online blackmail", IsTech = true, IsHeal = false, Power = 12, Speed = 4, Accuracy = 0.75 },
                    new AbilityEntity { Id = 41, Name = "Run Circles", Description = "He runs circles around you breaking your neck", IsTech = true, IsHeal = false, Power = 11, Speed = 7, Accuracy = 0.75 },
                    new AbilityEntity { Id = 42, Name = "Life Debug", Description = "Critiques your life choices in the biggest debug session", IsTech = true, IsHeal = false, Power = 20, Speed = 3, Accuracy = 0.35 },
                    new AbilityEntity { Id = 43, Name = "Lucazade Chug", Description = "Chugs lucazade to gain some energy", IsTech = true, IsHeal = true, Power = 11, Speed = 5, Accuracy = 0.8 },
                    new AbilityEntity { Id = 44, Name = "Pay review", Description = "Gives you a pay review and informs you of a 15% decrease causing mental anguish", IsTech = true, IsHeal = false, Power = 12, Speed = 4, Accuracy = 0.75 },
                    new AbilityEntity { Id = 45, Name = "Sick em'", Description = "Robodog is set to attack mode eviscearting the closest muscle tissue", IsTech = true, IsHeal = false, Power = 10, Speed = 6, Accuracy = 0.75 },
                    new AbilityEntity { Id = 46, Name = "Brain Rewrite", Description = "Rewrites the opponent's brain causing major damage", IsTech = true, IsHeal = false, Power = 20, Speed = 3, Accuracy = 0.35 },
                    new AbilityEntity { Id = 47, Name = "Living on a Prayer", Description = "Sings bon jovi revitilizing her power", IsTech = false, IsHeal = true, Power = 11, Speed = 5, Accuracy = 0.8 },
                    new AbilityEntity { Id = 48, Name = "Back handed slap", Description = "Slaps them in the face with back of the hand", IsTech = false, IsHeal = false, Power = 8, Speed = 5, Accuracy = 0.75 },
                    new AbilityEntity { Id = 49, Name = "Man Overboard", Description = "The opponent is thrown overboard", IsTech = false, IsHeal = false, Power = 10, Speed = 6, Accuracy = 0.75 },
                    new AbilityEntity { Id = 50, Name = "Blackspot", Description = "You hand them the blackspot potentially causing a great sickness", IsTech = false, IsHeal = false, Power = 20, Speed = 3, Accuracy = 0.35 },
                    new AbilityEntity { Id = 51, Name = "Rum & Crackers", Description = "Eat rum and crackers with the other scallywags", IsTech = false, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.8 },
                    new AbilityEntity { Id = 52, Name = "Patrol Move", Description = "Drops a patrol on your head through the quick patrol dispatch machine", IsTech = true, IsHeal = false, Power = 12, Speed = 3, Accuracy = 0.75 },
                    new AbilityEntity { Id = 53, Name = "Roundhouse Kick", Description = "Delivers a powerful roundhouse kick", IsTech = false, IsHeal = false, Power = 11, Speed = 6, Accuracy = 0.7 },
                    new AbilityEntity { Id = 54, Name = "Quick attack", Description = "Gives a last minute request to attack", IsTech = false, IsHeal = false, Power = 7, Speed = 10, Accuracy = 0.9 },
                    new AbilityEntity { Id = 55, Name = "Drink a pint of wine", Description = "Enjoys a pint of wine to boost morale", IsTech = false, IsHeal = true, Power = 11, Speed = 5, Accuracy = 0.8 },
                    new AbilityEntity { Id = 56, Name = "Transcribe Hell", Description = "Pulls up a log where you said something inappropriate causing you to visit HR", IsTech = true, IsHeal = false, Power = 12, Speed = 4, Accuracy = 0.75 },
                    new AbilityEntity { Id = 57, Name = "Laptop Explosion Virus", Description = "Gives a compure virus destroying your tech equipment", IsTech = true, IsHeal = false, Power = 10, Speed = 6, Accuracy = 0.8 },
                    new AbilityEntity { Id = 58, Name = "Vibe Station", Description = "Vibes to the latest Swifty James", IsTech = true, IsHeal = true, Power = 9, Speed = 10, Accuracy = 0.85 },
                    new AbilityEntity { Id = 59, Name = "Swift Facial Redesign", Description = "Redesigns someones face with his fists", IsTech = false, IsHeal = false, Power = 12, Speed = 6, Accuracy = 0.75 },
                    new AbilityEntity { Id = 60, Name = "Chef Up a Storm", Description = "Created a swirling storm of meats and food", IsTech = true, IsHeal = false, Power = 10, Speed = 4, Accuracy = 0.75 },
                    new AbilityEntity { Id = 61, Name = "Chainsaw Slice", Description = "Cut them in half with the chainsaw", IsTech = false, IsHeal = false, Power = 14, Speed = 3, Accuracy = 0.65 },
                    new AbilityEntity { Id = 62, Name = "Minion Munch", Description = "Set minions on your enemy devouring your flesh", IsTech = true, IsHeal = false, Power = 12, Speed = 6, Accuracy = 0.8 },
                    new AbilityEntity { Id = 63, Name = "Blood Chugg", Description = "Drinks the blood of the enemies", IsTech = true, IsHeal = true, Power = 11, Speed = 4, Accuracy = 0.8 },
                    new AbilityEntity { Id = 64, Name = "Charged Up!", Description = "Charges at the enemy with full force emitting electricity", IsTech = true, IsHeal = false, Power = 13, Speed = 5, Accuracy = 0.75 },
                    new AbilityEntity { Id = 65, Name = "Feuer frei!", Description = "Sets the enemy on fire with a powerful flamethrower", IsTech = false, IsHeal = false, Power = 14, Speed = 4, Accuracy = 0.7 },
                    new AbilityEntity { Id = 66, Name = "Epic Bass Solo", Description = "Plays an epic bass solo that rejuvenates himself", IsTech = false, IsHeal = true, Power = 9, Speed = 8, Accuracy = 0.85 },
                    new AbilityEntity { Id = 67, Name = "Bass Smash", Description = "Smashes the enemy with a powerful bass attack", IsTech = false, IsHeal = false, Power = 13, Speed = 5, Accuracy = 0.75 },
                    new AbilityEntity { Id = 68, Name = "Kummerspeck!", Description = "Hurl abuse at your enemy making them fat from emotional trauma", IsTech = false, IsHeal = false, Power = 10, Speed = 7, Accuracy = 0.85 },
                };

                // Abilities table uses identity on Id; enable IDENTITY_INSERT to allow explicit Ids in seed
                try
                {
                    await _db.Database.OpenConnectionAsync();
                    await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Abilities] ON");
                    _db.Abilities.AddRange(abilities);
                    await _db.SaveChangesAsync();
                    await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Abilities] OFF");
                }
                finally
                {

                }
            }

            // Characters and mappings
            if (! _db.Characters.Any())
            {
                var characters = new[]
                {
                    new CharacterEntity { Id = 1, Name = "Ashley", ImageUrl = "public/Ashley.png", MaxHealth = 50, CurrentHealth = 50, Attack = 6, Defense = 6, TechAttack = 5, TechDefense = 5 },
                    new CharacterEntity { Id = 2, Name = "Banbury", ImageUrl = "public/Banbury.png", MaxHealth = 40, CurrentHealth = 40, Attack = 6, Defense = 6, TechAttack = 11, TechDefense = 7 },
                    new CharacterEntity { Id = 3, Name = "Beattie", ImageUrl = "public/Beattie.png", MaxHealth = 55, CurrentHealth = 55, Attack = 4, Defense = 8, TechAttack = 3, TechDefense = 5 },
                    new CharacterEntity { Id = 4, Name = "Faisal", ImageUrl = "public/Faisal.png", MaxHealth = 52, CurrentHealth = 52, Attack = 8, Defense = 8, TechAttack = 7, TechDefense = 8 },
                    new CharacterEntity { Id = 5, Name = "JP", ImageUrl = "public/JP.png", MaxHealth = 45, CurrentHealth = 45, Attack = 10, Defense = 8, TechAttack = 6, TechDefense = 7 },
                    new CharacterEntity { Id = 6, Name = "Langdon", ImageUrl = "public/Langdon.png", MaxHealth = 48, CurrentHealth = 48, Attack = 7, Defense = 10, TechAttack = 3, TechDefense = 6 },
                    new CharacterEntity { Id = 7, Name = "Lilley", ImageUrl = "public/Lilley.png", MaxHealth = 45, CurrentHealth = 45, Attack = 9, Defense = 7, TechAttack = 5, TechDefense = 5 },
                    new CharacterEntity { Id = 8, Name = "Nunan", ImageUrl = "public/Nunan.png", MaxHealth = 38, CurrentHealth = 38, Attack = 4, Defense = 4, TechAttack = 12, TechDefense = 8 },
                    new CharacterEntity { Id = 9, Name = "Sam", ImageUrl = "public/Sam.png", MaxHealth = 43, CurrentHealth = 43, Attack = 10, Defense = 7, TechAttack = 5, TechDefense = 4 },
                    new CharacterEntity { Id = 10, Name = "Simon", ImageUrl = "public/Simon.png", MaxHealth = 44, CurrentHealth = 44, Attack = 5, Defense = 9, TechAttack = 12, TechDefense = 8 },
                    new CharacterEntity { Id = 11, Name = "Paul", ImageUrl = "public/Paul2.png", MaxHealth = 42, CurrentHealth = 42, Attack = 8, Defense = 8, TechAttack = 10, TechDefense = 7 },
                    new CharacterEntity { Id = 12, Name = "Charl", ImageUrl = "public/Charl.png", MaxHealth = 42, CurrentHealth = 42, Attack = 7, Defense = 8, TechAttack = 6, TechDefense = 12 },
                    new CharacterEntity { Id = 13, Name = "Barnes", ImageUrl = "public/Barnes.png", MaxHealth = 42, CurrentHealth = 42, Attack = 8, Defense = 8, TechAttack = 6, TechDefense = 6 },
                    new CharacterEntity { Id = 14, Name = "Belassie", ImageUrl = "public/Belassie.png", MaxHealth = 35, CurrentHealth = 35, Attack = 12, Defense = 9, TechAttack = 5, TechDefense = 5 },
                    new CharacterEntity { Id = 15, Name = "Sailesh", ImageUrl = "public/Sailesh.png", MaxHealth = 48, CurrentHealth = 48, Attack = 8, Defense = 5, TechAttack = 10, TechDefense = 8 },
                    new CharacterEntity { Id = 16, Name = "Andy", ImageUrl = "public/Andy.png", MaxHealth = 46, CurrentHealth = 46, Attack = 4, Defense = 10, TechAttack = 11, TechDefense = 7 },
                    new CharacterEntity { Id = 17, Name = "Aaron", ImageUrl = "public/Aaron.png", MaxHealth = 40, CurrentHealth = 40, Attack = 12, Defense = 4, TechAttack = 7, TechDefense = 5 }
                };

                // Characters table uses identity on Id; enable IDENTITY_INSERT to allow explicit Ids in seed
                try
                {
                    await _db.Database.OpenConnectionAsync();
                    await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Characters] ON");
                    _db.Characters.AddRange(characters);
                    await _db.SaveChangesAsync();
                    await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Characters] OFF");
                }
                finally
                {

                }

                var mappings = new[]
                {
                    new CharacterAbilityEntity { CharacterId = 1, AbilityId = 1 },
                    new CharacterAbilityEntity { CharacterId = 1, AbilityId = 2 },
                    new CharacterAbilityEntity { CharacterId = 1, AbilityId = 3 },
                    new CharacterAbilityEntity { CharacterId = 1, AbilityId = 4 },
                    new CharacterAbilityEntity { CharacterId = 2, AbilityId = 5 },
                    new CharacterAbilityEntity { CharacterId = 2, AbilityId = 6 },
                    new CharacterAbilityEntity { CharacterId = 2, AbilityId = 7 },
                    new CharacterAbilityEntity { CharacterId = 2, AbilityId = 8 },
                    new CharacterAbilityEntity { CharacterId = 3, AbilityId = 9 },
                    new CharacterAbilityEntity { CharacterId = 3, AbilityId = 10 },
                    new CharacterAbilityEntity { CharacterId = 3, AbilityId = 11 },
                    new CharacterAbilityEntity { CharacterId = 3, AbilityId = 12 },
                    new CharacterAbilityEntity { CharacterId = 4, AbilityId = 13 },
                    new CharacterAbilityEntity { CharacterId = 4, AbilityId = 14 },
                    new CharacterAbilityEntity { CharacterId = 4, AbilityId = 15 },
                    new CharacterAbilityEntity { CharacterId = 4, AbilityId = 16 },
                    new CharacterAbilityEntity { CharacterId = 5, AbilityId = 17 },
                    new CharacterAbilityEntity { CharacterId = 5, AbilityId = 18 },
                    new CharacterAbilityEntity { CharacterId = 5, AbilityId = 19 },
                    new CharacterAbilityEntity { CharacterId = 5, AbilityId = 20 },
                    new CharacterAbilityEntity { CharacterId = 6, AbilityId = 21 },
                    new CharacterAbilityEntity { CharacterId = 6, AbilityId = 22 },
                    new CharacterAbilityEntity { CharacterId = 6, AbilityId = 23 },
                    new CharacterAbilityEntity { CharacterId = 6, AbilityId = 24 },
                    new CharacterAbilityEntity { CharacterId = 7, AbilityId = 25 },
                    new CharacterAbilityEntity { CharacterId = 7, AbilityId = 26 },
                    new CharacterAbilityEntity { CharacterId = 7, AbilityId = 27 },
                    new CharacterAbilityEntity { CharacterId = 7, AbilityId = 28 },
                    new CharacterAbilityEntity { CharacterId = 8, AbilityId = 29 },
                    new CharacterAbilityEntity { CharacterId = 8, AbilityId = 30 },
                    new CharacterAbilityEntity { CharacterId = 8, AbilityId = 31 },
                    new CharacterAbilityEntity { CharacterId = 8, AbilityId = 32 },
                    new CharacterAbilityEntity { CharacterId = 9, AbilityId = 33 },
                    new CharacterAbilityEntity { CharacterId = 9, AbilityId = 34 },
                    new CharacterAbilityEntity { CharacterId = 9, AbilityId = 35 },
                    new CharacterAbilityEntity { CharacterId = 9, AbilityId = 36 },
                    new CharacterAbilityEntity { CharacterId = 10, AbilityId = 37 },
                    new CharacterAbilityEntity { CharacterId = 10, AbilityId = 38 },
                    new CharacterAbilityEntity { CharacterId = 10, AbilityId = 39 },
                    new CharacterAbilityEntity { CharacterId = 10, AbilityId = 40 },
                    new CharacterAbilityEntity { CharacterId = 11, AbilityId = 41 },
                    new CharacterAbilityEntity { CharacterId = 11, AbilityId = 42 },
                    new CharacterAbilityEntity { CharacterId = 11, AbilityId = 43 },
                    new CharacterAbilityEntity { CharacterId = 11, AbilityId = 44 },
                    new CharacterAbilityEntity { CharacterId = 12, AbilityId = 45 },
                    new CharacterAbilityEntity { CharacterId = 12, AbilityId = 46 },
                    new CharacterAbilityEntity { CharacterId = 12, AbilityId = 47 },
                    new CharacterAbilityEntity { CharacterId = 12, AbilityId = 48 },
                    new CharacterAbilityEntity { CharacterId = 13, AbilityId = 49 },
                    new CharacterAbilityEntity { CharacterId = 13, AbilityId = 50 },
                    new CharacterAbilityEntity { CharacterId = 13, AbilityId = 51 },
                    new CharacterAbilityEntity { CharacterId = 13, AbilityId = 52 },
                    new CharacterAbilityEntity { CharacterId = 14, AbilityId = 53 },
                    new CharacterAbilityEntity { CharacterId = 14, AbilityId = 54 },
                    new CharacterAbilityEntity { CharacterId = 14, AbilityId = 55 },
                    new CharacterAbilityEntity { CharacterId = 14, AbilityId = 56 },
                    new CharacterAbilityEntity { CharacterId = 15, AbilityId = 57 },
                    new CharacterAbilityEntity { CharacterId = 15, AbilityId = 58 },
                    new CharacterAbilityEntity { CharacterId = 15, AbilityId = 59 },
                    new CharacterAbilityEntity { CharacterId = 15, AbilityId = 60 },
                    new CharacterAbilityEntity { CharacterId = 16, AbilityId = 61 },
                    new CharacterAbilityEntity { CharacterId = 16, AbilityId = 62 },
                    new CharacterAbilityEntity { CharacterId = 16, AbilityId = 63 },
                    new CharacterAbilityEntity { CharacterId = 16, AbilityId = 64 },
                    new CharacterAbilityEntity { CharacterId = 17, AbilityId = 65 },
                    new CharacterAbilityEntity { CharacterId = 17, AbilityId = 66 },
                    new CharacterAbilityEntity { CharacterId = 17, AbilityId = 67 },
                    new CharacterAbilityEntity { CharacterId = 17, AbilityId = 68 },
                };

                _db.CharacterAbilities.AddRange(mappings);
                await _db.SaveChangesAsync();
            }
        }
    }
}
