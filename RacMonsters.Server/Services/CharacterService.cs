using RacMonsters.Server.Models;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace RacMonsters.Server.Services;

public interface ICharacterService
{
    List<Character> GetAll();
}

public class CharacterService : ICharacterService
{
    private readonly IHostEnvironment _env;
    private readonly List<Character> _characters;

    public CharacterService(IHostEnvironment env)
    {
        _env = env;
        _characters = LoadCharacters();
    }

    private List<Character> LoadCharacters()
    {
        var chars = new List<Character>();
        var possible = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "frontend", "public"));
        if (!Directory.Exists(possible))
        {
            possible = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "public"));
        }

        if (Directory.Exists(possible))
        {
            var files = Directory.GetFiles(possible, "*.png").OrderBy(f => f).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                var fileName = Path.GetFileName(files[i]);
                var name = Path.GetFileNameWithoutExtension(fileName);
                chars.Add(new Character
                {
                    Id = i + 1,
                    Name = name,
                    Health = 100,
                    MaxHealth = 100,
                    Abilities = new[] {
                        new Ability("Basic Attack", 8, false),
                        new Ability("Special", 20, true),
                        new Ability("Minor Guard", 0, false)
                    },
                    ImageUrl = $"/public/{fileName}",
                    Attack = 18,
                    Defense = 12,
                    TechAttack = 14,
                    TechDefense = 10
                });
            }
        }
        else
        {
            chars.Add(new Character
            {
                Id = 1,
                Name = "Simon",
                Health = 100,
                MaxHealth = 100,
                Abilities = new[] {
                    new Ability("Flame Strike", 18, false),
                    new Ability("Heat Wave", 26, true),
                    new Ability("Singe", 10, false)
                },
                ImageUrl = "/public/Simon.png",
                Attack = 24,
                Defense = 12,
                TechAttack = 18,
                TechDefense = 10
            });

            chars.Add(new Character
            {
                Id = 2,
                Name = "Sam",
                Health = 120,
                MaxHealth = 120,
                Abilities = new[] {
                    new Ability("Tidal Punch", 16, false),
                    new Ability("Aqua Burst", 22, true),
                    new Ability("Shell Guard", 0, false)
                },
                ImageUrl = "/public/Sam.png",
                Attack = 18,
                Defense = 20,
                TechAttack = 14,
                TechDefense = 18
            });
        }

        return chars;
    }

    public List<Character> GetAll() => _characters;
}
