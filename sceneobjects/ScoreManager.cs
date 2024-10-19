using invaders.enums;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public sealed class ScoreManager : SceneObject
{
    private int _currentScore;
    private int _multiplier = 1;
    private float _multiplierTimer;
    private float _multiplierLifeSpan = 2f;
    private float _passiveScoreTimer;
    private float _passiveScoreInterval;
    private int _passiveScore;

    private TextGUI _scoreText = null!;
    private TextGUI _multiplierText = null!;
    private SpriteGUI _multiplierBar = null!;
    private WaveManager _waveManager = null!;

    public int CurrentScore => _currentScore;

    
    public ScoreManager(int passiveScore = 0, float interval = 1)
    {
        _passiveScore = passiveScore;
        _passiveScoreInterval = interval;
    }
    
    protected override void Initialize()
    {
        _scoreText = Scene.FindByTag<TextGUI>(SceneObjectTag.ScoreText);
        _multiplierText = Scene.FindByTag<TextGUI>(SceneObjectTag.MultiplierText);
        _multiplierBar = Scene.FindByTag<SpriteGUI>(SceneObjectTag.MultiplierBar);
        _waveManager = Scene.FindByType<WaveManager>();
        
        EventManager.EnemyDeath += OnEnemyDeath;
        EventManager.PlayerHit += ResetMultiplier;
    }

    public override void Destroy()
    {
        LevelInfo<int>.Create(_currentScore);
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
                -Program.ScreenHeight / 2f + 36 + (48 - _scoreText.Bounds.Height) / 2)
        );
        
        _multiplierText.SetText($"x{_multiplier}");
        _multiplierText.Position = new Vector2f(
            Program.ScreenWidth - (TextureRects["guiBackgroundRight"].Width - 2) * RenderObject.Scale + 8,
            32f
        );
        
        if (_multiplier > 1)
        {
            _multiplierBar.Unhide();
            float percent = _multiplierTimer / _multiplierLifeSpan;
            _multiplierBar.SetScale(new Vector2f(80 - 80 * percent, RenderObject.Scale));
            _multiplierBar.Position = _multiplierText.Position + new Vector2f(_multiplierText.Bounds.Width + 8, 6);
        }
        else
        {
            _multiplierBar.Hide();
        }
        
        if (_passiveScore > 0 && !_waveManager.InTransition)
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
            17 + (uint) MathF.Floor((score - 50) / 50f),
            f => enemy.Position + new Vector2f((enemy.Bounds.Width - f.Bounds.Width) / 2, -enemy.Bounds.Height),
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
    
    private void CreateFadingScoreText(float fadeTime, string text, uint size, Func<FadingTextGUI, Vector2f> positionFunc, Vector2f drift)
    {
        FadingTextGUI fadingScore = new FadingTextGUI(fadeTime, text, size);
        fadingScore.Position = positionFunc(fadingScore);
        fadingScore.SetDrift(drift);
        Scene.QueueSpawn(fadingScore);
    }
    
}