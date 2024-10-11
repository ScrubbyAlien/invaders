using SFML.Graphics;
using SFML.System;
using invaders.enums;
using static invaders.Utility;


namespace invaders.entity;

public class Bullet : Entity
{
    public BulletType Type;
    public CollisionLayer EffectiveAgainstLayer; // the collision layer that this bullet should collide with
    private Vector2f _velocity;
    public int Damage;
    
    private static Dictionary<BulletType, IntRect[]> bulletTypes = new()
    {
        {BulletType.Player, [
            TextureRects["playerBulletSmall"],
            TextureRects["playerBulletMedium"],
            TextureRects["playerBulletLarge"],
        ]},
        { BulletType.Enemy, [
            TextureRects["enemyBulletSmall"],
            TextureRects["enemyBulletMedium"],
            TextureRects["enemyBulletLarge"],
        ]}
    };

    public Bullet(BulletType type, float speed, int damage) : base("invaders", bulletTypes[type][1], Scale)
    {
        Type = type;
        EffectiveAgainstLayer = Type == BulletType.Player ? CollisionLayer.Enemy : CollisionLayer.Player;
        _velocity = new Vector2f(0, speed * (Type == BulletType.Player ? -1 : 1));
        Damage = damage;
        
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
            // prevents enemies in death animation or invincible player to eat bullets
            if (!intersect.IntersectedEntity.WillDie && !intersect.IntersectedEntity.IsInvincible)
            {
                Dead = true;
                intersect.IntersectedEntity.HitByBullet(this);
            }
        }
    }
}