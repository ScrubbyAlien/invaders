using invaders.enums;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;


namespace invaders.sceneobjects.renderobjects;

public class Bullet : RenderObject
{
    public BulletType Type;
    public CollisionLayer EffectiveAgainstLayer; // the collision layer that this bullet should collide with
    private Vector2f _velocity;
    public int Damage;
    private Func<float, float, Vector2f, Vector2f> _movement;
    private float _timeAlive;
    
    private static Dictionary<BulletType, IntRect[]> bulletTypes = new()
    {
        { BulletType.Player, [TextureRects["playerBullet"]] },
        { BulletType.Enemy, [TextureRects["enemyBullet"]] },
        { BulletType.Runner, [TextureRects["runnerBullet"]] },
        { BulletType.Squid, [TextureRects["squidBullet"]] },
        
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
        _movement = (deltaTime, _, velocity) =>
        {
            return velocity * deltaTime;
        };
    }

    public Bullet(BulletType type, float speed, int damage, Func<float, float, Vector2f, Vector2f> movement) : this(type, speed, damage)
    {
        _movement = movement;
    }

    public override void Update(float deltaTime)
    {
        _timeAlive += deltaTime;
        
        Position += _movement(deltaTime, _timeAlive, _velocity);
        if (Position.Y > Program.ScreenHeight + Bounds.Height || Position.Y < Settings.TopGuiHeight - Bounds.Height)
        {
            Dead = true;
        }
        
        foreach (IntersectResult<RenderObject> intersect in this.FindIntersectingEntities<RenderObject>(EffectiveAgainstLayer))
        {
            // prevents enemies in death animation or invincible player to eat bullets
            if (intersect.IntersectedEntity is Actor actor)
            {
                if (!actor.WillDie && !actor.IsInvincible)
                {
                    Dead = true;
                    actor.HitByBullet(this);
                }
            }

            if (intersect.IntersectedEntity is Bullet bullet)
            {
                this.Evaporate();
                bullet.Evaporate();
            }
        }

    }

    public void Evaporate()
    {
        Dead = true;
    }
}