using System.Globalization;
using invaders.sceneobjects.renderobjects;
using invaders.sceneobjects.renderobjects.gui;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public class EndlessManager : Invasion
{
    public override bool InTransition => inEndLevel || _untilStartTimer < _timeUntilStart;
    private float _timeUntilStart = 6f;
    private float _untilStartTimer;
    private bool _scrollSlowedDown;
    private int _enemyRange = 1;
    private float _spawnTimer;
    private float _spawnRate = 3;

    // the order needs to be defined so we use list of tuples instead of dictionary
    private List<(char, float)> enemyProbabilities = new()
    { // how long until next enemy of type should spawn
        ('g', 0.7f),
        ('r', 0.07f),
        ('s', 0.03f),
        ('j', 0.01f)
    };
    
    private float _timeFromStart;
    private TextGUI _timer = new TextGUI("");
    
    protected override void Initialize()
    {
        base.Initialize();
        GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInTransition, 1f);
        _timer.SetText(GetTimeString(0));
        _timer.Position = MiddleOfScreen(_timer.Bounds, new Vector2f(0, Program.ScreenHeight / 2f - _timer.Bounds.Height));
        Scene.QueueSpawn(_timer);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (inEndLevel)
        {
            Scene.QueueSpawn(new LevelInfo<bool>(true, "endless"));
            return;
        } 

        if (InTransition)
        {
            if (_untilStartTimer == 0)
            {
                DrawText("Defeat the invaders!", new Vector2f(0, -100));
            }
            _untilStartTimer += deltaTime;
            if (_untilStartTimer >= _timeUntilStart / 2f)
            {
                messageText.SetText("Don't give up!");
                messageText.Position = MiddleOfScreen(messageText.Bounds, new Vector2f(0, -100));
            }
            if (_untilStartTimer >= _timeUntilStart - 2 && !_scrollSlowedDown)
            {        
                GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
                _scrollSlowedDown = true;
            }
        }
        else
        {
            messageText.SetText("");
            messageText.Hide();
            _timeFromStart += deltaTime;
            _timer.SetText(GetTimeString(_timeFromStart));
            
            if (_timeFromStart >= _enemyRange * 30) _enemyRange++;
            if (_enemyRange > enemyProbabilities.Count())
            {
                // if all enemies have been added to the pool, we increase their probability of spawning instead
                enemyProbabilities.ForEach(pair => pair.Item2 *= 1.10f);
                _enemyRange = enemyProbabilities.Count();
            }
            
            _spawnTimer += deltaTime;
            if (_spawnTimer >= _spawnRate)
            {
                SpawnEnemy();
                _spawnTimer = 0;
            }
            // not linear but smooth, faster in the beginning
            if (_spawnRate > 0.7f) _spawnRate = -0.3f * MathF.Sqrt(_timeFromStart) + 3;
        }
    }
    
    private void SpawnEnemy()
    {
        float random = new Random().NextSingle();
        foreach ((char type, float prob) pair in enemyProbabilities.Take(_enemyRange))
        {
            if (random <= pair.prob)
            {
                AbstractEnemy enemy = Constructors[pair.type]();
                enemy.InitPosition = new Vector2f(enemy.InitPosition.X, 0);
                Scene.QueueSpawn(enemy);
            }
        }
    }

    // n=1 => 1, n=2 => 3, n=3 => 6 ...
    private float TriangularNumber(int n)
    {
        float acc = 0;
        for (int i = 0; i < n; i++) acc += i; 
        return acc;
    }

    /// <summary>
    /// Returns a string in the format MM:SS
    /// </summary>
    /// <param name="time">a time in seconds</param>
    /// <returns></returns>
    private string GetTimeString(float time)
    {
        float seconds = MathF.Floor(time % 60);
        float minutes = MathF.Floor(time / 60f);

        string secongsString = seconds.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0');
        string minutesString = minutes.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0');

        return minutesString + ":" + secongsString;
    }
    
}