using invaders.sceneobjects;

namespace invaders;

public class WaveManager : SceneObject
{
    private bool _inTransition;
    
    private float _waveTimer;
    private int _currentWave;
    private int _currentAssault;
    public int CurrentAssault => _currentAssault;
    
    private float _transitionBuffer = 8f;
    private float _transitionBufferTimer;
    private bool _scrollSpedUp;
    private bool _scrollSlowedDown;
    
    // entity constructor dictionary system borrowed from lab project 4
    public static readonly Dictionary<char, Func<AbstractEnemy>> Constructors = new()
    {
        { 'g', () => new Grunt() },
    };
    
    private static readonly List<List<Wave>> _assaults = new()
    {
        new()
        {
            { new Wave(0f).AddEnemyGroup('g', 7) },
            { new Wave(10f).AddEnemyGroup('g', 10) },
            { new Wave(10f).AddEnemyGroup('g', 10) },
        },
        new()
        {
            {new Wave(0f).AddEnemyGroup('g', 10)},
            {new Wave(10f).AddEnemyGroup('g', 15)},
            {new Wave(10f).AddEnemyGroup('g', 20)},
        }
    };
    
    protected override void Initialize()
    {
        _waveTimer = 0;
        _currentWave = 0;
        _currentAssault = 0;
        _inTransition = true;
        _transitionBufferTimer = 0;
    }

    public override void Update(float deltaTime)
    {
        if (!_inTransition)
        {
            _waveTimer += deltaTime;
            
            if (_currentWave == _assaults[_currentAssault].Count)
            { // end of assault
                if (!Scene.FindByType(out AbstractEnemy _))
                {
                    StartTransition();
                    _currentAssault++;
                }
                if (_currentAssault == _assaults.Count)
                { 
                    // end of game  
                }
            }
            else
            {
                Wave wave = _assaults[_currentAssault][_currentWave];
                if (_waveTimer >= wave.timer)
                {
                    wave.Spawn();
                    // Console.WriteLine($"{_currentAssault}, {_currentWave}");
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
                EventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInTransition, 2f);
                _scrollSpedUp = true;
            }
            else if (_transitionBufferTimer > _transitionBuffer - 5 && !_scrollSlowedDown)
            {
                EventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 2f);
                _scrollSlowedDown = true;
            }
            if (_transitionBufferTimer >= _transitionBuffer)
            {
                _inTransition = false;
                _currentWave = 0;
            }
        }
    }

    private void StartTransition()
    {
        _inTransition = true;
        _transitionBufferTimer = 0;
        _scrollSpedUp = false;
        _scrollSlowedDown = false;
    }
    
    private struct Wave(float timer)
    {
        private Dictionary<char, int> _enemies = new();
        public float timer = timer;

        public Wave AddEnemyGroup(char enemyType, int number)
        {
            _enemies.Add(enemyType, number);
            return this;
        }

        public void Spawn()
        {
            foreach (KeyValuePair<char,int> group in _enemies)
            {
                for (int i = 0; i < group.Value; i++)
                {
                    AbstractEnemy enemy = Constructors[group.Key]();
                    Scene.QueueSpawn(enemy);
                }
            }
        }
    }
}

