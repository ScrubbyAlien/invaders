using invaders.sceneobjects.renderobjects;
using SFML.System;

namespace invaders.sceneobjects;

public sealed class WaveManager : Invasion
{
    private bool _inTransition;
    
    private float _waveTimer;
    private int _currentWave;
    public int CurrentAssault { get; private set; }

    public override bool InTransition => _inTransition || inEndLevel;
    
    private const float _transitionBuffer = 5f;
    private float _transitionBufferTimer;
    private bool _scrollSpedUp;
    private bool _scrollSlowedDown;
    
    private readonly List<Assault> _assaults = new();

    public WaveManager()
    {
        _waveTimer = 0;
        _currentWave = 0;
        CurrentAssault = -1;
        _inTransition = true;
        _transitionBufferTimer = 0;
    }

    protected override void Initialize()
    {
        base.Initialize();
        DrawText("defeat the invaders!", new Vector2f(0, -100));
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (inEndLevel)
        {
            Scene.QueueSpawn(new LevelInfo<bool>(false, "endless"));
            return;
        }
        
        if (!_inTransition)
        {
            _waveTimer += deltaTime;
            
            if (_currentWave == _assaults[CurrentAssault].Waves.Count)
            {
                // end of assault
                if (Scene.FindAllByType<AbstractEnemy>().Any()) return;
                
                if (CurrentAssault == _assaults.Count - 1)
                {
                    EndLevel();
                    return;
                }
                if (CurrentAssault == _assaults.Count - 2)
                {
                    MusicManager.ChangeMusic("finale");
                }
                        
                StartTransition();
            }
            else
            {
                Wave wave = _assaults[CurrentAssault].Waves[_currentWave];
                if (_waveTimer >= wave.Timer)
                {
                    SpawnWave(wave);
                    _currentWave++;
                    _waveTimer = 0;
                }
            }
        }
        else
        {
            _transitionBufferTimer += deltaTime;
            if (_transitionBufferTimer > 1 && !_scrollSpedUp)
            {
                GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInTransition, 1f);
                _scrollSpedUp = true;
            }
            else if (_transitionBufferTimer > _transitionBuffer - 2 && !_scrollSlowedDown)
            {
                GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
                _scrollSlowedDown = true;
            }

            if (_transitionBufferTimer > _transitionBuffer / 2f)
            {
                // should avoid out of bounds errors becuase StartTransition is not called after last assault
                DrawText(_assaults[CurrentAssault + 1].BeforeAssault, new Vector2f(0, -100));
            }
            if (_transitionBufferTimer >= _transitionBuffer)
            {
                HideText();
                _inTransition = false;
                _currentWave = 0;
                CurrentAssault++;
            }
        }
    }

    public void AddAssault(Assault[] assaults) => _assaults.AddRange(assaults);
    
    private void StartTransition()
    {
        Scene.FindAllByType<Bullet>().ForEach(b => Scene.QueueDestroy(b)); 
        _inTransition = true;
        _transitionBufferTimer = 0;
        _scrollSpedUp = false;
        _scrollSlowedDown = false;
        
        DrawText(_assaults[CurrentAssault].AfterAssault, new Vector2f(0, -100));
    }
    
    private void EndLevel()
    {
        inEndLevel = true;
        
        DrawText(
            "Invaders defeated!\n" +
            "Congratulations!\n" +
            " \n" +
            "press space to continue", 
            new Vector2f(0, -100)
        );
        GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInTransition, 3f);
    }
    
    private static void SpawnWave(Wave wave)
    {
        foreach (KeyValuePair<char,int> group in wave.Enemies)
        {
            for (int i = 0; i < group.Value; i++)
            {
                AbstractEnemy enemy = Invasion.Constructors[group.Key]();
                Scene.QueueSpawn(enemy);
            }
        }
    }
}

public struct Wave(float timer)
{
    public float Timer = timer;
    public Dictionary<char, int> Enemies { get; } = new();

    public Wave Group(char type, int number)
    {
        Enemies.Add(type, number);
        return this;
    }
}

public struct Assault(string[] assaultStrings)
{
    private readonly string[] _assaultStrings = assaultStrings;
    public string BeforeAssault => _assaultStrings[0];
    public string AfterAssault => _assaultStrings[1];
    public List<Wave> Waves { get; } = new();

    public Assault AddWave(Wave wave)
    {
        Waves.Add(wave);
        return this;
    }

    public void AddWave(Wave[] waves) => Waves.AddRange(waves); 
}

