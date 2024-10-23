using invaders.enums;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;


namespace invaders.sceneobjects.renderobjects;

public class Bullet : RenderObject
{
    public readonly string Type;
    public readonly CollisionLayer EffectiveAgainstLayer; // the collision layer that this bullet should collide with
    private Vector2f _velocity;
    public readonly int Damage;
    private readonly Func<float, float, Vector2f, Vector2f> _movement;
    private float _timeAlive;
    
    private static Dictionary<string, IntRect> bulletTypes = new()
    {
        { "player", TextureRects["playerBullet"] },
        { "grunt", TextureRects["gruntBullet"] },
        { "runner", TextureRects["runnerBullet"] },
        { "squid", TextureRects["squidBullet"] },
        { "juggernautLeft", TextureRects["juggernautBullet1"] },
        { "juggernautRight", TextureRects["juggernautBullet2"] },
    };

    /// <summary>
    /// A bullet that will collide with an Actor (enemies or the player)
    /// </summary>
    /// <param name="type">the type of object that created the bullet, determines the y component of the velocity</param>
    /// <param name="speed">How fast the bullet will travel in pixels per second</param>
    /// <param name="damage">How much damage the bullet will deal to the Actor it collides with</param>
    public Bullet(string type, float speed, int damage) : base("invaders", bulletTypes[type], Scale)
    {
        Type = type;
        EffectiveAgainstLayer = Type == "player" ? CollisionLayer.Enemy : CollisionLayer.Player;
        _velocity = new Vector2f(0, speed * (Type == "player" ? -1 : 1));
        Damage = damage;
        
        sprite.Origin = new Vector2f(
            sprite.GetGlobalBounds().Width / 2f,
            sprite.GetGlobalBounds().Height / 2f);
        _movement = (deltaTime, _, velocity) =>
        {
            return velocity * deltaTime;
        };
    }
    
    /// <summary>
    /// A bullet that will collide with an Actor (enemies or the player)
    /// </summary>
    /// <param name="type">the type of object that created the bullet, determines the y component of the velocity</param>
    /// <param name="speed">How fast the bullet will travel in pixels per second</param>
    /// <param name="damage">How much damage the bullet will deal to the Actor it collides with</param>
    /// <param name="movement">A function that returns a vector2f that will be used to calculate the bullets new position every frame</param>
    public Bullet(string type, float speed, int damage, Func<float, float, Vector2f, Vector2f> movement) : this(type, speed, damage)
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
                    Evaporate();
                    actor.HitByBullet(this);
                }
            }

            // squid and juggernaut bullets can block player bullets so they have enemy collision layer
            // so player bullets actuall collide with them and are destroyed
            if (intersect.IntersectedEntity is Bullet bullet)
            {
                if (bullet.Type == "squid")
                { // juggernaut bullets are too strong to be destroyed
                    bullet.Evaporate();
                }
                this.Evaporate();
            }
        }

    }

    public void ScaleVelocity(float scale)
    {
        _velocity *= scale;
    }

    public void Evaporate()
    {
        Dead = true;
    }
}