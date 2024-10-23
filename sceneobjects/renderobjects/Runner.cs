using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects;

public sealed class Runner : AbstractEnemy
{
    public override int ScoreValue => 250;
    protected override float powerUpSpawnChance => 0.6f;

    private float _timeUntilFire;
    private float _fireTimer;
    private int _burstIndex;
    private int _burstLength => 3 + touchedBottom;

    protected override Vector2f bulletOrigin => Position + new Vector2f(32, 40);
    protected override float bulletSpeed => 400f;

    public Runner() : base("invaders", TextureRects["runner1"], Scale) {
        horizontalSpeed = 150;
        maxHealth = 10;
        bulletDamage = 3;
    }

    protected override void Initialize() {
        _timeUntilFire = GetNewFireTime();

        animator.SetDefaultSprite(TextureRects["runner1"]);
        Animation idle = new("idle", true, 3, 0, idleFrames);
        animator.AddAnimation(idle);
        animator.PlayAnimation("idle", true);
        bulletSoundEffect.Volume = 25;

        base.Initialize();
    }

    public override void Update(float deltaTime) {
        base.Update(deltaTime);

        if (WillDie) return;

        if (Position.Y > Settings.TopGuiHeight) _fireTimer += deltaTime;
        if (_fireTimer >= _timeUntilFire) {
            Shoot("runner");
            _burstIndex++;
            if (_burstIndex < _burstLength) {
                _timeUntilFire = 0.2f;
            }
            else {
                _timeUntilFire = GetNewFireTime();
                _burstIndex = 0;
            }

            _fireTimer = 0;
        }
    }

    private static float GetNewFireTime() => 2f + new Random().NextSingle() * 2;

    private readonly Animation.FrameRenderer[] idleFrames = [
        BasicFrameRenderer(TextureRects["runner1"]),
        BasicFrameRenderer(TextureRects["runner2"]),
    ];
}