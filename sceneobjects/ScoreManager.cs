using invaders.enums;
using invaders.sceneobjects.renderobjects;
using invaders.sceneobjects.renderobjects.gui;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public sealed class ScoreManager : SceneObject
{
    private int _currentScore;
    private int _multiplier = 1;
    private float _multiplierTimer;
    private const float _multiplierLifeSpan = 2f;
    private const float _multiplierBarWidth = 70f;
    private float _passiveScoreTimer;
    private readonly float _passiveScoreInterval;
    private readonly int _passiveScore;

    private readonly TextGUI _scoreText = new("0");
    private readonly TextGUI _multiplierText = new("x1", 8);
    private readonly SpriteGUI _multiplierBar = new(TextureRects["multiplierBar"]);
    private SpriteGUI _middleGui = null!;
    private SpriteGUI _rightGui = null!;

    public ScoreManager(int passiveScore = 0, float interval = 1) {
        _passiveScore = passiveScore;
        _passiveScoreInterval = interval;
    }

    protected override void Initialize() {
        _scoreText.SetZIndex(310);
        _multiplierText.SetZIndex(310);
        _multiplierBar.SetZIndex(310);
        _multiplierBar.SetScale(new Vector2f(_multiplierBarWidth, 5));

        _middleGui = Scene.FindByTag<SpriteGUI>(SceneObjectTag.GuiBackgroundMiddle)!;
        _rightGui = Scene.FindByTag<SpriteGUI>(SceneObjectTag.GuiBackgroundRight)!;

        _scoreText.Position = GetScoreTextPosition();
        _scoreText.Spawn();

        _multiplierText.Position = GetMultiplierTextPosition();
        _multiplierText.Spawn();

        _multiplierBar.Position =
            _multiplierText.Position +
            new Vector2f(_multiplierText.Bounds.Width + 20, 8);
        _multiplierBar.Spawn();

        GlobalEventManager.EnemyDeath += OnEnemyDeath;
        GlobalEventManager.PlayerHit += ResetMultiplier;
    }

    public override void Destroy() => new LevelInfo<int>(_currentScore, "score").Spawn();

    public override void Update(float deltaTime) {
        _multiplierTimer += deltaTime;
        if (_multiplierTimer > _multiplierLifeSpan) {
            ResetMultiplier();
        }

        _scoreText.SetText($"{_currentScore}");
        _scoreText.Position = GetScoreTextPosition();

        _multiplierText.SetText($"x{_multiplier}");
        _multiplierText.Position = GetMultiplierTextPosition();

        if (_multiplier > 1) {
            _multiplierBar.Unhide();
            float percent = _multiplierTimer / _multiplierLifeSpan;
            _multiplierBar.SetScale(new Vector2f(_multiplierBarWidth - _multiplierBarWidth * percent,
                RenderObject.Scale));
        }
        else {
            _multiplierBar.Hide();
        }

        Scene.FindByType(out Invasion? invasion);
        if (_passiveScore > 0 && !invasion!.InTransition) {
            _passiveScoreTimer += deltaTime;
            if (_passiveScoreTimer >= _passiveScoreInterval) {
                _passiveScoreTimer = 0;
                GainScore(_passiveScore);
            }
        }
    }

    private void OnEnemyDeath(AbstractEnemy enemy) {
        int score = enemy.ScoreValue * _multiplier;

        CreateFadingScoreText(
            0.5f,
            $"{score}",
            17 + (uint)MathF.Floor((score - 50) / 50f),
            f => enemy.Position + new Vector2f((enemy.Bounds.Width - f.Bounds.Width) / 2, -enemy.Bounds.Height),
            new Vector2f(0, -1).Normalized() * 30
        );
        GainScore(enemy.ScoreValue);
        IncrementMultiplier();
    }

    private void GainScore(int scoreGained) => _currentScore += scoreGained * _multiplier;

    private void IncrementMultiplier() {
        _multiplier++;
        _multiplier = (int)MathF.Min(_multiplier, 5);
        _multiplierTimer = 0;
    }

    private void ResetMultiplier() {
        _multiplier = 1;
        _multiplierTimer = 0f;
    }

    private Vector2f GetScoreTextPosition() =>
        _middleGui.GetPositionInAvailableArea(new Vector2f(
            (_middleGui.AvailableArea.Width - _scoreText.Bounds.Width) / 2f,
            1.5f // should be 6.5f but if I set it to that the text appears too low for some reason, idk why
        ));

    private Vector2f GetMultiplierTextPosition() =>
        _rightGui.GetPositionInAvailableArea(new Vector2f(6.5f, 6.5f)); // it works here! makes no sense

    private static void CreateFadingScoreText(float fadeTime, string text, uint size,
        Func<FadingTextGUI, Vector2f> positionFunc, Vector2f drift) {
        FadingTextGUI fadingScore = new(fadeTime, text, size, drift);
        fadingScore.Position = positionFunc(fadingScore);
        fadingScore.Spawn();
    }
}