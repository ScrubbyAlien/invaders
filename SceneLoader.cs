using SFML.System;

namespace invaders;

public static class SceneLoader
{
    // entity constructor dictionary system borrowed from lab project 4
    private static Dictionary<char, Func<int, AbstractEnemy>> _constructors = new()
    {
        { 'g', (rank) => new Grunt(rank) },
    };
    
    private static Dictionary<char, int[]> _enemyDimensions = new()
    {
        { ' ', new int[3] {8, 8, 1}},
        { 'g', new int[3] {Grunt.SpriteWidth, Grunt.SpriteHeight, (int)Grunt.Scale}},
    };
    
    private static Dictionary<int, List<string>> _loadedLevels = new();
    
    public static void Load(int level, ref List<List<AbstractEnemy>> enemies)
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

        enemies = new();
        for (int i = 0; i < levelToLoad.Count; i++)
        {
            int leftMargin = (Program.ScreenWidth - CalculateEnemyLineWidth(levelToLoad[i])) / 2;
            int heightPosition = CalculateEnemyHeightPosition(levelToLoad, i, 24);
            enemies.Add(new List<AbstractEnemy>());
            for (int j = 0; j < levelToLoad[i].Length; j++)
            {
                if (_constructors.ContainsKey(levelToLoad[i][j]))
                {
                    AbstractEnemy enemy = _constructors[levelToLoad[i][j]](i); 
                    enemies[i].Add(enemy);
                    enemy.Position = new Vector2f(
                        leftMargin + CalculateEnemyLineWidth(levelToLoad[i], j),
                        Scene.MarginTop + heightPosition);
                    Scene.Spawn(enemy);
                }
            }
        }
    }

    private static List<string> ParseLevel(int level)
    {
        List<string> cleaned = AssetManager.ReadLevel($"level{level}")
            .Select(s => s.IndexOf("#") >= 0 ? s.Remove(s.IndexOf("#")).Trim() : s)
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

    private static int CalculateEnemyLineWidth(string line)
    {
        int width = 0;
        for (int i = 0; i < line.Length; i++)
        {
            width += _enemyDimensions[line[i]][0] * _enemyDimensions[line[i]][2];
        }
        return width;
    }
    
    private static int CalculateEnemyLineWidth(string line, int untilIndex)
    {
        int width = 0;
        for (int i = 0; i < untilIndex; i++)
        {
            width += _enemyDimensions[line[i]][0] * _enemyDimensions[line[i]][2];
        }
        return width;
    }

    private static int CalculateEnemyHeightPosition(List<string> enemies, int untilIndex, int margin)
    {
        int height = 0;
        for (int i = 0; i < untilIndex; i++)
        {
            int tallest = 0;
            for (int j = 0; j < enemies[i].Length; j++)
            {
                // extract largest height value (adjusted by scale) from all the enemies in the row;
                tallest = enemies[i].Select(c => _enemyDimensions[c][1] * _enemyDimensions[c][2]).Max();
                
            }
            height += tallest + margin;
        }
        return height;
    }
}