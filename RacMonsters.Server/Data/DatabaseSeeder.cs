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
                    new AbilityEntity { Id = 1, Name = "GOAL!", Description = "Kick your opponent in the nuts and celebrate", IsTech = false, IsHeal = false, Power = 6, Speed = 8, Accuracy = 0.95 },
                    new AbilityEntity { Id = 2, Name = "Hulk Smash", Description = "A powerful smash attack.", IsTech = true, IsHeal = false, Power = 11, Speed = 4, Accuracy = 0.75 },
                    new AbilityEntity { Id = 3, Name = "Cheeky Cuppa", Description = "Restore some health with a cuppa.", IsTech = true, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.95 },
                    new AbilityEntity { Id = 4, Name = "Story Time", Description = "Force your opponent to read a badly worded user story", IsTech = true, IsHeal = false, Power = 13, Speed = 2, Accuracy = 0.55 },
                    new AbilityEntity { Id = 5, Name = "MicroTalk", Description = "Talks the praises of microsoft.", IsTech = true, IsHeal = false, Power = 7, Speed = 8, Accuracy = 0.95 },
                    new AbilityEntity { Id = 6, Name = "PR Review", Description = "Nick reviews your PR and call it Bollocks", IsTech = true, IsHeal = false, Power = 11, Speed = 4, Accuracy = 0.75 },
                    new AbilityEntity { Id = 7, Name = "Hot Choccy", Description = "Restore some health with a Hot Choccy.", IsTech = true, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.95 },
                    new AbilityEntity { Id = 8, Name = "Jaguar", Description = "Becomes a jaguar and speeds at you at 120MPH", IsTech = false, IsHeal = false, Power = 13, Speed = 2, Accuracy = 0.5 },
                    new AbilityEntity { Id = 9, Name = "Home DIY", Description = "Forces you to assist with DIY causing an accident with a saw", IsTech = false, IsHeal = false, Power = 8, Speed = 8, Accuracy = 0.95 }, // BUFFED: Power 6->8
                    new AbilityEntity { Id = 10, Name = "Hyrox", Description = "Join a hyrox session leaving you delapadated", IsTech = false, IsHeal = false, Power = 11, Speed = 4, Accuracy = 0.85 }, // BUFFED: Power 9->11, Acc 0.8->0.85
                    new AbilityEntity { Id = 11, Name = "Mushroom Munch", Description = "Restore some health with a shroom.", IsTech = false, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.95 },
                    new AbilityEntity { Id = 12, Name = "New FE Package", Description = "Preaches about a new FE Package derailing your workflow", IsTech = true, IsHeal = false, Power = 11, Speed = 3, Accuracy = 0.75 }, // BUFFED: Power 9->11, Speed 2->3, Acc 0.65->0.75
                    new AbilityEntity { Id = 13, Name = "Homebrew Tea", Description = "Drink some hombrew tea", IsTech = false, IsHeal = true, Power = 9, Speed = 7, Accuracy = 0.95 },
                    new AbilityEntity { Id = 14, Name = "Kebabby", Description = "Throws a kebab stick at you attempting to poke your eye out", IsTech = false, IsHeal = false, Power = 8, Speed = 7, Accuracy = 0.9 },
                    new AbilityEntity { Id = 15, Name = "NPM Critical Dependency", Description = "You discover a vulnerability in your NPM dependencies", IsTech = true, IsHeal = false, Power = 10, Speed = 5, Accuracy = 0.85 },
                    new AbilityEntity { Id = 16, Name = "Bad Horror Movie", Description = "Forces you to watch a terrible horror movie", IsTech = false, IsHeal = false, Power = 9, Speed = 2, Accuracy = 0.7 },
                    new AbilityEntity { Id = 17, Name = "User Research", Description = "Conducts user research to disprove your theory", IsTech = true, IsHeal = false, Power = 10, Speed = 7, Accuracy = 0.9 },
                    new AbilityEntity { Id = 18, Name = "Throw Miniature", Description = "Throws a freshly painted warhammer 40k space marine at your eyeball", IsTech = false, IsHeal = false, Power = 11, Speed = 7, Accuracy = 0.8 },
                    new AbilityEntity { Id = 19, Name = "Puff Puff Vape", Description = "Blows a phat cloud from his vape", IsTech = true, IsHeal = true, Power = 12, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 20, Name = "Force feed dog turd", Description = "Force feed dog turd from this shoe into the opponents mouth", IsTech = false, IsHeal = false, Power = 14, Speed = 2, Accuracy = 0.55 },
                    new AbilityEntity { Id = 21, Name = "Notes for Monday", Description = "Leaves a large stack of notes for the opponent to sort out on monday", IsTech = true, IsHeal = false, Power = 9, Speed = 7, Accuracy = 0.85 },
                    new AbilityEntity { Id = 22, Name = "Scratch one Grub", Description = "Pulls out a chainsaw swings it at his opponent", IsTech = false, IsHeal = false, Power = 20, Speed = 3, Accuracy = 0.35 },
                    new AbilityEntity { Id = 23, Name = "Rugby Tackle", Description = "Takedown the enemy with a rugby tackle", IsTech = false, IsHeal = false, Power = 9, Speed = 5, Accuracy = 0.85 },
                    new AbilityEntity { Id = 24, Name = "Coffee", Description = "Drinks a cup of coffee to regain focus and energy", IsTech = false, IsHeal = true, Power = 11, Speed = 4, Accuracy = 0.9 },
                    new AbilityEntity { Id = 25, Name = "Beliggerent Banjo", Description = "Plays a newly discovered band's beliggerent banjo to inflict ear pain on the opponent", IsTech = false, IsHeal = false, Power = 9, Speed = 7, Accuracy = 0.85 },
                    new AbilityEntity { Id = 26, Name = "Spilled Tea", Description = "Spills hot tea on the opponent causing burns", IsTech = false, IsHeal = false, Power = 10, Speed = 3, Accuracy = 0.75 },
                    new AbilityEntity { Id = 27, Name = "Take hat off", Description = "Takes his hat off revealing embracing his baldness", IsTech = false, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.95 },
                    new AbilityEntity { Id = 28, Name = "3 Amigos", Description = "Call upon 2 of your team to have a 3 amigos and decide the best way to defeat the opponent", IsTech = true, IsHeal = false, Power = 12, Speed = 4, Accuracy = 0.65 },
                    new AbilityEntity { Id = 29, Name = "Datalake", Description = "Drown the user in data so they cant breathe", IsTech = true, IsHeal = false, Power = 13, Speed = 4, Accuracy = 0.8 }, // BUFFED: Power 11->13, Speed 3->4, Acc 0.7->0.8
                    new AbilityEntity { Id = 30, Name = "Ah huh huh", Description = "In full Johnny Bravo fashion he steals your girl", IsTech = false, IsHeal = false, Power = 12, Speed = 5, Accuracy = 0.8 }, // BUFFED: Power 11->12, Acc 0.75->0.8
                    new AbilityEntity { Id = 31, Name = "Snowflake", Description = "Throws a snowflake data query at you at lighting speed", IsTech = true, IsHeal = false, Power = 10, Speed = 9, Accuracy = 0.9 }, // BUFFED: Power 8->10
                    new AbilityEntity { Id = 32, Name = "Energy Drink", Description = "Chugs an energy drink to regain stamina", IsTech = false, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 33, Name = "P1", Description = "Alert of a P1 bringing everyone to work on their day off", IsTech = true, IsHeal = false, Power = 15, Speed = 3, Accuracy = 0.45 },
                    new AbilityEntity { Id = 34, Name = "Last Minute Request", Description = "Request a last minute change causing you to work overtime", IsTech = true, IsHeal = false, Power = 9, Speed = 5, Accuracy = 0.85 },
                    new AbilityEntity { Id = 35, Name = "Eat Smoked Meat", Description = "Eats delicious smoked meat to restore energy", IsTech = false, IsHeal = true, Power = 10, Speed = 6, Accuracy = 0.95 },
                    new AbilityEntity { Id = 36, Name = "Patrol Smash", Description = "Smashes through your car with a patrol van", IsTech = false, IsHeal = false, Power = 14, Speed = 2, Accuracy = 0.55 },
                    new AbilityEntity { Id = 37, Name = "PBomb", Description = "Created a prototype bomb in 2 hours", IsTech = true, IsHeal = false, Power = 12, Speed = 6, Accuracy = 0.7 },
                    new AbilityEntity { Id = 38, Name = "Lie in", Description = "Have a nice lie in", IsTech = false, IsHeal = true, Power = 12, Speed = 4, Accuracy = 0.9 },
                    new AbilityEntity { Id = 39, Name = "Your Mom", Description = "Tells a your mom joke", IsTech = false, IsHeal = false, Power = 9, Speed = 5, Accuracy = 0.85 },
                    new AbilityEntity { Id = 40, Name = "Do the Washing", Description = "Forces opponent into doing his washing through online blackmail", IsTech = true, IsHeal = false, Power = 12, Speed = 4, Accuracy = 0.7 },
                    new AbilityEntity { Id = 41, Name = "Run Circles", Description = "He runs circles around you breaking your neck", IsTech = true, IsHeal = false, Power = 11, Speed = 7, Accuracy = 0.8 },
                    new AbilityEntity { Id = 42, Name = "Life Debug", Description = "Critiques your life choices in the biggest debug session", IsTech = true, IsHeal = false, Power = 20, Speed = 3, Accuracy = 0.3 },
                    new AbilityEntity { Id = 43, Name = "Lucazade Chug", Description = "Chugs lucazade to gain some energy", IsTech = true, IsHeal = true, Power = 11, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 44, Name = "Pay review", Description = "Gives you a pay review and informs you of a 15% decrease causing mental anguish", IsTech = true, IsHeal = false, Power = 12, Speed = 4, Accuracy = 0.65 },
                    new AbilityEntity { Id = 45, Name = "Sick em'", Description = "Robodog is set to attack mode eviscearting the closest muscle tissue", IsTech = true, IsHeal = false, Power = 10, Speed = 6, Accuracy = 0.8 },
                    new AbilityEntity { Id = 46, Name = "Brain Rewrite", Description = "Rewrites the opponent's brain causing major damage", IsTech = true, IsHeal = false, Power = 20, Speed = 3, Accuracy = 0.3 },
                    new AbilityEntity { Id = 47, Name = "Living on a Prayer", Description = "Sings bon jovi revitilizing her power", IsTech = false, IsHeal = true, Power = 11, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 48, Name = "Back handed slap", Description = "Slaps them in the face with back of the hand", IsTech = false, IsHeal = false, Power = 8, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 49, Name = "Man Overboard", Description = "The opponent is thrown overboard", IsTech = false, IsHeal = false, Power = 10, Speed = 6, Accuracy = 0.85 },
                    new AbilityEntity { Id = 50, Name = "Blackspot", Description = "You hand them the blackspot potentially causing a great sickness", IsTech = false, IsHeal = false, Power = 20, Speed = 3, Accuracy = 0.3 },
                    new AbilityEntity { Id = 51, Name = "Rum & Crackers", Description = "Eat rum and crackers with the other scallywags", IsTech = false, IsHeal = true, Power = 10, Speed = 5, Accuracy = 0.95 },
                    new AbilityEntity { Id = 52, Name = "Patrol Move", Description = "Drops a patrol on your head through the quick patrol dispatch machine", IsTech = true, IsHeal = false, Power = 12, Speed = 3, Accuracy = 0.65 },
                    new AbilityEntity { Id = 53, Name = "Roundhouse Kick", Description = "Delivers a powerful roundhouse kick", IsTech = false, IsHeal = false, Power = 13, Speed = 6, Accuracy = 0.8 }, // BUFFED: Power 11->13, Acc 0.75->0.8
                    new AbilityEntity { Id = 54, Name = "Quick attack", Description = "Gives a last minute request to attack", IsTech = false, IsHeal = false, Power = 7, Speed = 10, Accuracy = 0.95 },
                    new AbilityEntity { Id = 55, Name = "Drink a pint of wine", Description = "Enjoys a pint of wine to boost morale", IsTech = false, IsHeal = true, Power = 11, Speed = 5, Accuracy = 0.9 },
                    new AbilityEntity { Id = 56, Name = "Transcribe Hell", Description = "Pulls up a log where you said something inappropriate causing you to visit HR", IsTech = true, IsHeal = false, Power = 14, Speed = 5, Accuracy = 0.75 }, // BUFFED: Power 12->14, Speed 4->5, Acc 0.65->0.75
                    new AbilityEntity { Id = 57, Name = "Laptop Explosion Virus", Description = "Gives a compure virus destroying your tech equipment", IsTech = true, IsHeal = false, Power = 10, Speed = 6, Accuracy = 0.8 },
                    new AbilityEntity { Id = 58, Name = "Vibe Station", Description = "Vibes to the latest Swifty James", IsTech = true, IsHeal = true, Power = 9, Speed = 10, Accuracy = 0.95 },
                    new AbilityEntity { Id = 59, Name = "Swift Facial Redesign", Description = "Redesigns someones face with his fists", IsTech = false, IsHeal = false, Power = 12, Speed = 6, Accuracy = 0.65 },
                    new AbilityEntity { Id = 60, Name = "Chef Up a Storm", Description = "Created a swirling storm of meats and food", IsTech = true, IsHeal = false, Power = 10, Speed = 4, Accuracy = 0.75 },
                    new AbilityEntity { Id = 61, Name = "Chainsaw Slice", Description = "Cut them in half with the chainsaw", IsTech = false, IsHeal = false, Power = 14, Speed = 3, Accuracy = 0.55 },
                    new AbilityEntity { Id = 62, Name = "Minion Munch", Description = "Set minions on your enemy devouring your flesh", IsTech = true, IsHeal = false, Power = 12, Speed = 6, Accuracy = 0.75 },
                    new AbilityEntity { Id = 63, Name = "Blood Chugg", Description = "Drinks the blood of the enemies", IsTech = true, IsHeal = true, Power = 7, Speed = 4, Accuracy = 0.5 },
                    new AbilityEntity { Id = 64, Name = "Charged Up!", Description = "Charges at the enemy with full force emitting electricity", IsTech = true, IsHeal = false, Power = 13, Speed = 5, Accuracy = 0.65 },
                    new AbilityEntity { Id = 65, Name = "Feuer frei!", Description = "Sets the enemy on fire with a powerful flamethrower", IsTech = false, IsHeal = false, Power = 14, Speed = 4, Accuracy = 0.55 },
                    new AbilityEntity { Id = 66, Name = "Epic Bass Solo", Description = "Plays an epic bass solo that rejuvenates himself", IsTech = false, IsHeal = true, Power = 9, Speed = 8, Accuracy = 0.95 },
                    new AbilityEntity { Id = 67, Name = "Bass Smash", Description = "Smashes the enemy with a powerful bass attack", IsTech = false, IsHeal = false, Power = 13, Speed = 5, Accuracy = 0.65 },
                    new AbilityEntity { Id = 68, Name = "Kummerspeck!", Description = "Hurl abuse at your enemy making them fat from emotional trauma", IsTech = false, IsHeal = false, Power = 10, Speed = 7, Accuracy = 0.8 },
                    new AbilityEntity { Id = 69, Name = "Automation Destruction!", Description = "Automate the destruction of their soul", IsTech = true, IsHeal = false, Power = 11, Speed = 6, Accuracy = 0.85 },
                    new AbilityEntity { Id = 70, Name = "Chef Time", Description = "Chop them up like onion", IsTech = false, IsHeal = false, Power = 13, Speed = 4, Accuracy = 0.7 },
                    new AbilityEntity { Id = 71, Name = "Eat a lil snacky", Description = "Enjoys a small snack to regain energy", IsTech = false, IsHeal = true, Power = 9, Speed = 6, Accuracy = 0.95 },
                    new AbilityEntity { Id = 72, Name = "Turn the lights off!", Description = "Turn the lights off causing the enemy to stub their toe", IsTech = false, IsHeal = false, Power = 7, Speed = 9, Accuracy = 0.9 },
                    new AbilityEntity { Id = 73, Name = "Diamond Slice", Description = "Slices the enemy with a diamond-edged weapon", IsTech = false, IsHeal = false, Power = 8, Speed = 5, Accuracy = 0.85 },
                    new AbilityEntity { Id = 74, Name = "Death by cycle tyre", Description = "Hurls a cycle tyre at the enemy causing massive damage", IsTech = false, IsHeal = false, Power = 12, Speed = 3, Accuracy = 0.65 },
                    new AbilityEntity { Id = 75, Name = "Mystery Meat!", Description = "Eats a mysterious piece of meat in front of the enemy", IsTech = false, IsHeal = true, Power = 12, Speed = 4, Accuracy = 0.9 },
                    new AbilityEntity { Id = 76, Name = "Body slam!", Description = "Hurls the enemy to the ground causing massive damage", IsTech = false, IsHeal = false, Power = 6, Speed = 6, Accuracy = 0.95 },
                    new AbilityEntity { Id = 77, Name = "Smoke Screen", Description = "Creates a smoke screen that reduces enemy accuracy", IsTech = false, IsHeal = false, Power = 0, Speed = 10, Accuracy = 0.9 },

                    // Ashley status moves
                    new AbilityEntity { Id = 78, Name = "Yellow Card", Description = "Intimidate the opponent with a yellow card reducing their speed", IsTech = false, IsHeal = false, Power = 0, Speed = 8, Accuracy = 0.95 },
                    new AbilityEntity { Id = 79, Name = "Offsides Confusion", Description = "Confuse the opponent with complex offside rules", IsTech = false, IsHeal = false, Power = 3, Speed = 7, Accuracy = 0.85 },

                    // Banbury status moves
                    new AbilityEntity { Id = 80, Name = "Azure Outage", Description = "Cloud services go down causing tech attack failure", IsTech = true, IsHeal = false, Power = 4, Speed = 6, Accuracy = 0.8 },
                    new AbilityEntity { Id = 81, Name = "Windows Update", Description = "Force a mandatory restart stunning the opponent", IsTech = true, IsHeal = false, Power = 0, Speed = 5, Accuracy = 0.9 },

                    // Beattie status moves
                    new AbilityEntity { Id = 82, Name = "Protein Shake", Description = "Drink a protein shake to boost attack and defense", IsTech = false, IsHeal = false, Power = 0, Speed = 6, Accuracy = 1.0 },
                    new AbilityEntity { Id = 83, Name = "Sawdust Cloud", Description = "Kick up sawdust reducing enemy accuracy", IsTech = false, IsHeal = false, Power = 2, Speed = 8, Accuracy = 0.9 },

                    // Faisal status moves
                    new AbilityEntity { Id = 84, Name = "Food Poisoning", Description = "Serve dodgy food causing poison damage over time", IsTech = false, IsHeal = false, Power = 5, Speed = 6, Accuracy = 0.75 },
                    new AbilityEntity { Id = 85, Name = "Scalding Tea", Description = "Spill boiling tea causing burn damage", IsTech = false, IsHeal = false, Power = 6, Speed = 5, Accuracy = 0.8 },

                    // JP status moves
                    new AbilityEntity { Id = 86, Name = "Call Out", Description = "Call someone out for talking smack reducing their attack", IsTech = false, IsHeal = false, Power = 4, Speed = 7, Accuracy = 0.85 },
                    new AbilityEntity { Id = 87, Name = "Paint Fumes", Description = "Release toxic paint fumes confusing the enemy", IsTech = false, IsHeal = false, Power = 3, Speed = 6, Accuracy = 0.8 },

                    // Langdon status moves
                    new AbilityEntity { Id = 88, Name = "Tuesday Blues", Description = "Demotivate the opponent reducing attack and speed", IsTech = false, IsHeal = false, Power = 3, Speed = 5, Accuracy = 0.85 },
                    new AbilityEntity { Id = 89, Name = "Caffeine Rush", Description = "Chug coffee for a massive speed boost", IsTech = false, IsHeal = false, Power = 0, Speed = 10, Accuracy = 1.0 },

                    // Lilley status moves
                    new AbilityEntity { Id = 90, Name = "Bearded Confidence", Description = "Embrace the beard for increased defense", IsTech = false, IsHeal = false, Power = 0, Speed = 7, Accuracy = 1.0 },
                    new AbilityEntity { Id = 91, Name = "New Priority", Description = "Agile priority change causes bleed but reduces your attack", IsTech = true, IsHeal = false, Power = 8, Speed = 6, Accuracy = 0.8 },

                    // Nunan status moves
                    new AbilityEntity { Id = 92, Name = "Data Overload", Description = "Overwhelm with data causing confusion", IsTech = true, IsHeal = false, Power = 4, Speed = 7, Accuracy = 0.85 },
                    new AbilityEntity { Id = 93, Name = "Moustache Offensive", Description = "Johnny Bravo charm offensive reducing enemy attack", IsTech = false, IsHeal = false, Power = 3, Speed = 8, Accuracy = 0.9 },

                    // Sam status moves
                    new AbilityEntity { Id = 94, Name = "On-Call Stress", Description = "Emanate stress reducing all enemy stats", IsTech = true, IsHeal = false, Power = 5, Speed = 6, Accuracy = 0.8 },
                    new AbilityEntity { Id = 95, Name = "Bad Press", Description = "Charge up a devastating press release", IsTech = true, IsHeal = false, Power = 25, Speed = 4, Accuracy = 0.7 },

                    // Simon status moves
                    new AbilityEntity { Id = 96, Name = "Laid Back Vibes", Description = "Relax to increase defense", IsTech = false, IsHeal = false, Power = 0, Speed = 6, Accuracy = 1.0 },
                    new AbilityEntity { Id = 97, Name = "Weekend Mood", Description = "Spread weekend laziness reducing enemy speed", IsTech = false, IsHeal = false, Power = 2, Speed = 7, Accuracy = 0.9 },

                    // Paul status moves
                    new AbilityEntity { Id = 98, Name = "Code Review", Description = "Point out flaws reducing enemy accuracy", IsTech = true, IsHeal = false, Power = 4, Speed = 6, Accuracy = 0.85 },
                    new AbilityEntity { Id = 99, Name = "Caffeine Crash", Description = "Speed boost followed by dramatic crash", IsTech = false, IsHeal = false, Power = 0, Speed = 9, Accuracy = 1.0 },

                    // Charl status moves
                    new AbilityEntity { Id = 100, Name = "System Override", Description = "Hack enemy systems reducing tech defense", IsTech = true, IsHeal = false, Power = 5, Speed = 7, Accuracy = 0.8 },
                    new AbilityEntity { Id = 101, Name = "Robo-Dog Shield", Description = "Deploy robo-dog as a shield boosting defense", IsTech = true, IsHeal = false, Power = 0, Speed = 6, Accuracy = 1.0 },

                    // Barnes status moves
                    new AbilityEntity { Id = 102, Name = "Sea Legs", Description = "Rock the boat reducing enemy accuracy", IsTech = false, IsHeal = false, Power = 3, Speed = 7, Accuracy = 0.85 },
                    new AbilityEntity { Id = 103, Name = "Scurvy", Description = "Inflict disease reducing defense over time", IsTech = false, IsHeal = false, Power = 4, Speed = 5, Accuracy = 0.75 },

                    // Belassie status moves
                    new AbilityEntity { Id = 104, Name = "Speed Intimidation", Description = "Intimidate with speed reducing enemy speed", IsTech = false, IsHeal = false, Power = 4, Speed = 9, Accuracy = 0.9 },
                    new AbilityEntity { Id = 105, Name = "Sprint Priority", Description = "Sprint priority change causes bleed but costs attack", IsTech = true, IsHeal = false, Power = 9, Speed = 7, Accuracy = 0.75 },

                    // Sailesh status moves
                    new AbilityEntity { Id = 106, Name = "Tech Stack Overflow", Description = "Overload with tech stacks causing confusion", IsTech = true, IsHeal = false, Power = 5, Speed = 6, Accuracy = 0.8 },
                    new AbilityEntity { Id = 107, Name = "Get Swifty", Description = "Channel Taylor Swift energy for speed boost", IsTech = false, IsHeal = false, Power = 0, Speed = 10, Accuracy = 1.0 },

                    // Andy status moves
                    new AbilityEntity { Id = 108, Name = "Blood Frenzy", Description = "Enter a frenzy boosting attack at cost of defense", IsTech = false, IsHeal = false, Power = 0, Speed = 7, Accuracy = 1.0 },

                    // Aaron status moves
                    new AbilityEntity { Id = 109, Name = "Bass Amplification", Description = "Amplify the bass boosting attack power", IsTech = false, IsHeal = false, Power = 0, Speed = 8, Accuracy = 1.0 },

                    // Haarunnya status moves
                    new AbilityEntity { Id = 110, Name = "Power Outage", Description = "Cut the power reducing enemy tech attack", IsTech = true, IsHeal = false, Power = 4, Speed = 7, Accuracy = 0.85 },

                    // Brooke status moves
                    new AbilityEntity { Id = 111, Name = "Diamond Guard", Description = "Create a diamond shield massively boosting defense", IsTech = false, IsHeal = false, Power = 0, Speed = 5, Accuracy = 1.0 },
                    new AbilityEntity { Id = 112, Name = "Taunt", Description = "Taunt the enemy reducing attack and accuracy", IsTech = false, IsHeal = false, Power = 2, Speed = 8, Accuracy = 0.9 },
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
                    new CharacterEntity { Id = 1, Name = "Ashley", ImageUrl = "public/Ashley.png", MaxHealth = 52, CurrentHealth = 52, Attack = 8, Defense = 8, TechAttack = 7, TechDefense = 7 },
                    new CharacterEntity { Id = 2, Name = "Banbury", ImageUrl = "public/Banbury.png", MaxHealth = 40, CurrentHealth = 40, Attack = 6, Defense = 6, TechAttack = 11, TechDefense = 7 },
                    new CharacterEntity { Id = 3, Name = "Beattie", ImageUrl = "public/Beattie.png", MaxHealth = 55, CurrentHealth = 55, Attack = 7, Defense = 8, TechAttack = 5, TechDefense = 6 }, // BUFFED: Atk 4->7, TechAtk 3->5, TechDef 5->6
                    new CharacterEntity { Id = 4, Name = "Faisal", ImageUrl = "public/Faisal.png", MaxHealth = 56, CurrentHealth = 56, Attack = 10, Defense = 10, TechAttack = 9, TechDefense = 10 },
                    new CharacterEntity { Id = 5, Name = "JP", ImageUrl = "public/JP.png", MaxHealth = 54, CurrentHealth = 54, Attack = 13, Defense = 11, TechAttack = 9, TechDefense = 10 }, // BUFFED: HP 50->54, Def 10->11, TechAtk 8->9, TechDef 9->10 (S-TIER)
                    new CharacterEntity { Id = 6, Name = "Langdon", ImageUrl = "public/Langdon.png", MaxHealth = 48, CurrentHealth = 48, Attack = 7, Defense = 10, TechAttack = 3, TechDefense = 6 },
                    new CharacterEntity { Id = 7, Name = "Lilley", ImageUrl = "public/Lilley.png", MaxHealth = 48, CurrentHealth = 48, Attack = 10, Defense = 9, TechAttack = 7, TechDefense = 7 },
                    new CharacterEntity { Id = 8, Name = "Nunan", ImageUrl = "public/Nunan.png", MaxHealth = 44, CurrentHealth = 44, Attack = 5, Defense = 6, TechAttack = 12, TechDefense = 8 }, // BUFFED: HP 38->44, Atk 4->5, Def 4->6
                    new CharacterEntity { Id = 9, Name = "Sam", ImageUrl = "public/Sam.png", MaxHealth = 43, CurrentHealth = 43, Attack = 9, Defense = 8, TechAttack = 6, TechDefense = 5 },
                    new CharacterEntity { Id = 10, Name = "Simon", ImageUrl = "public/Simon.png", MaxHealth = 52, CurrentHealth = 52, Attack = 7, Defense = 10, TechAttack = 14, TechDefense = 10 },
                    new CharacterEntity { Id = 11, Name = "Paul", ImageUrl = "public/Paul2.png", MaxHealth = 40, CurrentHealth = 40, Attack = 7, Defense = 7, TechAttack = 8, TechDefense = 6 },
                    new CharacterEntity { Id = 12, Name = "Charl", ImageUrl = "public/Charl.png", MaxHealth = 40, CurrentHealth = 40, Attack = 6, Defense = 7, TechAttack = 5, TechDefense = 10 },
                    new CharacterEntity { Id = 13, Name = "Barnes", ImageUrl = "public/Barnes.png", MaxHealth = 46, CurrentHealth = 46, Attack = 9, Defense = 9, TechAttack = 8, TechDefense = 8 },
                    new CharacterEntity { Id = 14, Name = "Belassie", ImageUrl = "public/Belassie.png", MaxHealth = 42, CurrentHealth = 42, Attack = 12, Defense = 9, TechAttack = 6, TechDefense = 6 }, // BUFFED: HP 35->42, TechAtk 5->6, TechDef 5->6
                    new CharacterEntity { Id = 15, Name = "Sailesh", ImageUrl = "public/Sailesh.png", MaxHealth = 48, CurrentHealth = 48, Attack = 8, Defense = 5, TechAttack = 10, TechDefense = 8 },
                    new CharacterEntity { Id = 16, Name = "Andy", ImageUrl = "public/Andy.png", MaxHealth = 46, CurrentHealth = 46, Attack = 4, Defense = 10, TechAttack = 11, TechDefense = 7 },
                    new CharacterEntity { Id = 17, Name = "Aaron", ImageUrl = "public/Aaron.png", MaxHealth = 44, CurrentHealth = 44, Attack = 13, Defense = 7, TechAttack = 8, TechDefense = 7 },
                    new CharacterEntity { Id = 18, Name = "Haarunnya", ImageUrl = "public/Haarunnya.png", MaxHealth = 48, CurrentHealth = 48, Attack = 9, Defense = 9, TechAttack = 8, TechDefense = 8 },
                    new CharacterEntity { Id = 19, Name = "Brooke", ImageUrl = "public/Brooke.png", MaxHealth = 65, CurrentHealth = 65, Attack = 5, Defense = 14, TechAttack = 6, TechDefense = 12 }
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
                    new CharacterAbilityEntity { CharacterId = 18, AbilityId = 69 },
                    new CharacterAbilityEntity { CharacterId = 18, AbilityId = 70 },
                    new CharacterAbilityEntity { CharacterId = 18, AbilityId = 71 },
                    new CharacterAbilityEntity { CharacterId = 18, AbilityId = 72 },
                    new CharacterAbilityEntity { CharacterId = 19, AbilityId = 73 },
                    new CharacterAbilityEntity { CharacterId = 19, AbilityId = 74 },
                    new CharacterAbilityEntity { CharacterId = 19, AbilityId = 75 },
                    new CharacterAbilityEntity { CharacterId = 19, AbilityId = 76 },
                    new CharacterAbilityEntity { CharacterId = 17, AbilityId = 77 },
                    new CharacterAbilityEntity { CharacterId = 18, AbilityId = 77 },
                    new CharacterAbilityEntity { CharacterId = 16, AbilityId = 77 },
                    // New status move mappings
                    // Ashley (1)
                    new CharacterAbilityEntity { CharacterId = 1, AbilityId = 78 },
                    new CharacterAbilityEntity { CharacterId = 1, AbilityId = 79 },

                    // Banbury (2)
                    new CharacterAbilityEntity { CharacterId = 2, AbilityId = 80 },
                    new CharacterAbilityEntity { CharacterId = 2, AbilityId = 81 },

                    // Beattie (3)
                    new CharacterAbilityEntity { CharacterId = 3, AbilityId = 82 },
                    new CharacterAbilityEntity { CharacterId = 3, AbilityId = 83 },

                    // Faisal (4)
                    new CharacterAbilityEntity { CharacterId = 4, AbilityId = 84 },
                    new CharacterAbilityEntity { CharacterId = 4, AbilityId = 85 },

                    // JP (5)
                    new CharacterAbilityEntity { CharacterId = 5, AbilityId = 86 },
                    new CharacterAbilityEntity { CharacterId = 5, AbilityId = 87 },

                    // Langdon (6)
                    new CharacterAbilityEntity { CharacterId = 6, AbilityId = 88 },
                    new CharacterAbilityEntity { CharacterId = 6, AbilityId = 89 },

                    // Lilley (7)
                    new CharacterAbilityEntity { CharacterId = 7, AbilityId = 90 },
                    new CharacterAbilityEntity { CharacterId = 7, AbilityId = 91 },

                    // Nunan (8)
                    new CharacterAbilityEntity { CharacterId = 8, AbilityId = 92 },
                    new CharacterAbilityEntity { CharacterId = 8, AbilityId = 93 },

                    // Sam (9)
                    new CharacterAbilityEntity { CharacterId = 9, AbilityId = 94 },
                    new CharacterAbilityEntity { CharacterId = 9, AbilityId = 95 },

                    // Simon (10)
                    new CharacterAbilityEntity { CharacterId = 10, AbilityId = 96 },
                    new CharacterAbilityEntity { CharacterId = 10, AbilityId = 97 },

                    // Paul (11)
                    new CharacterAbilityEntity { CharacterId = 11, AbilityId = 98 },
                    new CharacterAbilityEntity { CharacterId = 11, AbilityId = 99 },

                    // Charl (12)
                    new CharacterAbilityEntity { CharacterId = 12, AbilityId = 100 },
                    new CharacterAbilityEntity { CharacterId = 12, AbilityId = 101 },

                    // Barnes (13)
                    new CharacterAbilityEntity { CharacterId = 13, AbilityId = 102 },
                    new CharacterAbilityEntity { CharacterId = 13, AbilityId = 103 },

                    // Belassie (14)
                    new CharacterAbilityEntity { CharacterId = 14, AbilityId = 104 },
                    new CharacterAbilityEntity { CharacterId = 14, AbilityId = 105 },

                    // Sailesh (15)
                    new CharacterAbilityEntity { CharacterId = 15, AbilityId = 106 },
                    new CharacterAbilityEntity { CharacterId = 15, AbilityId = 107 },

                    // Andy (16)
                    new CharacterAbilityEntity { CharacterId = 16, AbilityId = 108 },

                    // Aaron (17)
                    new CharacterAbilityEntity { CharacterId = 17, AbilityId = 109 },

                    // Haarunnya (18)
                    new CharacterAbilityEntity { CharacterId = 18, AbilityId = 110 },

                    // Brooke (19)
                    new CharacterAbilityEntity { CharacterId = 19, AbilityId = 111 },
                    new CharacterAbilityEntity { CharacterId = 19, AbilityId = 112 },
                };

                _db.CharacterAbilities.AddRange(mappings);
                await _db.SaveChangesAsync();
            }

            // Status Effects for Abilities
            if (!_db.AbilityStatusEffects.Any())
            {
                var statusEffects = new[]
                {
                    // NEW STATUS EFFECTS FOR EXISTING ABILITIES

                    // Ability 1 - GOAL!: Reduce enemy Speed (kick in the nuts)
                    new AbilityStatusEffectEntity { AbilityId = 1, StatusEffectType = 14, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.95, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 4 - Story Time: Charging attack (drone on)
                    new AbilityStatusEffectEntity { AbilityId = 4, StatusEffectType = 15, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = true },

                    // Ability 9 - Home DIY: Bleed (saw accident)
                    new AbilityStatusEffectEntity { AbilityId = 9, StatusEffectType = 2, Duration = 3, Power = 5, Modifier = 1.0, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 16 - Bad Horror Movie: Confusion (Accuracy + Evasion Down)
                    new AbilityStatusEffectEntity { AbilityId = 16, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.7, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 16, StatusEffectType = 14, Duration = 2, Power = 0, Modifier = 0.2, ApplyChance = 0.7, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 22 - Scratch one Grub: Charging attack + Bleed
                    new AbilityStatusEffectEntity { AbilityId = 22, StatusEffectType = 15, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = true },
                    new AbilityStatusEffectEntity { AbilityId = 22, StatusEffectType = 2, Duration = 4, Power = 8, Modifier = 1.0, ApplyChance = 0.6, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 20 - Force feed dog turd: HealBlock (disgusting!)
                    new AbilityStatusEffectEntity { AbilityId = 20, StatusEffectType = 16, Duration = 3, Power = 0, Modifier = 1.0, ApplyChance = 0.7, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 26 - Spilled Tea: Burn damage
                    new AbilityStatusEffectEntity { AbilityId = 26, StatusEffectType = 0, Duration = 3, Power = 7, Modifier = 1.0, ApplyChance = 0.75, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 33 - P1: Charging attack (building up to emergency)
                    new AbilityStatusEffectEntity { AbilityId = 33, StatusEffectType = 15, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = true },

                    // Ability 36 - Patrol Smash: Charging attack (van accelerating)
                    new AbilityStatusEffectEntity { AbilityId = 36, StatusEffectType = 15, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = true },

                    // Ability 42 - Life Debug: Charging attack
                    new AbilityStatusEffectEntity { AbilityId = 42, StatusEffectType = 15, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = true },

                    // Ability 46 - Brain Rewrite: Charging attack
                    new AbilityStatusEffectEntity { AbilityId = 46, StatusEffectType = 15, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = true },

                    // Ability 50 - Blackspot: Charging attack + Poison/Sickness
                    new AbilityStatusEffectEntity { AbilityId = 50, StatusEffectType = 15, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = true },
                    new AbilityStatusEffectEntity { AbilityId = 50, StatusEffectType = 1, Duration = 5, Power = 10, Modifier = 1.0, ApplyChance = 0.5, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 61 - Chainsaw Slice: Bleed damage
                    new AbilityStatusEffectEntity { AbilityId = 61, StatusEffectType = 2, Duration = 3, Power = 7, Modifier = 1.0, ApplyChance = 0.7, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 65 - Feuer frei!: Burn damage (flamethrower)
                    new AbilityStatusEffectEntity { AbilityId = 65, StatusEffectType = 0, Duration = 4, Power = 8, Modifier = 1.0, ApplyChance = 0.65, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 72 - Turn the lights off!: Reduce enemy Accuracy
                    new AbilityStatusEffectEntity { AbilityId = 72, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.3, ApplyChance = 0.9, ApplyToSelf = false, RequiresCharging = false },

                    // Ability 77 - Smoke Screen (Evasion Up)
                    new AbilityStatusEffectEntity { AbilityId = 77, StatusEffectType = 13, Duration = 2, Power = 0, Modifier = 0.2, ApplyChance = 0.9, ApplyToSelf = true, RequiresCharging = false },

                    // Ashley abilities (78-79)
                    // 78 - Yellow Card: Reduce enemy Speed
                    new AbilityStatusEffectEntity { AbilityId = 78, StatusEffectType = 14, Duration = 3, Power = 0, Modifier = 0.3, ApplyChance = 0.95, ApplyToSelf = false, RequiresCharging = false },

                    // 79 - Offsides Confusion: Lower enemy Accuracy
                    new AbilityStatusEffectEntity { AbilityId = 79, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },

                    // Banbury abilities (80-81)
                    // 80 - Azure Outage: Reduce enemy Tech Attack
                    new AbilityStatusEffectEntity { AbilityId = 80, StatusEffectType = 8, Duration = 3, Power = 0, Modifier = 0.35, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },

                    // 81 - Windows Update: Stun
                    new AbilityStatusEffectEntity { AbilityId = 81, StatusEffectType = 17, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 0.9, ApplyToSelf = false, RequiresCharging = false },

                    // Beattie abilities (82-83)
                    // 82 - Protein Shake: Buff own Attack and Defense
                    new AbilityStatusEffectEntity { AbilityId = 82, StatusEffectType = 3, Duration = 3, Power = 0, Modifier = 0.3, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 82, StatusEffectType = 5, Duration = 3, Power = 0, Modifier = 0.25, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // 83 - Sawdust Cloud: Reduce enemy Accuracy
                    new AbilityStatusEffectEntity { AbilityId = 83, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.3, ApplyChance = 0.9, ApplyToSelf = false, RequiresCharging = false },

                    // Faisal abilities (84-85)
                    // 84 - Food Poisoning: Poison damage over time + HealBlock
                    new AbilityStatusEffectEntity { AbilityId = 84, StatusEffectType = 1, Duration = 4, Power = 5, Modifier = 1.0, ApplyChance = 0.75, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 84, StatusEffectType = 16, Duration = 3, Power = 0, Modifier = 1.0, ApplyChance = 0.75, ApplyToSelf = false, RequiresCharging = false },

                    // 85 - Scalding Tea: Burn damage over time
                    new AbilityStatusEffectEntity { AbilityId = 85, StatusEffectType = 0, Duration = 3, Power = 6, Modifier = 1.0, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },

                    // JP abilities (86-87)
                    // 86 - Call Out: Reduce enemy attack
                    new AbilityStatusEffectEntity { AbilityId = 86, StatusEffectType = 4, Duration = 3, Power = 0, Modifier = 0.3, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },

                    // 87 - Paint Fumes: Confuse enemy (Evasion Down + Accuracy Down)
                    new AbilityStatusEffectEntity { AbilityId = 87, StatusEffectType = 14, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 87, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.2, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },

                    // Langdon abilities (88-89)
                    // 88 - Tuesday Blues: Reduce enemy Attack and Speed
                    new AbilityStatusEffectEntity { AbilityId = 88, StatusEffectType = 4, Duration = 3, Power = 0, Modifier = 0.25, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 88, StatusEffectType = 14, Duration = 3, Power = 0, Modifier = 0.25, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },

                    // 89 - Caffeine Rush: Buff own Speed
                    new AbilityStatusEffectEntity { AbilityId = 89, StatusEffectType = 13, Duration = 3, Power = 0, Modifier = 0.5, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // Lilley abilities (90-91)
                    // 90 - Bearded Confidence: Buff own Defense
                    new AbilityStatusEffectEntity { AbilityId = 90, StatusEffectType = 5, Duration = 3, Power = 0, Modifier = 0.4, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // 91 - New Priority: Bleed enemy, reduce own attack
                    new AbilityStatusEffectEntity { AbilityId = 91, StatusEffectType = 2, Duration = 3, Power = 6, Modifier = 1.0, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 91, StatusEffectType = 4, Duration = 2, Power = 0, Modifier = 0.2, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // Nunan abilities (92-93)
                    // 92 - Data Overload: Confuse enemy (Accuracy/Evasion Down)
                    new AbilityStatusEffectEntity { AbilityId = 92, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 92, StatusEffectType = 14, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },

                    // 93 - Moustache Offensive: Reduce enemy Attack
                    new AbilityStatusEffectEntity { AbilityId = 93, StatusEffectType = 4, Duration = 3, Power = 0, Modifier = 0.3, ApplyChance = 0.9, ApplyToSelf = false, RequiresCharging = false },

                    // Sam abilities (94-95)
                    // 94 - On-Call Stress: Reduce all enemy stats
                    new AbilityStatusEffectEntity { AbilityId = 94, StatusEffectType = 4, Duration = 2, Power = 0, Modifier = 0.15, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 94, StatusEffectType = 6, Duration = 2, Power = 0, Modifier = 0.15, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 94, StatusEffectType = 8, Duration = 2, Power = 0, Modifier = 0.15, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 94, StatusEffectType = 10, Duration = 2, Power = 0, Modifier = 0.15, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },

                    // 95 - Bad Press: Charging attack
                    new AbilityStatusEffectEntity { AbilityId = 95, StatusEffectType = 15, Duration = 1, Power = 0, Modifier = 1.0, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = true },

                    // Simon abilities (96-97)
                    // 96 - Laid Back Vibes: Buff own Defense
                    new AbilityStatusEffectEntity { AbilityId = 96, StatusEffectType = 5, Duration = 3, Power = 0, Modifier = 0.35, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // 97 - Weekend Mood: Reduce enemy Speed
                    new AbilityStatusEffectEntity { AbilityId = 97, StatusEffectType = 14, Duration = 3, Power = 0, Modifier = 0.3, ApplyChance = 0.9, ApplyToSelf = false, RequiresCharging = false },

                    // Paul abilities (98-99)
                    // 98 - Code Review: Reduce enemy Accuracy
                    new AbilityStatusEffectEntity { AbilityId = 98, StatusEffectType = 12, Duration = 3, Power = 0, Modifier = 0.3, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },

                    // 99 - Caffeine Crash: Speed boost then dramatic crash
                    new AbilityStatusEffectEntity { AbilityId = 99, StatusEffectType = 13, Duration = 2, Power = 0, Modifier = 0.4, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 99, StatusEffectType = 14, Duration = 3, Power = 0, Modifier = 0.5, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // Charl abilities (100-101)
                    // 100 - System Override: Reduce enemy Tech Defense
                    new AbilityStatusEffectEntity { AbilityId = 100, StatusEffectType = 10, Duration = 3, Power = 0, Modifier = 0.35, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },

                    // 101 - Robo-Dog Shield: Buff own Defense
                    new AbilityStatusEffectEntity { AbilityId = 101, StatusEffectType = 5, Duration = 3, Power = 0, Modifier = 0.35, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // Barnes abilities (102-103)
                    // 102 - Sea Legs: Reduce enemy Accuracy
                    new AbilityStatusEffectEntity { AbilityId = 102, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.3, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },

                    // 103 - Scurvy: Reduce enemy Defense over time + HealBlock (disease)
                    new AbilityStatusEffectEntity { AbilityId = 103, StatusEffectType = 6, Duration = 4, Power = 0, Modifier = 0.25, ApplyChance = 0.75, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 103, StatusEffectType = 16, Duration = 4, Power = 0, Modifier = 1.0, ApplyChance = 0.75, ApplyToSelf = false, RequiresCharging = false },

                    // Belassie abilities (104-105)
                    // 104 - Speed Intimidation: Reduce enemy Speed
                    new AbilityStatusEffectEntity { AbilityId = 104, StatusEffectType = 14, Duration = 2, Power = 0, Modifier = 0.35, ApplyChance = 0.9, ApplyToSelf = false, RequiresCharging = false },

                    // 105 - Sprint Priority: Bleed enemy, reduce own attack
                    new AbilityStatusEffectEntity { AbilityId = 105, StatusEffectType = 2, Duration = 3, Power = 7, Modifier = 1.0, ApplyChance = 0.75, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 105, StatusEffectType = 4, Duration = 2, Power = 0, Modifier = 0.2, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // 56 - Transcribe Hell (Belassie): HealBlock (HR blocks wellness)
                    new AbilityStatusEffectEntity { AbilityId = 56, StatusEffectType = 16, Duration = 3, Power = 0, Modifier = 1.0, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },

                    // Sailesh abilities (106-107)
                    // 106 - Tech Stack Overflow: Confuse enemy
                    new AbilityStatusEffectEntity { AbilityId = 106, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.3, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 106, StatusEffectType = 14, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.8, ApplyToSelf = false, RequiresCharging = false },

                    // 107 - Get Swifty: Buff own Speed
                    new AbilityStatusEffectEntity { AbilityId = 107, StatusEffectType = 13, Duration = 3, Power = 0, Modifier = 0.5, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // Andy ability (108)
                    // 108 - Blood Frenzy: Buff own Attack, reduce own Defense
                    new AbilityStatusEffectEntity { AbilityId = 108, StatusEffectType = 3, Duration = 3, Power = 0, Modifier = 0.5, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 108, StatusEffectType = 6, Duration = 3, Power = 0, Modifier = 0.3, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // Aaron ability (109)
                    // 109 - Bass Amplification: Buff own Attack
                    new AbilityStatusEffectEntity { AbilityId = 109, StatusEffectType = 3, Duration = 3, Power = 0, Modifier = 0.4, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // Haarunnya ability (110)
                    // 110 - Power Outage: Reduce enemy Tech Attack
                    new AbilityStatusEffectEntity { AbilityId = 110, StatusEffectType = 8, Duration = 3, Power = 0, Modifier = 0.35, ApplyChance = 0.85, ApplyToSelf = false, RequiresCharging = false },

                    // Brooke abilities (111-112)
                    // 111 - Diamond Guard: Buff own Defense (high) and Tech Defense
                    new AbilityStatusEffectEntity { AbilityId = 111, StatusEffectType = 5, Duration = 3, Power = 0, Modifier = 0.6, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 111, StatusEffectType = 9, Duration = 3, Power = 0, Modifier = 0.4, ApplyChance = 1.0, ApplyToSelf = true, RequiresCharging = false },

                    // 112 - Taunt: Reduce enemy Attack and Accuracy
                    new AbilityStatusEffectEntity { AbilityId = 112, StatusEffectType = 4, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.9, ApplyToSelf = false, RequiresCharging = false },
                    new AbilityStatusEffectEntity { AbilityId = 112, StatusEffectType = 12, Duration = 2, Power = 0, Modifier = 0.25, ApplyChance = 0.9, ApplyToSelf = false, RequiresCharging = false },
                };

                _db.AbilityStatusEffects.AddRange(statusEffects);
                await _db.SaveChangesAsync();
            }
        }
    }
}
