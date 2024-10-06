using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public abstract class AbstractEnemy : Actor
{
    public int Wave;
    protected float _horizontalSpeed;
    
    protected Dictionary<int, float> _speedByLevel = new()
    {
        {-1, 0f},
        {0, 20f},
        {1, 25f}
    };

    public AbstractEnemy(int wave, string textureName, IntRect initRect, float scale) : 
           base(textureName, initRect, scale)
    {
        Wave = wave;
    }

    public override CollisionLayer Layer => CollisionLayer.Enemy;
    
    public override void Update(float deltaTime)
    {
        Vector2f velocity = new Vector2f(_horizontalSpeed, GetVerticalSpeedByLevel());
        TryMoveWithinBounds(velocity * deltaTime, Scene.MarginSide, 0);
    }
    
    protected void Reverse()
    {
        _horizontalSpeed = -_horizontalSpeed;
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

    protected float GetVerticalSpeedByLevel()
    {
        if (_speedByLevel.ContainsKey(Scene.LevelCounter))
        {
            return _speedByLevel[Scene.LevelCounter];
        }
        else
        {
            return Scene.MaxEnemySpeed;
        }
    }
}