using invaders.interfaces;
using SFML.Graphics;
using SFML.System;
using static SFML.Window.Keyboard;

namespace invaders.entity;

public class Player : Actor, IAnimatable
{
    protected new const int Scale = 3;
    private const float Speed = 200f;
    private float FireRate = 0.5f;
    private float fireTimer;
    private int burstLength = 2;
    private int burstIndex;
    private float burstRate = 0.1f;
    private float invicibilityWindow = 0.5f;
    private float invincibilityTimer;

    private static readonly IntRect PlayerRect = new(73, 19, 14, 12);
    private static readonly IntRect blank = new(0, 0, 0, 0);
    
    public Player() : base("invaders", PlayerRect, Scale)
    {
        sprite.Scale = new Vector2f(sprite.Scale.X, -sprite.Scale.Y);
        sprite.Origin = new Vector2f(sprite.Origin.X, 12); // adjust origin after flipping sprite
        _maxHealth = 3;
        _currentHealth = 3;
        invincibilityTimer = invicibilityWindow;
    }

    protected override Vector2f _bulletOrigin => Position + new Vector2f(9 * Scale, Bounds.Height);

    public override CollisionLayer Layer => CollisionLayer.Player;
    public float AnimationRate => 0.05f;
    private bool IsInvincible => invincibilityTimer < invicibilityWindow;
    
    public override void Update(float deltaTime)
    {
        fireTimer += deltaTime;
        invincibilityTimer += deltaTime;
        
        Vector2f newPos = new();
        bool right = AreAnyKeysPressed([Key.Right, Key.D]);
        bool left = AreAnyKeysPressed([Key.Left, Key.A]);
        bool up = AreAnyKeysPressed([Key.Up, Key.W]);
        bool down = AreAnyKeysPressed([Key.Down, Key.S]);
        
        if (right) newPos.X = 1;
        if (left) newPos.X = -1;
        if (right && left) newPos.X = 0;
        if (up) newPos.Y = -1;
        if (down) newPos.Y = 1;
        if (up && down) newPos.Y = 0;
        
        TryMoveWithinBounds(Normalized(newPos) * Speed * deltaTime, Scene.MarginSide,Scene.MarginSide);

        if (AreAnyKeysPressed([Key.Space]))
        {
            if (burstIndex == 0 && fireTimer >= FireRate)
            {
                Shoot(Bullet.BulletType.Player);
                burstIndex++;
                fireTimer = 0;
            } 
            else if (burstIndex > 0 && burstIndex < burstLength && fireTimer >= burstRate)
            {
                Shoot(Bullet.BulletType.Player);
                burstIndex++;
                fireTimer = 0;
            }
        }

        if (fireTimer >= FireRate) burstIndex = 0;
        if (_currentHealth <= 0) Dead = true;
    }
    
    
    public override void Init() { }

    public override void Destroy() { }

    public override void HitByBullet(Bullet bullet)
    {
        if (!IsInvincible)
        {
            _currentHealth--;
            invincibilityTimer = 0f;
        }
    }

    protected override void OnOutsideScreen((ScreenState x, ScreenState y) state, Vector2f outsidePos, out Vector2f adjustedPos)
    {
        adjustedPos = outsidePos;
        switch (state.x)
        {
            case ScreenState.OutSideRight:
                adjustedPos.X = Program.ScreenWidth - Bounds.Width - Scene.MarginSide;
                break;
            case ScreenState.OutSideLeft:
                adjustedPos.X = Scene.MarginSide;
                break;
        }

        switch (state.y)
        {
            case ScreenState.OutSideBottom:
                adjustedPos.Y = Program.ScreenHeight - Bounds.Height - Scene.MarginSide;
                break;
            case ScreenState.OutSideTop:
                adjustedPos.Y = Scene.MarginSide;
                break;
        }
    }

    private bool AreAnyKeysPressed(Key[] keys)
    {
        foreach (Key key in keys) if (IsKeyPressed(key)) return true;
        return false;
    }

    private bool AreAllKeysPressed(Key[] keys)
    {
        foreach (Key key in keys) if (!IsKeyPressed(key)) return false;
        return true;
    }

    private Vector2f Normalized(Vector2f v)
    {
        float magnitude = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
        if (magnitude == 0) return new Vector2f(0, 0);
        return new Vector2f(v.X / magnitude, v.Y / magnitude);
    }

    
    public void Animate()
    {
        if (IsInvincible)
        {
            if (sprite.TextureRect == PlayerRect) sprite.TextureRect = blank;
            else if (sprite.TextureRect == blank) sprite.TextureRect = PlayerRect;
        }
        else
        {
            sprite.TextureRect = PlayerRect;
        }
    }
}