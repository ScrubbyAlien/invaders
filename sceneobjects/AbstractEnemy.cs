using System.Diagnostics;
using invaders.enums;
using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects;

public abstract class AbstractEnemy : Actor
{
    protected float horizontalSpeed = 30f;
    private WaveManager? _manager;
    
    private Dictionary<int, float> _speedByLevel = new()
    {
        {-1, 30f},
        {0, 20f},
        {1, 25f}
    };

    public AbstractEnemy(string textureName, IntRect initRect, float scale) : 
           base(textureName, initRect, scale)
    {
        _manager = null;
        deathAnimationLength = 0.5f;
    }

    public override CollisionLayer Layer => CollisionLayer.Enemy;

    protected override void Initialize()
    {
        if (Scene.FindByType(out WaveManager? manager))
        {
            _manager = manager;
        }
        
        horizontalSpeed = new Random().Next(2) == 0 ? horizontalSpeed : -horizontalSpeed;
        Position = new Vector2f(
            new Random().Next(Settings.MarginSide, Program.ScreenWidth - Settings.MarginSide - (int) Bounds.Width),
            new Random().Next((int) -Bounds.Height - Settings.SpawnInterval, (int) -Bounds.Height)
        );
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (!WillDie)
        {
            Vector2f velocity = new Vector2f(horizontalSpeed, GetVerticalSpeed());
            TryMoveWithinBounds(velocity * deltaTime, Settings.MarginSide, 0);
            foreach (IntersectResult<AbstractEnemy> intersect in Scene.FindIntersectingEntities<AbstractEnemy>(Bounds, CollisionLayer.Enemy))
            {
                Position += intersect.Diff * deltaTime;
            }
        }
    }
    
    protected void Reverse()
    {
        horizontalSpeed = -horizontalSpeed;
    }
    
    protected override void OnOutsideScreen((ScreenState x, ScreenState y) state, Vector2f outsidePos, out Vector2f adjustedPos)
    {
        adjustedPos = outsidePos;

        if (adjustedPos.Y > Program.ScreenHeight + Bounds.Height)
        {
            adjustedPos.Y = -Bounds.Height;
            // invoke touch bottom event
        }
    }

    protected float GetVerticalSpeed()
    {
        Debug.Assert(_manager != null, nameof(_manager) + " != null");
        if (_speedByLevel.ContainsKey(_manager.CurrentAssault))
        {
            return _speedByLevel[_manager.CurrentAssault];
        }
        else
        {
            return _speedByLevel[-1];
        }
    }
}