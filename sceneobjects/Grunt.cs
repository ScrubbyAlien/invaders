using invaders.enums;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public class Grunt : AbstractEnemy
{
    private float _timeUntilFire;
    private float _fireTimer;

    public Grunt() : base("invaders", TextureRects["grunt1"], Scale)
    {
        maxHealth = 5;
        bulletDamage = 5;
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(Bounds.Width / 2, Bounds.Height);
    protected override float bulletSpeed => 300f;

    protected override void Initialize()
    {
        _timeUntilFire = GetNewFireTime();
        
        animator.SetDefaultSprite(TextureRects["grunt1"]);
        Animation idle = new Animation("idle", true, 3, 0, idleFrames);
        Animation death = new Animation("death", true, 18, deathAnimationLength, deathFrames);
        animator.AddAnimation(idle);
        animator.AddAnimation(death);
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
                Shoot(BulletType.Enemy);
                _fireTimer = 0;
                _timeUntilFire = GetNewFireTime();
            }
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
        animator.PlayAnimation("death", true);
        base.Die();
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

    private Animation.FrameRenderer[] deathFrames =
    {
        (animatable, target) =>
        { // simulates explosion by randomly placing bullet sprites over the enemy rapidly
            animatable.SetTextureRect(TextureRects["grunt1"]);
            target.Draw(animatable.Drawable);
            
            // draw explosion
            Sprite explosion = new Sprite();
            int frameCount = animatable.Animator.FrameCount;
            string rectKey = new Random().Next(2) == 0 ? "enemyBullet" : "enemyExplosion";
            explosion.Texture = AssetManager.LoadTexture("invaders");
            explosion.TextureRect = TextureRects[rectKey];
            explosion.Scale = new Vector2f(Scale, Scale);
            // this function is called every frame so seed needs to be set so fps can be set
            // otherwise it will render something new every frame no matter what fps is
            explosion.Position = animatable.Sprite.Position + new Vector2f(
                (float) new Random((int) animatable.Sprite.Position.X + frameCount).NextDouble() * 
                animatable.Sprite.TextureRect.Width * Scale - 12,
                (float) new Random((int) animatable.Sprite.Position.Y * frameCount).NextDouble() * 
                animatable.Sprite.TextureRect.Height * Scale - 12
                );
            target.Draw(explosion);
        },
    };

}