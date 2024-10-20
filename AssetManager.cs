using SFML.Audio;
using SFML.Graphics;

namespace invaders;

public static class AssetManager
{
    private const string AssetPath = "assets";
    private static readonly Dictionary<string, Font> _fonts = new();
    private static readonly Dictionary<string, Texture> _textures = new();
    private static readonly Dictionary<string, SoundBuffer> _soundBuffers = new();

    static AssetManager()
    {
        // to make more resusable get all file names in AssetPath and add entries dynamically
        _fonts.Add("pixel-font", new Font($"{AssetPath}/pixel-font.ttf"));
        _textures.Add("invaders", new Texture($"{AssetPath}/invaders.png"));
        _soundBuffers.Add("enemy_shot", new SoundBuffer($"{AssetPath}/enemy_shot.wav"));
        _soundBuffers.Add("player_shot", new SoundBuffer($"{AssetPath}/player_shot.wav"));
    }
    
    public static Texture LoadTexture(string name)
    {
        return _textures[name];
    }

    public static Font LoadFont(string name)
    {
        return _fonts[name];
    }

    public static Sound LoadSound(string name)
    {
        return new Sound(_soundBuffers[name]);
    }
    
}