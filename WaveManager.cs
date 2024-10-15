using invaders.sceneobjects;
using SFML.System;
using SFML.Window;
using static invaders.Utility;

namespace invaders;

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

    public WaveManager()
    {
        _waveTimer = 0;
        _currentWave = 0;
        _currentAssault = -1;
        _inTransition = true;
        _transitionBufferTimer = 0;
        
        DrawText("defeat the invaders!", new Vector2f(0, -100));
    }

    public override void Update(float deltaTime)
    {
        if (_inEndLevel)
        {
            if (AreAnyKeysPressed([Keyboard.Key.Space]))
            {
                // Scene.LoadLevel("mainmenu");
            }
            return;
        }
        if (!_inTransition)
        {
            _waveTimer += deltaTime;
            
            if (_currentWave == _assaults[_currentAssault].Count)
            { // end of assault
                if (!Scene.FindByType(out AbstractEnemy _))
                {
                    if (_currentAssault + 1 == _assaults.Count)
                    {
                        EndLevel();
                        return;
                    }
                    StartTransition();
                }
                
            }
            else
            {
                Wave wave = _assaults[_currentAssault][_currentWave];
                if (_waveTimer >= wave.timer)
                {
                    wave.Spawn();
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
                if (_currentAssault >= 0) _transitionText.SetText("proceeding to next threat");
                else _transitionText.SetText("first threat incoming"); 
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
    
    private void StartTransition()
    {
        Scene.FindAllByType<Bullet>().ForEach(o => Scene.QueueDestroy(o));
        _inTransition = true;
        _transitionBufferTimer = 0;
        _scrollSpedUp = false;
        _scrollSlowedDown = false;
        
        DrawText($"{numberToOrdinalWord[_currentAssault + 1]} threat cleared", new Vector2f(0, -100));
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

    private void DrawText(string text, Vector2f positionFromMiddle)
    {
        _transitionText = new TextGUI(text);
        _transitionText.Position = MiddleOfScreen(_transitionText.Bounds) + positionFromMiddle;
        Scene.QueueSpawn(_transitionText);
        // must call PlayAnimation after next update cycle so _transitionText's Initialize method is called first
        Scene.DeferredCall(_transitionText.GetAnimatable().Animator, "PlayAnimation", ["blink", true]);
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

