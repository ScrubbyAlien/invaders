using invaders.enums;
using SFML.Graphics;
using SFML.System;
using static SFML.Window.Keyboard.Key;
using static invaders.Utility;
namespace invaders.sceneobjects;

public class Player : Actor
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
        zIndex = 10;
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(10, 6) * Scale;

    public override CollisionLayer Layer => CollisionLayer.Player;
    public override bool IsInvincible => _invincibilityTimer < _invicibilityWindow;

    public int CurrentHealth => currentHealth;

    
    protected override void Initialize()
    {
        base.Initialize();
        animator.SetDefaultSprite(TextureRects["player"]);
        Animation invincible = new Animation("invincible", true, 25, _invicibilityWindow, blinking);
        animator.AddAnimation(invincible);
        
        Position = new Vector2f(
            (Program.ScreenWidth - Bounds.Width) / 2,
            (Program.ScreenHeight - 50)
        );
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
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
        
        TryMoveWithinBounds(newPos.Normalized() * Speed * deltaTime, Settings.MarginSide, Settings.MarginSide);

        Scene.FindByType(out WaveManager? manager);
        bool inTransition = manager!.InTransition;

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

    public void Reset()
    {
        EventManager.PublishPlayerChangeHealth(maxHealth - currentHealth);
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
        EventManager.PublishPlayerHit();
        EventManager.PublishPlayerChangeHealth(-damage);
    }

    protected override void Die()
    {
        EventManager.PublishPlayerDeath();
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
                adjustedPos.Y = Settings.MarginSide;
                break;
        }
    }

    private Animation.FrameRenderer[] blinking =
    [
        BasicFrameRenderer(NoSprite),
        (animatable, target) =>
        {
            target.Draw(animatable.Sprite);
        },
    ];
}