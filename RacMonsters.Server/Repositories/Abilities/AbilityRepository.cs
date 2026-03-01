using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Abilities
{
    public class AbilityRepository : IAbilityRepository
    {
        public async Task<Ability[]> GetAbilities(int[] ids)
        {
            // Simple in-memory abilities store
            var abilities = new[]
            {
                // Ashley
                new Ability { Id = 1, Name = "GOAL!", Description = "Kick your opponent in the nuts and celebrate", IsTech = false, Power = 5, Speed = 8, Accuracy = 0.95 },
                new Ability { Id = 2, Name = "Hulk Smash", Description = "A powerful smash attack.", IsTech = true, Power = 9, Speed = 4, Accuracy = 0.9 },
                new Ability { Id = 3, Name = "Cheeky Cuppa", Description = "Restore some health with a cuppa.", IsTech = true, IsHeal = true, Power = 9, Speed = 5, Accuracy = 1.0 },
                new Ability { Id = 4, Name = "Story Time", Description = "Force your opponent to read a badly worded user story", IsTech = true, IsHeal = false, Power = 12, Speed = 1, Accuracy = 0.5 },

                // Banbury
                new Ability { Id = 5, Name = "MicroTalk", Description = "Talks the praises of microsoft.", IsTech = true, Power = 6, Speed = 8, Accuracy = 0.95 },
                new Ability { Id = 6, Name = "PR Review", Description = "Nick reviews your PR and call it Bollocks", IsTech = true, Power = 9, Speed = 4, Accuracy = 0.9 },
                new Ability { Id = 7, Name = "Hot Choccy", Description = "Restore some health with a Hot Choccy.", IsTech = true, IsHeal = true, Power = 9, Speed = 5, Accuracy = 1.0 },
                new Ability { Id = 8, Name = "Jaguar", Description = "Becomes a jaguar and speeds at you at 120MPH", IsTech = false, IsHeal = false, Power = 10, Speed = 1, Accuracy = 0.5 },

                // Beattie
                new Ability { Id = 9, Name = "Home DIY", Description = "Forces you to assist with DIY causing an accident with a saw", IsTech = false, Power = 5, Speed = 8, Accuracy = 0.95 },
                new Ability { Id = 10, Name = "Hyrox", Description = "Join a hyrox session leaving you delapadated", IsTech = false, Power = 7, Speed = 4, Accuracy = 0.9 },
                new Ability { Id = 11, Name = "Mushroom Munch", Description = "Restore some health with a shroom.", IsTech = false, IsHeal = true, Power = 9, Speed = 5, Accuracy = 1.0 },
                new Ability { Id = 12, Name = "New FE Package", Description = "Preaches about a new FE Package derailing your workflow", IsTech = true, IsHeal = false, Power = 7, Speed = 1, Accuracy = 0.8 },

                // Faisal
                new Ability { Id = 13, Name = "Homebrew Tea", Description = "Drink some hombrew tea", IsTech = false, IsHeal = true, Power = 6, Speed = 8, Accuracy = 0.95 },
                new Ability { Id = 14, Name = "Kebabby", Description = "Throws a kebab stick at you attempting to poke your eye out", IsTech = false, Power = 7, Speed = 8, Accuracy = 0.9 },
                new Ability { Id = 15, Name = "NPM Critical Dependency", Description = "You discover a vulnerability in your NPM dependencies", IsTech = true, IsHeal = false, Power = 9, Speed = 5, Accuracy = 1.0 },
                new Ability { Id = 16, Name = "Bad Horror Movie", Description = "Preaches about a new FE Package derailing your workflow", IsTech = false, IsHeal = false, Power = 7, Speed = 1, Accuracy = 0.8 },
                
                // JP
                new Ability { Id = 17, Name = "User Research", Description = "Conducts user research to disprove your theory", IsTech = true, IsHeal = false, Power = 6, Speed = 8, Accuracy = 0.95 },
                new Ability { Id = 18, Name = "Throw Miniature", Description = "Throws a freshly painted warhammer 40k space marine at your eyeball", IsTech = false, Power = 7, Speed = 8, Accuracy = 0.9 },
                new Ability { Id = 19, Name = "Go Green", Description = "Gets high on the bud of life", IsTech = true, IsHeal = true, Power = 15, Speed = 5, Accuracy = 0.6 },
                new Ability { Id = 20, Name = "Force feed dog turd", Description = "Force feed dog turd from this shoe into the opponents mouth", IsTech = false, IsHeal = false, Power = 12, Speed = 1, Accuracy = 0.6 },

                // Langdon
                new Ability { Id = 21, Name = "Notes for Monday", Description = "Leaves a large stack of notes for the opponent to sort out on monday", IsTech = true, IsHeal = false, Power = 8, Speed = 8, Accuracy = 0.75 },
                new Ability { Id = 22, Name = "Scratch one Grub", Description = "Pulls out a chainsaw swings it at his opponent", IsTech = false, Power = 20, Speed = 4, Accuracy = 0.1 },
                new Ability { Id = 23, Name = "Inner Turmoil", Description = "Looks stressed and overwhelmed dealing psychic damage", IsTech = false, IsHeal = false, Power = 8, Speed = 5, Accuracy = 0.75 },
                new Ability { Id = 24, Name = "Coffee", Description = "Drinks a cup of coffee to regain focus and energy", IsTech = false, IsHeal = true, Power = 13, Speed = 4, Accuracy = 0.75},

                // Lilley
                new Ability { Id = 25, Name = "Beliggerent Banjo", Description = "Plays a newly discovered band's beliggerent banjo to inflict ear pain on the opponent", IsTech = false, IsHeal = false, Power = 8, Speed = 8, Accuracy = 0.75 },
                new Ability { Id = 26, Name = "Story Time", Description = "Force your opponent to read a badly worded user story", IsTech = true, IsHeal = false, Power = 14, Speed = 1, Accuracy = 0.5 },
                new Ability { Id = 27, Name = "Take hat off", Description = "Takes his hat off revealing embracing his baldness", IsTech = false, IsHeal = true, Power = 9, Speed = 5, Accuracy = 0.9 },
                new Ability { Id = 28, Name = "3 Amigos", Description = "Call upon 2 of your team to have a 3 amigos and decide the best way to defeat the opponent", IsTech = true, IsHeal = false, Power = 13, Speed = 4, Accuracy = 0.75},

                // Nunan
                new Ability { Id = 29, Name = "Datalake", Description = "Drown the user in data so they cant breathe", IsTech = true, IsHeal = false, Power = 10, Speed = 3, Accuracy = 0.6 },
                new Ability { Id = 30, Name = "Ah huh huh", Description = "In full Johnny Bravo fashion he steals your girl", IsTech = false, Power = 10, Speed = 5, Accuracy = 0.8 },
                new Ability { Id = 31, Name = "Snowflake", Description = "Throws a snowflake data query at you at lighting speed", IsTech = true, IsHeal = false, Power = 7, Speed = 9, Accuracy = 0.75 },
                new Ability { Id = 32, Name = "Coffee", Description = "Drinks a cup of coffee to regain focus and energy", IsTech = false, IsHeal = true, Power = 10, Speed = 4, Accuracy = 0.75},

                // Sam
                new Ability { Id = 33, Name = "P1", Description = "Alert of a P1 bringing everyone to work on their day off", IsTech = true, IsHeal = false, Power = 14, Speed = 4, Accuracy = 0.5 },
                new Ability { Id = 34, Name = "Last Minute Request", Description = "Request a last minute change causing you to work overtime", IsTech = true, Power = 8, Speed = 5, Accuracy = 0.75 },
                new Ability { Id = 35, Name = "Eat Smoked Meat", Description = "Looks stressed and overwhelmed dealing psychic damage", IsTech = false, IsHeal = true, Power = 8, Speed = 7, Accuracy = 0.75 },
                new Ability { Id = 36, Name = "Patrol Smash", Description = "Smashes through your car with a patrol van", IsTech = false, IsHeal = false, Power = 13, Speed = 2, Accuracy = 0.7},

                // Simon
                new Ability { Id = 37, Name = "PBomb", Description = "Created a prototype bomb in 2 hours", IsTech = true, IsHeal = false, Power = 13, Speed = 6, Accuracy = 0.75 },
                new Ability { Id = 38, Name = "Lie in", Description = "Have a nice lie in", IsTech = false, IsHeal = true, Power = 20, Speed = 4, Accuracy = 0.4 },
                new Ability { Id = 39, Name = "Your Mom", Description = "Tells a your mom joke", IsTech = false, IsHeal = false, Power = 8, Speed = 5, Accuracy = 0.75 },
                new Ability { Id = 40, Name = "Do the Washing", Description = "Forces opponent into doing his washing through online blackmail", IsTech = true, IsHeal = false, Power = 13, Speed = 4, Accuracy = 0.75},
            };

            var result = abilities.Where(a => ids.Contains(a.Id)).ToArray();
            return await Task.FromResult(result);
        }
    }
}
