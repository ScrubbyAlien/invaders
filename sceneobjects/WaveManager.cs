using invaders.sceneobjects;
using SFML.System;
using SFML.Window;
using static invaders.Utility;

namespace invaders.sceneobjects;

public class WaveManager : SceneObject
{
    private bool _inTransition;
    private bool _inEndLevel;
    
    private float _waveTimer;
    private int _currentWave;
    private int _currentAssault;
    public int CurrentAssault => _currentAssault;
    public bool InTransition => _inTransition || _inEndLevel;
    
    private float _transitionBuffer = 5f;
    private float _transitionBufferTimer;
    private bool _scrollSpedUp;
    private bool _scrollSlowedDown;

    private TextGUI _transitionText = new("");
    
    // entity constructor dictionary system borrowed from lab project 4
    public static readonly Dictionary<char, Func<AbstractEnemy>> Constructors = new()
    {
        { 'g', () => new Grunt() },
    };
    
    private readonly List<Assault> _assaults = new();

    public WaveManager()
    {
        _waveTimer = 0;
        _currentWave = 0;
        _currentAssault = -1;
        _inTransition = true;
        _transitionBufferTimer = 0;
        DrawText("defeat the invaders!", new Vector2f(0, -100));
    }

    protected override void Initialize()
    {
        EventManager.PlayerDeath += PlayerDied;
    }

    public override void Destroy()
    {
        EventManager.PlayerDeath -= PlayerDied;
    }

    public override void Update(float deltaTime)
    {
        if (_inEndLevel)
        {
            if (AreAnyKeysPressed([Keyboard.Key.Space]))
            {
                EventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
                Scene.LoadLevel("mainmenu");
            }
            return;
        }
        if (!_inTransition)
        {
            _waveTimer += deltaTime;
            
            if (_currentWave == _assaults[_currentAssault].Waves.Count())
            { // end of assault
                if (!Scene.FindByType(out AbstractEnemy _))
                {
                    if (_currentAssault == _assaults.Count - 1)
                    {
                        EndLevel();
                        return;
                    }
                    StartTransition();
                }
            }
            else
            {
                Wave wave = _assaults[_currentAssault].Waves[_currentWave];
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
                EventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInTransition, 1f);
                _scrollSpedUp = true;
            }
            else if (_transitionBufferTimer > _transitionBuffer - 2 && !_scrollSlowedDown)
            {
                EventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
                _scrollSlowedDown = true;
            }

            if (_transitionBufferTimer > _transitionBuffer / 2f)
            {
                // should avoid out of bounds errors becuase StartTransition is not called after last assault
                _transitionText.SetText(_assaults[_currentAssault + 1].BeforeAssault); 
                _transitionText.Position = MiddleOfScreen(_transitionText.Bounds) - new Vector2f(0, 100);
            }
            if (_transitionBufferTimer >= _transitionBuffer)
            {
                Scene.QueueDestroy(_transitionText);
                _inTransition = false;
                _currentWave = 0;
                _currentAssault++;
            }
        }
    }

    public void AddAssault(Assault assault) => _assaults.Add(assault);
    public void AddAssault(Assault[] assaults) => _assaults.AddRange(assaults);
    
    private void StartTransition()
    {
        Scene.FindAllByType<Bullet>().ForEach(o => Scene.QueueDestroy(o));
        _inTransition = true;
        _transitionBufferTimer = 0;
        _scrollSpedUp = false;
        _scrollSlowedDown = false;
        
        DrawText(_assaults[_currentAssault].AfterAssault, new Vector2f(0, -100));
    }
    
    private void EndLevel()
    {
        _inEndLevel = true;
        DrawText(
            "Invaders defeated!\n" +
            "Congratulations!\n" +
            " \n" +
            "press space to return to menu", 
            new Vector2f(0, -100)
        );
        EventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInTransition, 3f);
    }

    private void PlayerDied()
    {
        _inEndLevel = true;
        DrawText(
            "You have been defeated!\n" +
            " \n" +
            "press space to return to menu",
            new Vector2f(0, -100));
    }

    private void DrawText(string text, Vector2f positionFromMiddle)
    {
        _transitionText = new TextGUI(text);
        _transitionText.Position = MiddleOfScreen(_transitionText.Bounds) + positionFromMiddle;
        Scene.QueueSpawn(_transitionText);
        // must call PlayAnimation after next update cycle so _transitionText's Initialize method is called first
        Scene.DeferredCall(_transitionText.GetAnimatable().Animator, "PlayAnimation", ["blink", true]);
    }
    
    public void SpawnWave(Wave wave)
    {
        foreach (KeyValuePair<char,int> group in wave.Enemies)
        {
            for (int i = 0; i < group.Value; i++)
            {
                AbstractEnemy enemy = Constructors[group.Key]();
                Scene.QueueSpawn(enemy);
            }
        }
    }
}

public struct Wave(float timer)
{
    private Dictionary<char, int> _enemies = new();
    public float Timer = timer;
    public Dictionary<char, int> Enemies => _enemies;

    public Wave AddEnemyGroup(char enemyType, int number)
    {
        _enemies.Add(enemyType, number);
        return this;
    }
}

public struct Assault(string[] assaultStrings)
{
    private List<Wave> _waves = new();
    private string[] _assaultStrings = assaultStrings;
    public string BeforeAssault => _assaultStrings[0];
    public string AfterAssault => _assaultStrings[1];
    public List<Wave> Waves => _waves;
        
    public void AddWave(Wave wave) => _waves.Add(wave); 
    public void AddWave(Wave[] waves) => _waves.AddRange(waves); 
}

