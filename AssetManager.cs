using SFML.Graphics;

namespace invaders;

public static class AssetManager
{
    private const string AssetPath = "assets";
    public static readonly Dictionary<string, Font> _fonts = new();
    public static readonly Dictionary<string, Texture> _textures = new();

    static AssetManager()
    {
        // to make more resusable get all file names in AssetPath and add entries dynamically
        _fonts.Add("pixel-font", new Font($"{AssetPath}/pixel-font.ttf"));
        _textures.Add("invaders", new Texture($"{AssetPath}/invaders.png"));
    }
    
    public static Texture LoadTexture(string name)
    {
        return _textures[name];
    }

    public static Font LoadFont(string name)
    {
        return _fonts[name];
    }
}