using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Keyboard;

namespace invaders.entity;

public class Player : Actor
{
    private const float Speed = 200f;
    private const float BulletSpeed = 700f;
    private float FireRate = 0.5f;
    private float fireTimer;
    private int burstLength = 2;
    private int burstIndex;
    private float burstRate = 0.1f;
    
    public Player() : base("invaders", new IntRect(73, 19, 14, 12), 3)
    {
        sprite.Scale = new Vector2f(sprite.Scale.X, -sprite.Scale.Y);
        sprite.Origin = new Vector2f(sprite.Origin.X, 12); // adjust origin after flipping sprite
    }

    private Vector2f bulletOrigin => Position + new Vector2f(Bounds.Width / 2, 10);
    
    public override void Update(float deltaTime)
    {
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

        fireTimer += deltaTime;
        if (AreAnyKeysPressed([Key.Space]))
        {
            if (burstIndex == 0 && fireTimer >= FireRate)
            {
                Shoot();
                burstIndex++;
                fireTimer = 0;
            } 
            else if (burstIndex > 0 && burstIndex < burstLength && fireTimer >= burstRate)
            {
                Shoot();
                burstIndex++;
                fireTimer = 0;
            }
        }

        if (fireTimer >= FireRate) burstIndex = 0;
    }
    
    
    public override void Init() { }

    public override void Destroy() { }

    private void Shoot()
    {
        Bullet bullet = new(Bullet.BulletType.Player, BulletSpeed);
        bullet.Position = bulletOrigin;
        Scene.QueueSpawn(bullet);
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
}