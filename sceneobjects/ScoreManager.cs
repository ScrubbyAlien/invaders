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
    private TextGUI _scoreText = null!;
    private SpriteGUI _multiplierBar = null!;
    
    
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
        _scoreText.SetText($"x{_multiplier}\n" +
                           $"{_currentScore}");
        _scoreText.Position = BottomRightOfScreen(_scoreText.Bounds, new Vector2f(-24, -24));
        
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
    }


    private void OnEnemyDeath(AbstractEnemy enemy)
    {
        int score = 0;
        if (enemy is Grunt) score = 100;
        score *= _multiplier;
        FadingTextGUI fadingScore = new FadingTextGUI(0.5f, $"{score}");
        fadingScore.Position = enemy.Position + new Vector2f(-2, -16);
        fadingScore.SetDrift(new Vector2f(0, -1).Normalized() * 30);
        Scene.QueueSpawn(fadingScore);
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
    
    
    
}