using SFML.Graphics;
using SFML.System;
using invaders.interfaces;

namespace invaders.entity;

public class Grunt : AbstractEnemy, IAnimatable
{
    public new const int SpriteWidth = 8;
    public new const int SpriteHeight = 8;

    private float _timeUntilFire;
    private float _fireTimer;
    

    public static IntRect[] AnimationStages => new IntRect[2]
    {
        new(24, 0, SpriteWidth, SpriteHeight),
        new(24, 8, SpriteWidth, SpriteHeight)
    };
    
    public float AnimationRate => inDeathAnimation ? 0.05f : 0.3f;
    
    private IAnimatable.AnimationStage _animStage = IAnimatable.AnimationStage.Stage1;

    public Grunt(int wave) : base(wave, "invaders", AnimationStages[0], Scale)
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

    public void Animate()
    {
        if (inDeathAnimation)
        {
            if (AnimationStages.Contains(sprite.TextureRect)) sprite.TextureRect = NoSprite;
            else sprite.TextureRect = AnimationStages[1];
            return;
        }
        switch (_animStage)
        {
            case IAnimatable.AnimationStage.Stage1:
                sprite.TextureRect = AnimationStages[1];
                _animStage = IAnimatable.AnimationStage.Stage2;
                break;
            case IAnimatable.AnimationStage.Stage2:
                sprite.TextureRect = AnimationStages[0];
                _animStage = IAnimatable.AnimationStage.Stage1;
                break;
        }
    }

    private float getNewFireTime()
    {
        return (float)(1 + new Random().NextDouble() * 6);
    }
}