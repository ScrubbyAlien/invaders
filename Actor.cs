using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace invaders;

public abstract class Actor : Entity
{
    protected float _horizontalSpeed;
    
    public Actor(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }

    // implement movement functions, children determine specifc movement behaviour

    protected bool TryMoveWithinBounds(Vector2f velocity)
    {
        Vector2f newPos = Position + velocity;
        (ScreenState x, ScreenState y) state = (ScreenState.Inside, ScreenState.Inside);
        
        if (newPos.X >= Program.ScreenWidth - Bounds.Width - Scene.MarginSide) state.x = ScreenState.OutSideRight;
        else if (newPos.X <= Scene.MarginSide) state.x = ScreenState.OutSideLeft;
       
        if (newPos.Y >= Program.ScreenHeight - Bounds.Height) state.y = ScreenState.OutSideBottom;
        else if (newPos.Y <= 0) state.y = ScreenState.OutSideTop;

        bool outside = state.x != ScreenState.Inside || state.y != ScreenState.Inside;
        if (outside)
        {
            OnOutsideScreen(state, newPos, out Vector2f adjusted);
            Position = adjusted;
        }
        else Position = newPos;
        return outside;
    }

    protected virtual void OnOutsideScreen(
        (ScreenState x, ScreenState y) state, 
        Vector2f outsidePos, 
        out Vector2f adjustedPos)
    { adjustedPos = outsidePos; }
 
    
}