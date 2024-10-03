using SFML.Graphics;

namespace invaders;

public static class AssetManager
{
    private const string AssetPath = "assets";

    public static void LoadTexture(string name, IntRect intRect, ref Sprite sprite)
    {
        sprite.Texture = new Texture($"{AssetPath}/{name}.png");
        sprite.TextureRect = intRect;
    }

    public static Font LoadFont(string name)
    {
        return new Font($"{AssetPath}/{name}.ttf");
    }

    public static List<string> ReadLevel(string name)
    {
        return File.ReadLines($"{AssetPath}/{name}.txt").ToList();
    }
}