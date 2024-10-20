using invaders.enums;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using static SFML.Window.Keyboard.Key;
using static invaders.Utility;
namespace invaders.sceneobjects;

public sealed class Player : Actor
{
    private const float Speed = 200f;
    private float _fireRate = 0.5f;
    private float _fireTimer;
    private int _burstLength = 2;
    private int _burstIndex;
    private float _burstRate = 0.1f;
    private float _invicibilityWindow = 1f;
    private float _invincibilityTimer;
    
    public Player() : base("invaders", TextureRects["player"], Scale)
    {
        maxHealth = 32;
        bulletDamage = 5;
        _invincibilityTimer = _invicibilityWindow;
        deathAnimationLength = 3f;
        zIndex = 10;
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(40, 24);

    public override CollisionLayer Layer => CollisionLayer.Player;
    public override bool IsInvincible => _invincibilityTimer < _invicibilityWindow;

    public int CurrentHealth => currentHealth;

    
    protected override void Initialize()
    {
        base.Initialize();
        animator.SetDefaultSprite(TextureRects["player"]);
        Animation invincible = new Animation("invincible", true, 25, _invicibilityWindow, blinking);
        Animation explode = new Animation("explode", true, 2, deathAnimationLength, explodeFrames);
        animator.AddAnimation(invincible);
        animator.AddAnimation(explode);
        
        Position = new Vector2f(
            (Program.ScreenWidth - Bounds.Width) / 2,
            (Program.ScreenHeight - 50)
        );
        
        SetBulletSoundEffect("player_shot");
        bulletSoundEffect.Volume = 25;
    }

    public override void Destroy()
    {
        Scene.QueueSpawn(new LevelInfo<bool>(CurrentHealth > 0));   
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (!WillDie)
        {
            _fireTimer += deltaTime;
            _invincibilityTimer += deltaTime;
            
            Vector2f newPos = new();
            bool right = AreAnyKeysPressed([Right, D]);
            bool left = AreAnyKeysPressed([Left, A]);
            bool up = AreAnyKeysPressed([Up, W]);
            bool down = AreAnyKeysPressed([Down, S]);
            
            if (right) newPos.X = 1;
            if (left) newPos.X = -1;
            if (right && left) newPos.X = 0;
            if (up) newPos.Y = -1;
            if (down) newPos.Y = 1;
            if (up && down) newPos.Y = 0;

            if (newPos.X > 0) sprite.TextureRect = TextureRects["playerRight"];
            else if (newPos.X < 0) sprite.TextureRect = TextureRects["playerLeft"];
            else sprite.TextureRect = TextureRects["player"];
            
            TryMoveWithinBounds(
                newPos.Normalized() * Speed * deltaTime, 
                Settings.MarginSide,
                Settings.MarginSide,
                Settings.TopGuiHeight + Settings.MarginSide,
                Settings.MarginSide);

            Scene.FindByType(out WaveManager manager);
            bool inTransition = manager.InTransition;

            if (!inTransition)
            {
                if (AreAnyKeysPressed([Space]))
                {
                    if (_burstIndex == 0 && _fireTimer >= _fireRate)
                    {
                        Shoot(BulletType.Player);
                        _burstIndex++;
                        _fireTimer = 0;
                    } 
                    else if (_burstIndex > 0 && _burstIndex < _burstLength && _fireTimer >= _burstRate)
                    {
                        Shoot(BulletType.Player);
                        _burstIndex++;
                        _fireTimer = 0;
                    }
                }

                if (_fireTimer >= _fireRate) _burstIndex = 0;
            }
        }
    }

    public void Reset()
    {
        GlobalEventManager.PublishPlayerChangeHealth(maxHealth - currentHealth);
        currentHealth = maxHealth;
    }

    public override void HitByBullet(Bullet bullet)
    {
        if (!IsInvincible)
        {
            TakeDamage(bullet.Damage);
        }
    }
    
    protected override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        _invincibilityTimer = 0f;
        animator.PlayAnimation("invincible", true);
        if (currentHealth <= 0) Die();
        GlobalEventManager.PublishPlayerHit();
        GlobalEventManager.PublishPlayerChangeHealth(-damage);
    }

    protected override void Die()
    {
        GlobalEventManager.PublishPlayerDeath();
        animator.PlayAnimation("explode", true);
        base.Die();
    }

    protected override void OnOutsideScreen((ScreenState x, ScreenState y) state, Vector2f outsidePos, out Vector2f adjustedPos)
    {
        adjustedPos = outsidePos;
        switch (state.x)
        {
            case ScreenState.OutSideRight:
                adjustedPos.X = Program.ScreenWidth - Bounds.Width - Settings.MarginSide;
                break;
            case ScreenState.OutSideLeft:
                adjustedPos.X = Settings.MarginSide;
                break;
        }

        switch (state.y)
        {
            case ScreenState.OutSideBottom:
                adjustedPos.Y = Program.ScreenHeight - Bounds.Height - Settings.MarginSide;
                break;
            case ScreenState.OutSideTop:
                adjustedPos.Y = Settings.MarginSide + Settings.TopGuiHeight;
                break;
        }
    }

    private Animation.FrameRenderer[] blinking =
    [
        (_, _) => { },
        (animatable, target) =>
        {
            target.Draw(animatable.Sprite);
        },
    ];
    
    private Animation.FrameRenderer[] explodeFrames =
    {
        (animatable, target) =>
        { // simulates explosion by randomly placing bullet sprites over the enemy rapidly
            animatable.SetTextureRect(TextureRects["player"]);
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