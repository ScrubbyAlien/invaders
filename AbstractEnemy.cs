using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace invaders;

public abstract class AbstractEnemy : Actor
{
    protected delegate void RankEvent(AbstractEnemy sender, int rank);
    protected static event RankEvent? RankHitWall;
    protected void InvokeRankHitWall() { RankHitWall?.Invoke(this, Rank); }
    
    
    public int Rank;

    public AbstractEnemy(int rank, string textureName, IntRect initRect, float scale) : 
           base(textureName, initRect, scale)
    {
        Rank = rank;
    }

    protected Dictionary<int, float> _speedByLevel = new()
    {
        {-1, 0f},
        {0, 10f},
        {1, 15f}
    };
    
    public override void Update(float deltaTime)
    {
        Vector2f velocity = new Vector2f(_horizontalSpeed, GetVerticalSpeedByLevel());
        TryMoveWithinBounds(velocity * deltaTime);
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