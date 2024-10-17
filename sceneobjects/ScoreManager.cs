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

        
        // functionality is encapsulated in if statements because ScoreManager will persist between levels
        if (Scene.FindByTag(SceneObjectTag.ScoreText, out TextGUI scoreText))
        {
            scoreText.SetText($"{_currentScore}");
            scoreText.Position = MiddleOfScreen(
                scoreText.Bounds, 
                new Vector2f(0, 
                    -Program.ScreenHeight / 2f + 36 + (48 - scoreText.Bounds.Height) / 2)
                );
            
        }

        if (Scene.FindByTag(SceneObjectTag.MultiplierText, out TextGUI multiplierText))
        {
            multiplierText.SetText($"x{_multiplier}");
            multiplierText.Position = new Vector2f(
                Program.ScreenWidth - (TextureRects["guiBackgroundRight"].Width - 2) * RenderObject.Scale + 8,
                32f
            );
            
            if (Scene.FindByTag(SceneObjectTag.MultiplierBar, out SpriteGUI bar))
            {
                if (_multiplier > 1)
                {
                    bar.Unhide();
                    float percent = _multiplierTimer / _multiplierLifeSpan;
                    bar.SetScale(new Vector2f(80 - 80 * percent, RenderObject.Scale));
                    bar.Position = multiplierText.Position + new Vector2f(multiplierText.Bounds.Width + 8, 6);
                }
                else
                {
                    bar.Hide();
                }
            }
        }
        
        if (Scene.FindByType(out WaveManager? waveManager))
        {
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