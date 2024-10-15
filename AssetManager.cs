using SFML.Graphics;

namespace invaders;

public static class AssetManager
{
    private const string AssetPath = "assets";

    public static Texture LoadTexture(string name)
    {
        return new Texture($"{AssetPath}/{name}.png");
        // sprite.TextureRect = intRect;
    }

    public static Font LoadFont(string name)
    {
        return new Font($"{AssetPath}/{name}.ttf");
    }
}