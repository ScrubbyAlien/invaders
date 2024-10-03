using SFML.System;

namespace invaders;

public class SceneLoader
{
    private Dictionary<char, Func<Entity>> _constructors = new()
    {
        { 'g', () => new Grunt() },
    };

    private Dictionary<char, int[]> _enemyDimensions = new()
    {
        { ' ', new int[3] {8, 8, 1}},
        { 'g', new int[3] {Grunt.SpriteWidth, Grunt.SpriteHeight, (int)Grunt.Scale}},
    };
    
    private Dictionary<string, List<string>> _loadedLevels = new();
    
    public void Load(Scene scene, string level)
    {
        List<string> levelToLoad;
        if (_loadedLevels.TryGetValue(level, out List<string>? loadedLevel))
        {
            levelToLoad = loadedLevel;
        }
        else
        {
            levelToLoad = ParseLevel(level);
            _loadedLevels.Add(level, levelToLoad);
        }

        
        for (var j = 0; j < levelToLoad.Count; j++)
        {
            int leftMargin = (Program.ScreenWidth - CalculateEnemyLineWidth(levelToLoad[j])) / 2;
            for (int i = 0; i < levelToLoad[j].Length; i++)
            {
                if (_constructors.ContainsKey(levelToLoad[j][i]))
                {
                    Entity entity = _constructors[levelToLoad[j][i]]();
                    entity.Position = new Vector2f(
                        leftMargin + CalculateEnemyLineWidth(levelToLoad[j], i), 
                        Scene.MarginTop + j * entity.Bounds.Height);
                    scene.Spawn(entity);
                }
            }
        }
    }

    private List<string> ParseLevel(string level)
    {
        List<string> cleaned = AssetManager.ReadLevel(level)
            .Select(s => s.IndexOf("#") >= 0 ? s.Remove(s.IndexOf("#")) : s)
            .Where(s => s.Length > 0)
            .ToList();

        List<string> parsed = new();
        foreach (string line in cleaned)
        {
            string parsedLine = "";
            foreach (string group in line.Split(";"))
            {
                string[] symbols = group.Split(",");
                string enemy = symbols[0];
                int count = int.Parse(symbols[1]);
                string[] parsedGroupArray = new string[count];
                for (int i = 0; i < count; i++)
                {
                    parsedGroupArray[i] = enemy == "n" ? " " : enemy;
                }

                string parsedGroup;
                string padding = "";
                if (enemy != "n") for (int i = 0; i < int.Parse(symbols[2]); i++) padding += " ";
                parsedGroup = String.Join(padding, parsedGroupArray);
                parsedLine += parsedGroup;
            }
            parsed.Add(parsedLine);
        }
        return parsed;
    }

    private int CalculateEnemyLineWidth(string line)
    {
        int width = 0;
        for (int i = 0; i < line.Length; i++)
        {
            width += _enemyDimensions[line[i]][0] * _enemyDimensions[line[i]][2];
        }
        return width;
    }
    
    private int CalculateEnemyLineWidth(string line, int untilIndex)
    {
        int width = 0;
        for (int i = 0; i < untilIndex; i++)
        {
            width += _enemyDimensions[line[i]][0] * _enemyDimensions[line[i]][2];
        }
        return width;
    }
    
}