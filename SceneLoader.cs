using invaders.entity;

namespace invaders;

public static class SceneLoader
{
    // entity constructor dictionary system borrowed from lab project 4
    public static Dictionary<char, Func<int, AbstractEnemy>> Constructors = new()
    {
        { 'g', (wave) => new Grunt(wave) },
    };
    
    
    private static Dictionary<int, List<(List<char>, int)>> _loadedLevels = new();
    
    /// <summary>
    /// Levels are represented by a list of tuples, where the first value is a list of chars representing enemies,
    /// and the second value is the wave timer.
    /// </summary>
    public static void Load(int level, ref List<(List<char>, int)> loaded)
    {
        if (_loadedLevels.TryGetValue(level, out List<(List<char>, int)>? alreadyLoaded))
        {
            loaded = alreadyLoaded;
        }
        else
        {
            loaded = ParseLevel(level);
            _loadedLevels.Add(level, loaded);
        }
    }

    private static List<(List<char>, int)> ParseLevel(int level)
    {
        List<string> cleaned = AssetManager.ReadLevel($"level{level}")
            .Select(s => s.IndexOf("#") >= 0 ? s.Remove(s.IndexOf("#")).Trim() : s)
            .Where(s => s.Length > 0)
            .ToList();

        List<(List<char> c, int t)> parsed = new();
        foreach (string line in cleaned)
        {
            (List<char> c, int t) parsedLine = (new List<char>(), 0);
            foreach (string group in line.Split(";"))
            {
                string[] tuple = group.Split(",");
                if (tuple[0] != "t")
                {
                    for (int i = 0; i < int.Parse(tuple[1]); i++)
                    {
                        parsedLine.c.Add(char.Parse(tuple[0]));
                    }
                }
                else
                {
                    parsedLine.t = int.Parse(tuple[1]);
                }
            }
            parsed.Add(parsedLine);
        }
        return parsed;
    }
}