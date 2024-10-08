using invaders.interfaces;
using SFML.Graphics;
using SFML.System;
using static SFML.Window.Keyboard;
using static SFML.Window.Keyboard.Key;

namespace invaders.entity;

public class Player : Actor, IAnimatable
{
    private const float Speed = 200f;
    private float _fireRate = 0.5f;
    private float _fireTimer;
    private int _burstLength = 2;
    private int _burstIndex;
    private float _burstRate = 0.1f;
    private float _invicibilityWindow = 0.5f;
    private float _invincibilityTimer;

    private static readonly IntRect PlayerRect = new(73, 19, 14, 12);
    
    
    public Player() : base("invaders", PlayerRect, Scale)
    {
        sprite.Scale = new Vector2f(sprite.Scale.X, -sprite.Scale.Y);
        sprite.Origin = new Vector2f(sprite.Origin.X, 12); // adjust origin after flipping sprite
        maxHealth = 3;
        currentHealth = 3;
        _invincibilityTimer = _invicibilityWindow;
        zIndex = 10;
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(25, 25);

    public override CollisionLayer Layer => CollisionLayer.Player;
    public float AnimationRate => 0.05f;
    private bool IsInvincible => _invincibilityTimer < _invicibilityWindow;
    
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
        
        TryMoveWithinBounds(newPos.Normalized() * Speed * deltaTime, Scene.MarginSide,Scene.MarginSide);

        if (AreAnyKeysPressed([Space]))
        {
            if (_burstIndex == 0 && _fireTimer >= _fireRate)
            {
                Shoot(Bullet.BulletType.Player);
                _burstIndex++;
                _fireTimer = 0;
            } 
            else if (_burstIndex > 0 && _burstIndex < _burstLength && _fireTimer >= _burstRate)
            {
                Shoot(Bullet.BulletType.Player);
                _burstIndex++;
                _fireTimer = 0;
            }
        }

        if (_fireTimer >= _fireRate) _burstIndex = 0;
        if (currentHealth <= 0) WillDie = true;
    }

    public override void HitByBullet(Bullet bullet)
    {
        if (!IsInvincible)
        {
            currentHealth--;
            _invincibilityTimer = 0f;
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

    
    public void Animate()
    {
        if (IsInvincible)
        {
            if (sprite.TextureRect == PlayerRect) sprite.TextureRect = NoSprite;
            else if (sprite.TextureRect == NoSprite) sprite.TextureRect = PlayerRect;
        }
        else
        {
            sprite.TextureRect = PlayerRect;
        }
    }
}