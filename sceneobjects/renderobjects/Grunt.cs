using invaders.enums;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects;

public sealed class Grunt : AbstractEnemy
{
    public override int ScoreValue => 20;
    protected override float powerUpSpawnChance => 0.05f;

    private float _timeUntilFire;
    private float _fireTimer;

    public Grunt() : base("invaders", TextureRects["grunt1"], Scale)
    {
        horizontalSpeed = 45;
        maxHealth = 5;
        bulletDamage = 5;
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(32, 40);
    protected override float bulletSpeed => 300f;

    protected override void Initialize()
    {
        _timeUntilFire = GetNewFireTime();
        
        animator.SetDefaultSprite(TextureRects["grunt1"]);
        Animation idle = new Animation("idle", true, 3, 0, idleFrames);
        animator.AddAnimation(idle);
        animator.PlayAnimation("idle", true);
        
        base.Initialize();
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (!WillDie)
        {
            if (Position.Y > Settings.TopGuiHeight) _fireTimer += deltaTime;
            if (_fireTimer >= _timeUntilFire)
            {
                Shoot("grunt");
                _fireTimer = 0;
                _timeUntilFire = GetNewFireTime();
            }
        }
    }

    private float GetNewFireTime()
    {
        int mod = (int) MathF.Min(touchedBottom, 3);
        return (float)(1 + new Random().NextDouble() * 6 - mod);
    }

    private Animation.FrameRenderer[] idleFrames =
    [
        BasicFrameRenderer(TextureRects["grunt1"]),
        BasicFrameRenderer(TextureRects["grunt2"]),
    ];
}