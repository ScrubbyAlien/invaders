using invaders.enums;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public class ScoreManager : SceneObject
{
    private int _currentScore;
    private int _multiplier = 1;
    private float _multiplierTimer;
    private float _multiplierLifeSpan = 2f;
    private float _passiveScoreTimer;
    private float _passiveScoreInterval;
    private int _passiveScore;
    
    private TextGUI _scoreText = null!;
    private SpriteGUI _multiplierBar = null!;

    public int CurrentScore => _currentScore;

    public ScoreManager()
    { }

    public ScoreManager(int passiveScore, float interval) : this()
    {
        
        _passiveScore = passiveScore;
        _passiveScoreInterval = interval;
    }
    
    protected override void Initialize()
    {
        if (Scene.FindByTag(SceneObjectTag.ScoreText, out TextGUI? text)) _scoreText = text!;
        if (Scene.FindByTag(SceneObjectTag.MultiplierBar, out SpriteGUI? sprite)) _multiplierBar = sprite!;
        
        EventManager.EnemyDeath += OnEnemyDeath;
        EventManager.PlayerHit += ResetMultiplier;
    }

    public override void Update(float deltaTime)
    {
        
        _multiplierTimer += deltaTime;
        if (_multiplierTimer > _multiplierLifeSpan)
        {
            ResetMultiplier();
        }
        _scoreText.SetText($"{_currentScore}");
        _scoreText.Position = MiddleOfScreen(
            _scoreText.Bounds, 
            new Vector2f(0, 
                (-Program.ScreenHeight / 2f) + 32 + (48 - _scoreText.Bounds.Height) / 2)
            );
        
        if (_multiplier > 1)
        {
            float percent = _multiplierTimer / _multiplierLifeSpan;
            _multiplierBar.SetScale(new Vector2f(100 - 100 * percent, 5));
            _multiplierBar.Position = BottomRightOfScreen(
                _multiplierBar.Bounds, 
                new Vector2f(-24, -24 - _scoreText.Bounds.Height - 8)
            );
        }
        else
        {
            _multiplierBar.SetScale(new Vector2f());
        }

        Scene.FindByType(out WaveManager? waveManager);
        if (_passiveScore > 0 && !waveManager!.InTransition)
        {
            _passiveScoreTimer += deltaTime;
            if (_passiveScoreTimer >= _passiveScoreInterval)
            {
                _passiveScoreTimer = 0;
                GainScore(_passiveScore);

            }
        }
    }


    private void OnEnemyDeath(AbstractEnemy enemy)
    {
        int score = 0;
        if (enemy is Grunt) score = 50;
        score *= _multiplier;
        
        CreateFadingScoreText(
            0.5f, 
            $"{score}",
            f => enemy.Position + new Vector2f((enemy.Bounds.Width - f.Bounds.Width) / 2, -enemy.Bounds.Height - 5),
            new Vector2f(0, -1).Normalized() * 30
            );
        GainScore(score);
        IncrementMultiplier();
    }
    
    private void GainScore(int scoreGained)
    {
        _currentScore += scoreGained * _multiplier;
    }

    private void IncrementMultiplier()
    {
        _multiplier++;
        _multiplier = (int)MathF.Min(_multiplier, 5);
        _multiplierTimer = 0;
    }

    private void ResetMultiplier()
    {
        _multiplier = 1;
        _multiplierTimer = 0f;
    }

    // private void CreateFadingScoreText(float fadeTime, string text, uint size, Func<FadingTextGUI, Vector2f> positionFunc, Vector2f drift)
    // {
    //     FadingTextGUI fadingScore = new FadingTextGUI(fadeTime, text, size);
    //     fadingScore.Position = positionFunc(fadingScore);
    //     fadingScore.SetDrift(drift);
    //     Scene.QueueSpawn(fadingScore);
    // }
    private void CreateFadingScoreText(float fadeTime, string text, Func<FadingTextGUI, Vector2f> positionFunc, Vector2f drift)
    {
        FadingTextGUI fadingScore = new FadingTextGUI(fadeTime, text);
        fadingScore.Position = positionFunc(fadingScore);
        fadingScore.SetDrift(drift);
        Scene.QueueSpawn(fadingScore);
    }
    
}