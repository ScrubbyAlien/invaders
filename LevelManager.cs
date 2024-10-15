using invaders.entity;
using SFML.System;

namespace invaders;

public class LevelManager
{
    public bool InLevel;
    
    private float _waveTimer = -1f;
    private int _currentWave = -1;
    private int _currentLevel = -1;
    private List<Entity> _nextWaveToSpawn = new();
    // private int _highestLevel = 0;
    
    private static float _levelBuffer = 7f;
    private static float _levelBufferTimer;
    
    // entity constructor dictionary system borrowed from lab project 4
    public static Dictionary<char, Func<int, AbstractEnemy>> Constructors = new()
    {
        { 'g', (wave) => new Grunt(wave) },
    };
    
    private Dictionary<int, Level> _loadedLevels = new()
    {
        {-1, new() }
    };
    
    public Level CurrentLevel
    {
        get
        {
            if (!_loadedLevels.TryGetValue(_currentLevel, out Level _))
            {
                _loadedLevels.Add(_currentLevel, InstantiateLevel(_currentLevel));
            }
            return _loadedLevels[_currentLevel];
        }
    }
    
    public List<Entity> GetNewEntities()
    {
        List<Entity> r = new();
        _nextWaveToSpawn.ForEach(enemy =>
        {
            enemy.Position = new Vector2f(
                new Random().Next(Scene.MarginSide, Program.ScreenWidth - Scene.MarginSide - (int)enemy.Bounds.Width),
                new Random().Next(-Scene.MarginSide - Scene.SpawnInterval, -Scene.MarginSide));
        });
        r.AddRange(_nextWaveToSpawn);
        _nextWaveToSpawn.Clear();
        return r;
    }

    public void Update(float deltaTime)
    {
        if (InLevel)
        {
            if (_currentWave >= 0)
            {
                _waveTimer += deltaTime;
                if (_waveTimer >= CurrentLevel.GetWaveTimer(_currentWave))
                {
                    Console.WriteLine(_currentWave);
                    _nextWaveToSpawn.AddRange(CurrentLevel.GetWave(_currentWave));
                    _waveTimer = 0;
                    _currentWave++;
                    if (_currentWave == CurrentLevel.WaveCount)
                    { 
                        _currentWave = -1;
                        _waveTimer = 0;
                    }
                }
            }
            else
            {
                if (!Scene.FindByType(out AbstractEnemy _))
                {
                    InLevel = false;
                    StartLevelBuffer();
                }
            }
        }
    }

    public void StartLevel(int level)
    {
        InLevel = true;
        _currentLevel = level;
        _currentWave = 0;
        _waveTimer = 0;
    }

    private static void StartLevelBuffer()
    {
        _levelBufferTimer = 0;
    }

    public void ProgressLevelBuffer(float deltaTime)
    {
        if (!InLevel)
        {
            _levelBufferTimer += deltaTime;
            if (_levelBufferTimer is > 1 and <= 2)
            { // wait one second before jumping to warp 9
                Scene.AmbientScroll = float.Lerp(
                    Scene.AmbientScrollInLevel, 
                    Scene.AmbientScrollInBuffer, 
                    _levelBufferTimer - 1);
            }
            if (_levelBufferTimer > _levelBuffer - 2 && _levelBufferTimer < _levelBuffer - 1)
            { // slow down one second before next wave starts
                Scene.AmbientScroll = float.Lerp(
                    Scene.AmbientScrollInBuffer, 
                    Scene.AmbientScrollInLevel,
                    _levelBufferTimer - _levelBuffer + 2);
            }
            if (_levelBufferTimer >= _levelBuffer)
            {
                Scene.NextLevel();
            }
        }
        
    }
    
    private static Level InstantiateLevel(int level)
    {
        List<(List<char> c, int i)> parsed = ParseLevel(level);

        Level instantiate = new Level();
        for (var i = 0; i < parsed.Count; i++)
        {
            List<AbstractEnemy> entities = new();
            foreach (char constructor in parsed[i].c)
            {
                entities.Add(Constructors[constructor](i));
            }
            instantiate.AddWave(i, entities, parsed[i].i);
        }

        return instantiate;
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

    public class Level()
    {
        private Dictionary<int, List<AbstractEnemy>> _waves = new() { { -1, new() } };
        private Dictionary<int, float> _waveTimers = new() { { -1, 0f } };
        public int WaveCount => _waves.Count - 1;
        public void AddWave(int waveNumber, List<AbstractEnemy> wave, float timer)
        {
            _waves[waveNumber] = wave;
            _waveTimers[waveNumber] = timer;
        }

        public List<AbstractEnemy> GetWave(int waveNumber) { return _waves[waveNumber]; }

        public float GetWaveTimer(int waveNumber) { return _waveTimers[waveNumber]; }
    }
}
