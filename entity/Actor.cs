using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public abstract class Actor : Entity
{
    protected float _maxHealth;
    protected float _currentHealth;
    
    public Actor(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }

    protected virtual Vector2f _bulletOrigin => Position;
    protected virtual float _bulletSpeed => 700f;
    
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

    public virtual void Shoot(Bullet.BulletType type)
    {
        Bullet bullet = new(type, _bulletSpeed);
        bullet.Position = _bulletOrigin;
        Scene.QueueSpawn(bullet);
    }
    
    public virtual void HitByBullet(Bullet bullet) { }

    protected virtual void OnOutsideScreen(
        (ScreenState x, ScreenState y) state, 
        Vector2f outsidePos, 
        out Vector2f adjustedPos)
    { adjustedPos = outsidePos; }
 
    
}