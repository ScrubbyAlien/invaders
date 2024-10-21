using invaders.enums;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public class Runner : AbstractEnemy
{
    public override int ScoreValue => 150;
    
    private float _timeUntilFire;
    private float _fireTimer;
    private int _burstIndex;
    private int _burstLength = 3;
    
    protected override Vector2f bulletOrigin => Position + new Vector2f(32, 40);
    protected override float bulletSpeed => 500f;
    
    public Runner() : base("invaders", TextureRects["runner1"], Scale)
    {
        horizontalSpeed = 150;
        maxHealth = 10;
        bulletDamage = 3;
    }
    
    protected override void Initialize()
    {
        _timeUntilFire = GetNewFireTime();
        
        animator.SetDefaultSprite(TextureRects["runner1"]);
        Animation idle = new Animation("idle", true, 3, 0, idleFrames);
        Animation death = new Animation("death", true, 18, deathAnimationLength, explosionFrames);
        Animation blink = new Animation("blink", true, 45, 0.2f, blinkFrames);
        animator.AddAnimation(idle);
        animator.AddAnimation(blink);
        animator.AddAnimation(death);
        animator.PlayAnimation("idle", true);
        SetBulletSoundEffect("enemy_shot");
        bulletSoundEffect.Volume = 25;
        
        base.Initialize();
        Console.WriteLine(currentHealth);
    }
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (!WillDie)
        {
            if (Position.Y > Settings.TopGuiHeight) _fireTimer += deltaTime;
            if (_fireTimer >= _timeUntilFire)
            {
                Shoot(BulletType.Runner);
                _burstIndex++;
                if (_burstIndex < _burstLength)
                {
                    _timeUntilFire = 0.2f;
                }
                else
                {
                    _timeUntilFire = GetNewFireTime();
                    _burstIndex = 0;
                }
                _fireTimer = 0;
            }
        }
    }

    private float GetNewFireTime()
    {
        return 1f + new Random().Next(2);
    }

    public override void HitByBullet(Bullet bullet)
    {
        Console.WriteLine(currentHealth);
        TakeDamage(bullet.Damage);
    }
    
    protected override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0) animator.PlayAnimation("blink", true);
        if (currentHealth <= 0) Die();
    }

    protected override void Die()
    {
        animator.PlayAnimation("death", true);
        explosionSound.Play();
        base.Die();
    }

    private Animation.FrameRenderer[] idleFrames =
    [
        BasicFrameRenderer(TextureRects["runner1"]),
        BasicFrameRenderer(TextureRects["runner2"]),
    ];
}