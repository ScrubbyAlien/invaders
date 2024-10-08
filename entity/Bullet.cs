using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public class Bullet : Entity
{
    public BulletType Type;
    public CollisionLayer EffectiveAgainstLayer; // the collision layer that this bullet should collide with
    private Vector2f _velocity;

    private new const float Scale = 3;
    
    private static Dictionary<BulletType, IntRect[]> bulletTypes = new()
    {
        {BulletType.Player, [
            new IntRect(11, 20, 2,2),
            new IntRect(10, 26, 4, 4),
            new IntRect(8, 32, 8, 8)
        ]},
        { BulletType.Enemy, [
            new IntRect(19, 20, 2,2),
            new IntRect(18, 26, 4, 4),
            new IntRect(16, 32, 8, 8)
        ]}
    };

    public Bullet(BulletType type, float speed) : base("invaders", bulletTypes[type][1], Scale)
    {
        Type = type;
        EffectiveAgainstLayer = Type == BulletType.Player ? CollisionLayer.Enemy : CollisionLayer.Player;
        _velocity = new Vector2f(0, speed * (Type == BulletType.Player ? -1 : 1));
        
        sprite.Origin = new Vector2f(
            bulletTypes[Type][0].Width * Scale / 2f,
            bulletTypes[Type][0].Height * Scale / 2f);
    }

    public override void Update(float deltaTime)
    {
        Position += _velocity * deltaTime;
        if (Position.Y > Program.ScreenHeight + Bounds.Height / 2 || Position.Y < 0 - Bounds.Height / 2)
        {
            Dead = true;
        }
        
        foreach (IntersectResult<Actor> intersect in Scene.FindIntersectingEntities<Actor>(Bounds, EffectiveAgainstLayer))
        {
            if (!intersect.entity.WillDie)
            {
                Dead = true;
                intersect.entity.HitByBullet(this);
            }
        }
    }

    public override void Init() { }

    public override void Destroy() { }
    
    public enum BulletType 
    {
        Player, Enemy
    }
}