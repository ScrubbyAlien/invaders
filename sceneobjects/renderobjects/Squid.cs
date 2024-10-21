using invaders.enums;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects;

public sealed class Squid : AbstractEnemy
{
    private float _timeUntilFire;
    private float _fireTimer;
    private float _bulletAmplitude => 500f + touchedBottom * 50;
    private float _movementFrequency = 1f;
    private float _movementPhase;
    private float _movementAmplitude = 100f;
    private float _timeAlive;


    public Squid() : base("invaders", TextureRects["squid1"], Scale)
    {
        horizontalSpeed = 0;
        maxHealth = 17;
        bulletDamage = 6;
        _movementPhase = new Random().Next(2) == 0 ? -MathF.PI / 2 : MathF.PI / 2;
        InitPosition = new Vector2f(
            300 + new Random().NextSingle() * 120,
            new Random().Next((int) -Bounds.Height - Settings.SpawnInterval, (int) -Bounds.Height +  Settings.TopGuiHeight)
        );
    }
    public override int ScoreValue => 500;
    
    protected override Vector2f bulletOrigin => Position + new Vector2f(40, 60);
    protected override float bulletSpeed => 150f;
    
    protected override void Initialize()
    {
        _timeUntilFire = GetNewFireTime();
        
        animator.SetDefaultSprite(TextureRects["squid1"]);
        Animation idle = new Animation("idle", true, 3, 0, idleFrames);
        
        animator.AddAnimation(idle);
        animator.PlayAnimation("idle", true);
        
        base.Initialize();
    }
    
    private float GetNewFireTime()
    {
        return 3f + new Random().NextSingle() * 2;
    }

    public override void Update(float deltaTime)
    {
        _timeAlive += deltaTime;
        base.Update(deltaTime);

        if (!WillDie)
        {
            if (Position.Y > Settings.TopGuiHeight) _fireTimer += deltaTime;
            if (_fireTimer >= _timeUntilFire)
            {
                Shoot("squid");
                _fireTimer = 0;
                _timeUntilFire = GetNewFireTime();
            }
        }
    }

    protected override void Move(float deltaTime)
    {
        if (!WillDie)
        {
            Vector2f velocity = new Vector2f(
                MathF.Sin(_movementFrequency * _timeAlive + _movementPhase) * _movementAmplitude,
                GetVerticalSpeed()
            );
            
            Position += velocity * deltaTime;
            
            if (Position.Y > Program.ScreenHeight)
            {
                Position = new Vector2f(Position.X, 0);
                touchedBottom++;
            }
        }
        
    }

    private Func<float, float, Vector2f, Vector2f> SpecialShoot(bool left)
    {
        return (deltaTime, timeAlive, velocity) =>
        {
            float frequency = 5f + touchedBottom * 0.5f;
            float phase = left ? MathF.PI / 2f : -MathF.PI / 2f;
            float xComponent = MathF.Sin(frequency * timeAlive + phase) * -_bulletAmplitude;
            velocity.X = xComponent;
            return velocity * deltaTime;
        };
    }
    
    protected override void Shoot(string type)
    {
        Bullet leftBullet = new(type, bulletSpeed, bulletDamage, SpecialShoot(true));
        Bullet rightBullet = new(type, bulletSpeed, bulletDamage, SpecialShoot(false));
        leftBullet.Position = bulletOrigin;
        rightBullet.Position = bulletOrigin;
        leftBullet.Layer = CollisionLayer.Enemy;
        rightBullet.Layer = CollisionLayer.Enemy;
        Scene.QueueSpawn(rightBullet);
        Scene.QueueSpawn(leftBullet);
        bulletSoundEffect.Play();
    }

    private Animation.FrameRenderer[] idleFrames =
    [
        BasicFrameRenderer(TextureRects["squid1"]),
        BasicFrameRenderer(TextureRects["squid2"]),
    ];

}