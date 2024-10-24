using invaders.enums;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects;

public abstract class Actor : RenderObject
{
    /// <summary>
    /// Position is set to InitPosition when the Actor is Initialized.
    /// Changing this property after the Actor has been Initialized will not change its Position.
    /// </summary>
    public virtual Vector2f InitPosition { get; set; }

    protected int maxHealth;
    protected int currentHealth;
    protected int bulletDamage;
    private Sound _bulletSoundEffect = new();
    protected Sound bulletSoundEffect => _bulletSoundEffect;

    public bool WillDie;
    protected bool inDeathAnimation => timeSinceDeath < deathAnimationLength && WillDie;
    protected float deathAnimationLength = 0f;
    protected float timeSinceDeath;
    protected Sound explosionSound { get; } = AssetManager.LoadSound("explosion");
    protected Sound hitSound { get; } = AssetManager.LoadSound("hit_sound");

    protected Actor(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }

    protected virtual Vector2f bulletOrigin => Position;
    protected virtual float bulletSpeed => 700f;
    public virtual bool IsInvincible => false;

    protected override void Initialize() {
        Position = InitPosition;
        currentHealth = maxHealth;
    }

    public override void Update(float deltaTime) {
        base.Update(deltaTime);
        if (WillDie) timeSinceDeath += deltaTime; // start death timer
        if (WillDie && timeSinceDeath >= deathAnimationLength) Dead = true;
    }

    protected bool TryMoveWithinBounds(Vector2f velocity, int horizontalMargin, int verticalMargin) =>
        TryMoveWithinBounds(velocity, horizontalMargin, horizontalMargin, verticalMargin, verticalMargin);

    protected bool TryMoveWithinBounds(Vector2f velocity, int leftMargin, int rightMargin, int topMargin,
        int bottomMargin) {
        Vector2f newPos = Position + velocity;
        (ScreenState x, ScreenState y) state = (ScreenState.Inside, ScreenState.Inside);

        if (newPos.X >= Program.ScreenWidth - Bounds.Width - rightMargin) {
            state.x = ScreenState.OutSideRight;
        }
        else if (newPos.X <= leftMargin) state.x = ScreenState.OutSideLeft;

        if (newPos.Y >= Program.ScreenHeight - Bounds.Height - bottomMargin) {
            state.y = ScreenState.OutSideBottom;
        }
        else if (newPos.Y <= topMargin) state.y = ScreenState.OutSideTop;

        bool outside = state.x != ScreenState.Inside || state.y != ScreenState.Inside;
        if (outside) {
            OnOutsideScreen(state, newPos, out Vector2f adjusted);
            Position = adjusted;
        }
        else {
            Position = newPos;
        }

        return outside;
    }

    protected virtual void Shoot(string type) {
        Bullet bullet = new(type, bulletSpeed, bulletDamage);
        bullet.Position = bulletOrigin;
        bullet.Spawn();
        bulletSoundEffect.Play();
    }

    public abstract void HitByBullet(Bullet bullet);
    protected abstract void TakeDamage(int damage);

    protected virtual void Die() => WillDie = true;

    protected virtual void OnOutsideScreen(
        (ScreenState x, ScreenState y) state,
        Vector2f outsidePos,
        out Vector2f adjustedPos) =>
        adjustedPos = outsidePos;

    protected void SetBulletSoundEffect(string name) => _bulletSoundEffect = AssetManager.LoadSound(name);

    protected readonly Animation.FrameRenderer[] blinkFrames = [
        (_, _) => { },
        (animatable, target) => {
            animatable.SetTextureRect(animatable.Animator.GetDefaultSprite());
            target.Draw(animatable.Sprite);
        },
    ];

    protected readonly Animation.FrameRenderer[] explosionFrames = {
        (animatable, target) => { // simulates explosion by randomly placing bullet sprites over the actor rapidly
            animatable.SetTextureRect(animatable.Animator.GetDefaultSprite());
            target.Draw(animatable.Drawable);

            // draw explosion
            Sprite explosion = new();
            int frameCount = animatable.Animator.FrameCount;
            string rectKey = new Random().Next(2) == 0 ? "explosionSmall" : "explosionLarge";
            explosion.Texture = AssetManager.LoadTexture("invaders");
            explosion.TextureRect = TextureRects[rectKey];
            explosion.Scale = new Vector2f(Scale, Scale);
            // this function is called every frame so seed needs to be set so fps can be set
            // otherwise it will render something new every frame no matter what fps is
            explosion.Position = animatable.Sprite.Position + new Vector2f(
                new Random((int)animatable.Sprite.Position.X + frameCount).NextSingle() *
                animatable.Sprite.TextureRect.Width * Scale - 12,
                new Random((int)animatable.Sprite.Position.Y * frameCount).NextSingle() *
                animatable.Sprite.TextureRect.Height * Scale - 12
            );
            target.Draw(explosion);
        },
    };
}