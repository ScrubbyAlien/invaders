using System.Runtime.Intrinsics.X86;
using SFML.System;
using SFML.Graphics;
using static invaders.Utility;

namespace invaders.entity;

public class Grunt : AbstractEnemy
{
    public new const int SpriteWidth = 8;
    public new const int SpriteHeight = 8;

    private float _timeUntilFire;
    private float _fireTimer;

    public Grunt(int wave) : base(wave, "invaders", TextureRects["grunt1"], Scale)
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
        
        animator.SetDefaultTextureRect(TextureRects["grunt1"]);
        Animation idle = new Animation("idle", true, 3, 0, idleFrames);
        Animation death = new Animation("death", true, 18, deathAnimationLength, deathFrames);
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
        BasicFrameRenderer(TextureRects["grunt1"]),
        BasicFrameRenderer(TextureRects["grunt2"]),
    ];

    private Animation.FrameRenderer[] deathFrames =
    {
        (animatable, target) =>
        { // simulates explosion by randomly placing bullet sprites over the enemy rapidly
            animatable.pSprite.TextureRect = TextureRects["grunt1"];
            target.Draw(animatable.pSprite);
            Sprite explosion = new Sprite();
            int frameCount = animatable.pAnimator.FrameCount;
            string rectKey = new Random().Next(2) == 0 ? "enemyBulletMedium" : "enemyBulletLarge";
            AssetManager.LoadTexture("invaders", TextureRects[rectKey], ref explosion);
            explosion.Scale = new Vector2f(Scale, Scale);
            // this function is called every frame so seed needs to be set so fps can be set
            // otherwise it will render something new every frame no matter what fps is
            explosion.Position = animatable.Position + new Vector2f(
                (float) new Random((int) animatable.Position.X + frameCount).NextDouble() * 
                animatable.pSprite.TextureRect.Width * Scale - 12,
                (float) new Random((int) animatable.Position.Y * frameCount).NextDouble() * 
                animatable.pSprite.TextureRect.Height * Scale - 12
                );
            target.Draw(explosion);
        },
    };

}