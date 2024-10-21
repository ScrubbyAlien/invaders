using invaders.enums;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects;

public abstract class Actor : RenderObject
{
    private Vector2f _initPosition;
    public virtual Vector2f InitPosition
    {
        get => _initPosition;
        set => _initPosition = value;
    }

    protected int maxHealth;
    protected int currentHealth;
    protected int bulletDamage;
    private Sound _bulletSoundEffect = new Sound();
    protected Sound bulletSoundEffect => _bulletSoundEffect;

    public bool WillDie;
    protected bool inDeathAnimation => timeSinceDeath < deathAnimationLength && WillDie;
    protected float deathAnimationLength = 0f;
    protected float timeSinceDeath;
    protected Sound explosionSound = AssetManager.LoadSound("explosion");
    protected Sound hitSound = AssetManager.LoadSound("hit_sound");

    protected static readonly IntRect NoSprite = new(0, 0, 0, 0);

    public Actor(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }

    protected virtual Vector2f bulletOrigin => Position;
    protected virtual float bulletSpeed => 700f;
    public virtual bool IsInvincible => false;

    protected override void Initialize()
    {
        Position = InitPosition;
        currentHealth = maxHealth;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (WillDie) timeSinceDeath += deltaTime; // start death timer
        if (WillDie && timeSinceDeath >= deathAnimationLength) Dead = true;
    }
    
    protected bool TryMoveWithinBounds(Vector2f velocity, int horizontalMargin, int verticalMargin)
    {
        return TryMoveWithinBounds(velocity, horizontalMargin, horizontalMargin, verticalMargin, verticalMargin);
    }
    protected bool TryMoveWithinBounds(Vector2f velocity, int leftMargin, int rightMargin, int topMargin,int bottomMargin)
    {
        Vector2f newPos = Position + velocity;
        (ScreenState x, ScreenState y) state = (ScreenState.Inside, ScreenState.Inside);
        
        if (newPos.X >= Program.ScreenWidth - Bounds.Width - rightMargin) state.x = ScreenState.OutSideRight;
        else if (newPos.X <= leftMargin) state.x = ScreenState.OutSideLeft;
       
        if (newPos.Y >= Program.ScreenHeight - Bounds.Height - bottomMargin) state.y = ScreenState.OutSideBottom;
        else if (newPos.Y <= topMargin) state.y = ScreenState.OutSideTop;

        bool outside = state.x != ScreenState.Inside || state.y != ScreenState.Inside;
        if (outside)
        {
            OnOutsideScreen(state, newPos, out Vector2f adjusted);
            Position = adjusted;
        }
        else Position = newPos;
        return outside;
    }

    protected virtual void Shoot(BulletType type)
    {
        Bullet bullet = new(type, bulletSpeed, bulletDamage);
        bullet.Position = bulletOrigin;
        Scene.QueueSpawn(bullet);
        bulletSoundEffect.Play();
    }

    public abstract void HitByBullet(Bullet bullet);

    protected virtual void TakeDamage(int damage) {}
    
    protected virtual void Die()
    {
        WillDie = true;
    }
    
    protected virtual void OnOutsideScreen(
        (ScreenState x, ScreenState y) state, 
        Vector2f outsidePos, 
        out Vector2f adjustedPos)
    { adjustedPos = outsidePos; }

    protected void SetBulletSoundEffect(string name)
    {
        _bulletSoundEffect = AssetManager.LoadSound(name);
    }

    protected Animation.FrameRenderer[] blinkFrames =
    [
        (_, _) => { },
        (animatable, target) =>
        {
            animatable.SetTextureRect(animatable.Animator.GetDefaultSprite());
            target.Draw(animatable.Sprite);
        }
    ];
    
    protected Animation.FrameRenderer[] explosionFrames =
    {
        (animatable, target) =>
        { // simulates explosion by randomly placing bullet sprites over the enemy rapidly
            animatable.SetTextureRect(animatable.Animator.GetDefaultSprite());
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
                new Random((int) animatable.Sprite.Position.X + frameCount).NextSingle() * 
                animatable.Sprite.TextureRect.Width * Scale - 12,
                new Random((int) animatable.Sprite.Position.Y * frameCount).NextSingle() * 
                animatable.Sprite.TextureRect.Height * Scale - 12
            );
            target.Draw(explosion);
        },
    };
}