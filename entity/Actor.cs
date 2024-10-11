using SFML.Graphics;
using SFML.System;
using invaders.enums;

namespace invaders.entity;

public abstract class Actor : Entity
{
    protected int maxHealth;
    protected int currentHealth;
    protected int bulletDamage;

    public bool WillDie;
    protected bool inDeathAnimation => timeSinceDeath < deathAnimationLength && WillDie;
    protected float deathAnimationLength = 0f;
    protected float timeSinceDeath;
    
    protected static readonly IntRect NoSprite = new(0, 0, 0, 0);

    public Actor(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }

    protected virtual Vector2f bulletOrigin => Position;
    protected virtual float bulletSpeed => 700f;
    public virtual bool IsInvincible => false;

    protected override void Initialize()
    {
        currentHealth = maxHealth;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (WillDie) timeSinceDeath += deltaTime; // start death timer
        if (WillDie && timeSinceDeath >= deathAnimationLength) Dead = true;
    }
    
    protected bool TryMoveWithinBounds(Vector2f velocity, int horizontalMargin, int verticalMargin)
    {
        Vector2f newPos = Position + velocity;
        (ScreenState x, ScreenState y) state = (ScreenState.Inside, ScreenState.Inside);
        
        if (newPos.X >= Program.ScreenWidth - Bounds.Width - horizontalMargin) state.x = ScreenState.OutSideRight;
        else if (newPos.X <= horizontalMargin) state.x = ScreenState.OutSideLeft;
       
        if (newPos.Y >= Program.ScreenHeight - Bounds.Height - verticalMargin) state.y = ScreenState.OutSideBottom;
        else if (newPos.Y <= verticalMargin) state.y = ScreenState.OutSideTop;

        bool outside = state.x != ScreenState.Inside || state.y != ScreenState.Inside;
        if (outside)
        {
            OnOutsideScreen(state, newPos, out Vector2f adjusted);
            Position = adjusted;
        }
        else Position = newPos;
        return outside;
    }

    public virtual void Shoot(BulletType type)
    {
        Bullet bullet = new(type, bulletSpeed, bulletDamage);
        bullet.Position = bulletOrigin;
        Scene.QueueSpawn(bullet);
    }

    public abstract void HitByBullet(Bullet bullet);

    protected virtual void TakeDamage(int damage) {}
    
    protected virtual void Die()
    {
        WillDie = true;
    }
    
    protected virtual void OnOutsideScreen(
        (ScreenState x, ScreenState y) state, 
        Vector2f outsidePos, 
        out Vector2f adjustedPos)
    { adjustedPos = outsidePos; }
}