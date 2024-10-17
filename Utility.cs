using invaders.sceneobjects;
using SFML.Graphics;
using SFML.System;
using static SFML.Window.Keyboard;

namespace invaders;

public static class Utility
{
    public static bool IntersectsOutDiff(this FloatRect rect1, FloatRect rect2, out Vector2f diff)
    {
        diff = new Vector2f(rect2.Left - rect1.Left, rect2.Top - rect1.Top);
        return rect1.Intersects(rect2);
    }
    
    public static bool AreAnyKeysPressed(Key[] keys)
    {
        foreach (Key key in keys) if (IsKeyPressed(key)) return true;
        return false;
    }

    public static void ForEach(this IEnumerable<SceneObject> objects, Action<SceneObject> action)
    {
        foreach (SceneObject sceneObject in objects)
        {
            action(sceneObject);
        }
    }
    
    public static Vector2f MiddleOfScreen(FloatRect bounds, Vector2f offset = new())
    {
        return new Vector2f(
            (Program.ScreenWidth - bounds.Width) / 2f,
            (Program.ScreenHeight - bounds.Height) / 2f
        ) + offset;
    }

    public static Vector2f BottomRightOfScreen(FloatRect bounds, Vector2f offset = new())
    {
        return new Vector2f(
            Program.ScreenWidth - bounds.Width,
            Program.ScreenHeight - bounds.Height
        ) + offset;
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

    public static readonly Dictionary<string, IntRect> TextureRects = new()
    {
        { "player", new(73, 19, 14, 12) },
        { "grunt1", new(24, 0, 8, 8) },
        { "grunt2", new(24, 8, 8, 8) },
        { "playerBulletSmall", new (11, 20, 2, 2) },
        { "playerBulletMedium", new (10, 26, 4, 4) },
        { "playerBulletLarge", new (8, 32, 8, 8) },
        { "enemyBulletSmall", new (19, 20, 2, 2) },
        { "enemyBulletMedium", new (18, 26, 4, 4) },
        { "enemyBulletLarge", new (16, 32, 8, 8) },
        { "smallStar", new(2, 54, 1, 1) },
        { "mediumStar", new(1, 57, 3, 3) },
        { "largeStar", new(0, 62, 5, 5) },
        { "largestStar", new(0, 69, 7, 7) },
        { "title", new(72, 40, 48, 24) },
        { "multiplierBar", new(2, 40, 1, 1) },
        { "healthBar", new(0, 40, 8, 2) }
    };
}

public struct IntersectResult<T>(T e, Vector2f diff) 
{
    public T IntersectedEntity = e;
    public Vector2f Diff = diff;
}

public class Animatable(RenderObject i, Drawable d, Animator a)
{
    public readonly RenderObject Instance = i;
    public readonly Drawable Drawable = d;
    public readonly Animator Animator = a;
    public Sprite Sprite
    {
        get
        {
            if (Drawable is Sprite sprite) return sprite;
            else throw new Exception($"animatable of {nameof(Instance)} does not animate Sprite");
        }
    }
    public Text Text 
    {
        get
        {
            if (Drawable is Text text) return text;
            else throw new Exception($"animatable of {nameof(Instance)} does not animate Text");
        }
    }

    public void SetTextureRect(IntRect rect)
    {
        Sprite.TextureRect = rect;
    }
}

// solution for dynamically invoking method on an instance inspired by solution here
// https://stackoverflow.com/questions/6469027/call-methods-using-names-in-c-sharp
public class DeferredMethodCall(Object instance, string methodName, object[] arguments)
{
    private Object _instance = instance;
    private string _methodName = methodName;
    private object[] _arguments = arguments;

    public void Invoke()
    {
        _instance.GetType().GetMethod(_methodName)?.Invoke(_instance, _arguments);
    }
}