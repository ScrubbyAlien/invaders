using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public class Bullet : Entity
{
    private BulletType bulletType;
    private Vector2f velocity = new();
    
    private static Dictionary<BulletType, IntRect[]> bulletTypes = new()
    {
        {BulletType.Player, [
            new IntRect(11, 20, 2,2),
            new IntRect(10, 27, 4, 4),
            new IntRect(8, 32, 8, 8)
        ]},
        { BulletType.Enemy, [
            new IntRect(19, 20, 2,2),
            new IntRect(18, 27, 4, 4),
            new IntRect(16, 32, 8, 8)
        ]}
    };

    public Bullet(BulletType type, float speed) : base("invaders", bulletTypes[type][0], 3)
    {
        bulletType = type;
        if (bulletType == BulletType.Player) velocity = new Vector2f(0, -speed);
        else velocity = new Vector2f(0, speed);
        sprite.Origin = new Vector2f(
            bulletTypes[bulletType][0].Width * 3 / 2f,
            bulletTypes[bulletType][0].Height * 3 / 2f);
    }

    public override void Update(float deltaTime)
    {
        Position += velocity * deltaTime;
        if (Position.Y > Program.ScreenHeight + Bounds.Height / 2 || Position.Y < 0 - Bounds.Height / 2)
        {
            Dead = true;
        }   
    }

    public override void Init()
    {
        
    }

    public override void Destroy()
    {
        
    }
    
    public enum BulletType 
    {
        Player, Enemy
    }
}