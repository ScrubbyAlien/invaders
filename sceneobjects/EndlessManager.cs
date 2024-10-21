using System.Globalization;
using invaders.sceneobjects.gui;
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
    private float _spawnRate = 5f; // seconds per enemy

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
            _untilStartTimer += deltaTime;
            if (_untilStartTimer >= _timeUntilStart - 2 && !_scrollSlowedDown)
            {        
                GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
                _scrollSlowedDown = true;
            }
        }
        else
        {
            _timeFromStart += deltaTime;
            _timer.SetText(GetTimeString(_timeFromStart));
            
            IncrementEnemyRange(30);
            
            _spawnTimer += deltaTime;
            if (_spawnTimer >= _spawnRate)
            {
                SpawnEnemy();
                _spawnTimer = 0;
            }
            if (_spawnRate > 0.7f) _spawnRate -= 0.2f * deltaTime;
        }
    }

    private void IncrementEnemyRange(float interval)
    {
        if (_timeFromStart >= _enemyRange * interval) _enemyRange++;
    } 
    
    private void SpawnEnemy()
    {
        int range = (int) MathF.Min(_enemyRange, Constructors.Count());
        char[] enemyConstructors = Constructors.Keys.Take(range).ToArray();
        float[] probabilities = [enemyConstructors.Length];
        float denominator = TriangularNumber(probabilities.Length);
        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i] =  i + 1 / denominator;
        }
        float random =  new Random().NextSingle();
        
        for (int i = probabilities.Length - 1; i >= 0; i--)
        {
            if (random < probabilities[i])
            {
                AbstractEnemy enemy = Constructors[enemyConstructors[i]]();
                enemy.InitPosition = new Vector2f(
                    new Random().Next(Program.ScreenWidth),
                    Settings.TopGuiHeight - enemy.Bounds.Height
                );
                Scene.QueueSpawn(enemy);
                break;
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