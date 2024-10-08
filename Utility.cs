using SFML.Graphics;
using SFML.System;

namespace invaders;

public static class Utility
{
    public static bool IntersectsOutDiff(this FloatRect rect1, FloatRect rect2, out Vector2f diff)
    {
        diff = new Vector2f(rect2.Left - rect1.Left, rect2.Top - rect1.Top);
        return rect1.Intersects(rect2);
    }
    
    // Vector2f extension methods borrowed from Collision class from lab project breakout
    public static Vector2f Normalized(this Vector2f v)
    {
        if (v.Length() == 0) return new Vector2f(0, 0);
        return v / v.Length();
        
    }
    
    public static float Length(this Vector2f v) {
        return MathF.Sqrt(v.Dot(v));
    }
    
    public static float Dot(this Vector2f a, Vector2f b) {
        return a.X * b.X + a.Y * b.Y;
    }
    
}

public struct IntersectResult<T>(T entity, Vector2f diff) 
{
    public T entity = entity;
    public Vector2f Diff = diff;
}