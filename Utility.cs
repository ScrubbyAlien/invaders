using invaders.sceneobjects;
using invaders.sceneobjects.renderobjects;
using SFML.Graphics;
using SFML.System;
using static SFML.Window.Keyboard;

namespace invaders;

public static class Utility
{
    public static bool IntersectsOutDiff(this FloatRect rect1, FloatRect rect2, out Vector2f diff) {
        diff = new Vector2f(rect2.Left - rect1.Left, rect2.Top - rect1.Top);
        return rect1.Intersects(rect2);
    }

    public static bool AreAnyKeysPressed(Key[] keys) {
        foreach (Key key in keys) {
            if (IsKeyPressed(key)) {
                return true;
            }
        }

        return false;
    }

    public static void ForEach(this IEnumerable<SceneObject> objects, Action<SceneObject> action) {
        foreach (SceneObject sceneObject in objects) {
            action(sceneObject);
        }
    }

    public static Vector2f MiddleOfScreen(FloatRect bounds, Vector2f offset = new()) =>
        new Vector2f(
            (Program.ScreenWidth - bounds.Width) / 2f,
            (Program.ScreenHeight - bounds.Height) / 2f
        ) + offset;

    // Vector2f extension methods borrowed from Collision class from lab project breakout
    public static Vector2f Normalized(this Vector2f v) {
        if (v.Length() == 0) return new Vector2f(0, 0);
        return v / v.Length();
    }

    public static float Length(this Vector2f v) => MathF.Sqrt(v.Dot(v));

    public static float Dot(this Vector2f a, Vector2f b) => a.X * b.X + a.Y * b.Y;

    public static readonly Dictionary<string, IntRect> TextureRects = new() {
        { "player", new IntRect(0, 1, 8, 7) },
        { "playerLeft", new IntRect(9, 1, 7, 7) },
        { "playerRight", new IntRect(17, 1, 7, 7) },
        { "healthPower", new IntRect(24, 40, 8, 8) },
        { "triplePower", new IntRect(24, 48, 8, 8) },
        { "speedPower", new IntRect(24, 56, 8, 8) },
        { "grunt1", new IntRect(24, 0, 8, 8) },
        { "grunt2", new IntRect(24, 8, 8, 8) },
        { "runner1", new IntRect(72, 0, 8, 8) },
        { "runner2", new IntRect(72, 8, 8, 8) },
        { "squid1", new IntRect(64, 0, 8, 8) },
        { "squid2", new IntRect(64, 8, 8, 8) },
        { "juggernaut", new IntRect(41, 18, 14, 12) },
        { "playerBullet", new IntRect(8, 26, 4, 4) },
        { "gruntBullet", new IntRect(16, 26, 4, 4) },
        { "runnerBullet", new IntRect(20, 26, 4, 4) },
        { "squidBullet", new IntRect(12, 26, 4, 4) },
        { "juggernautBullet1", new IntRect(41, 34, 6, 6) },
        { "juggernautBullet2", new IntRect(49, 34, 6, 6) },
        { "explosionSmall", new IntRect(16, 26, 4, 4) },
        { "explosionLarge", new IntRect(16, 32, 8, 8) },
        { "smallStar", new IntRect(2, 54, 1, 1) },
        { "mediumStar", new IntRect(1, 57, 3, 3) },
        { "largeStar", new IntRect(0, 62, 5, 5) },
        { "largestStar", new IntRect(0, 69, 7, 7) },
        { "smallStarLong", new IntRect(8, 69, 1, 7) },
        { "mediumStarLong", new IntRect(10, 69, 3, 12) },
        { "largeStarLong", new IntRect(14, 69, 5, 17) },
        { "largestStarLong", new IntRect(20, 69, 7, 27) },
        { "smallStarMiddle", new IntRect(28, 69, 1, 4) },
        { "mediumStarMiddle", new IntRect(31, 69, 3, 8) },
        { "largeStarMiddle", new IntRect(35, 69, 5, 11) },
        { "largestStarMiddle", new IntRect(41, 69, 7, 17) },
        { "title", new IntRect(40, 40, 59, 11) },
        { "multiplierBar", new IntRect(20, 42, 1, 5) },
        { "healthBar", new IntRect(3, 40, 1, 6) },
        { "healthBarEnd", new IntRect(1, 40, 3, 6) },
        { "guiBackgroundLeft", new IntRect(84, 58, 44, 24) },
        { "guiBackgroundRight", new IntRect(84, 81, 44, 24) },
        { "guiBackgroundMiddle", new IntRect(0, 104, 96, 24) },
        { "blackSquare", new IntRect(20, 114, 1, 1) },
        { "whiteSquare", new IntRect(9, 27, 1, 1) },
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

    public Sprite Sprite {
        get {
            if (Drawable is Sprite sprite) return sprite;
            throw new Exception($"animatable of {nameof(Instance)} does not animate Sprite");
        }
    }

    public Text Text {
        get {
            if (Drawable is Text text) return text;
            throw new Exception($"animatable of {nameof(Instance)} does not animate Text");
        }
    }

    public void SetTextureRect(IntRect rect) => Sprite.TextureRect = rect;
}