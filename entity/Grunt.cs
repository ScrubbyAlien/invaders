using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public class Grunt : AbstractEnemy
{
    public new const int SpriteWidth = 8;
    public new const int SpriteHeight = 8;

    private float _timeUntilFire;
    private float _fireTimer;
    
    // make overridden property of entity
    public static IntRect[] Rects => new IntRect[2]
    {
        new(24, 0, SpriteWidth, SpriteHeight),
        new(24, 8, SpriteWidth, SpriteHeight)
    };

    public Grunt(int wave) : base(wave, "invaders", Rects[0], Scale)
    {
        maxHealth = 5;
        bulletDamage = 5;
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(Bounds.Width / 2, Bounds.Height);
    protected override float bulletSpeed => 300f;

    protected override void Initialize()
    {
        horizontalSpeed = new Random().Next(2) == 0 ? 30f : -30f;
        _timeUntilFire = getNewFireTime();
        
        animator.SetDefaultTextureRect(Rects[0]);
        Animation idle = new Animation("idle", true, 3, 0, idleFrames);
        Animation death = new Animation("death", true, 20, deathAnimationLength, deathFrames);
        animator.AddAnimation(idle);
        animator.AddAnimation(death);
        animator.PlayAnimation("idle", true);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (!WillDie)
        {
            if (Position.Y > 0) _fireTimer += deltaTime;
            if (_fireTimer >= _timeUntilFire)
            {
                Shoot(Bullet.BulletType.Enemy);
                _fireTimer = 0;
                _timeUntilFire = getNewFireTime();
            }
        }
    }

    protected override void OnOutsideScreen((ScreenState x, ScreenState y) state, Vector2f outsidePos, out Vector2f adjustedPos)
    {
        base.OnOutsideScreen(state, outsidePos, out adjustedPos);
        
        switch (state.x)
        {
            case ScreenState.OutSideLeft: 
                adjustedPos.X = Scene.MarginSide;
                Reverse();
                break;
            case ScreenState.OutSideRight: 
                adjustedPos.X = Program.ScreenWidth - Bounds.Width - Scene.MarginSide;
                Reverse();
                break;
        }
    }

    public override void HitByBullet(Bullet bullet)
    {
        TakeDamage(bullet.Damage);
    }

    protected override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    protected override void Die()
    {
        base.Die();
        animator.PlayAnimation("death", true);
    }

    private float getNewFireTime()
    {
        return (float)(1 + new Random().NextDouble() * 6);
    }

    private Animation.FrameRenderer[] idleFrames =
    [
        (animatable, target) =>
        {
            animatable.Sprite.TextureRect = Rects[0];
            target.Draw(animatable.Sprite);
        },
        (animatable, target) =>
        {
            animatable.Sprite.TextureRect = Rects[1];
            target.Draw(animatable.Sprite);
        }
    ];

    private Animation.FrameRenderer[] deathFrames =
    {
        (animatable, target) =>
        {
            animatable.Sprite.TextureRect = NoSprite;
            target.Draw(animatable.Sprite);
        },
        (animatable, target) =>
        {
            animatable.Sprite.TextureRect = Rects[0];
            target.Draw(animatable.Sprite);
        }
    };

}