using invaders.enums;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;


namespace invaders.sceneobjects;

public class Bullet : RenderObject
{
    public BulletType Type;
    public CollisionLayer EffectiveAgainstLayer; // the collision layer that this bullet should collide with
    private Vector2f _velocity;
    public int Damage;
    
    private static Dictionary<BulletType, IntRect[]> bulletTypes = new()
    {
        { BulletType.Player, [TextureRects["playerBullet"]] },
        { BulletType.Enemy, [TextureRects["enemyBullet"]] },
        { BulletType.Runner, [TextureRects["runnerBullet"]] }
    };

    public Bullet(BulletType type, float speed, int damage) : base("invaders", bulletTypes[type][0], Scale)
    {
        Type = type;
        EffectiveAgainstLayer = Type == BulletType.Player ? CollisionLayer.Enemy : CollisionLayer.Player;
        _velocity = new Vector2f(0, speed * (Type == BulletType.Player ? -1 : 1));
        Damage = damage;
        
        sprite.Origin = new Vector2f(
            sprite.GetGlobalBounds().Width / 2f,
            sprite.GetGlobalBounds().Height / 2f);
    }

    public override void Update(float deltaTime)
    {
        Position += _velocity * deltaTime;
        if (Position.Y > Program.ScreenHeight + Bounds.Height || Position.Y < Settings.TopGuiHeight - Bounds.Height)
        {
            Dead = true;
        }
        
        foreach (IntersectResult<Actor> intersect in this.FindIntersectingEntities<Actor>(EffectiveAgainstLayer))
        {
            // prevents enemies in death animation or invincible player to eat bullets
            if (!intersect.IntersectedEntity.WillDie && !intersect.IntersectedEntity.IsInvincible)
            {
                Dead = true;
                intersect.IntersectedEntity.HitByBullet(this);
            }
        }
    }

    public void Evaporate()
    {
        Dead = true;
    }
}