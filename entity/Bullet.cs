using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public class Bullet : Entity
{
    public BulletType Type;
    public CollisionLayer EffectiveAgainstLayer; // the collision layer that this bullet should collide with
    private Vector2f velocity;

    private new const float Scale = 3;
    
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

    public Bullet(BulletType type, float speed) : base("invaders", bulletTypes[type][0], Scale)
    {
        Type = type;
        EffectiveAgainstLayer = Type switch
        {
            BulletType.Enemy => CollisionLayer.Player,
            BulletType.Player => CollisionLayer.Enemy,
            _ => CollisionLayer.None
        };
        if (Type == BulletType.Player) velocity = new Vector2f(0, -speed);
        else velocity = new Vector2f(0, speed);
        sprite.Origin = new Vector2f(
            bulletTypes[Type][0].Width * Scale / 2f,
            bulletTypes[Type][0].Height * Scale / 2f);
    }

    public override void Update(float deltaTime)
    {
        Position += velocity * deltaTime;
        if (Position.Y > Program.ScreenHeight + Bounds.Height / 2 || Position.Y < 0 - Bounds.Height / 2)
        {
            Dead = true;
        }
        
        foreach (Actor actor in Scene.FindIntersectingEntities<Actor>(Bounds, EffectiveAgainstLayer))
        {
            Dead = true;
            actor.HitByBullet(this);
        }
    }

    public override void Init() { }

    public override void Destroy() { }
    
    public enum BulletType 
    {
        Player, Enemy
    }
}