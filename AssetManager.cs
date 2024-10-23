using SFML.Audio;
using SFML.Graphics;

namespace invaders;

public static class AssetManager
{
    private const string AssetPath = "assets";
    private static readonly Dictionary<string, Font> _fonts = new();
    private static readonly Dictionary<string, Texture> _textures = new();
    private static readonly Dictionary<string, SoundBuffer> _soundBuffers = new();
    private static readonly Dictionary<string, Music> _musics = new();

    static AssetManager() {
        // to make more resusable get all file names in AssetPath and add entries dynamically
        _fonts.Add("pixel-font", new Font($"{AssetPath}/pixel-font.ttf"));
        _textures.Add("invaders", new Texture($"{AssetPath}/invaders.png"));

        _soundBuffers.Add("enemy_shot", new SoundBuffer($"{AssetPath}/enemy_shot.wav"));
        _soundBuffers.Add("player_shot", new SoundBuffer($"{AssetPath}/player_shot.wav"));
        _soundBuffers.Add("click1", new SoundBuffer($"{AssetPath}/click1.wav"));
        _soundBuffers.Add("click2", new SoundBuffer($"{AssetPath}/click2.wav"));
        _soundBuffers.Add("explosion", new SoundBuffer($"{AssetPath}/explosion.wav"));
        _soundBuffers.Add("hit_sound", new SoundBuffer($"{AssetPath}/hitsound.wav"));
        _soundBuffers.Add("powerup", new SoundBuffer($"{AssetPath}/powerup.wav"));

        _musics.Add("mainmenu", new Music($"{AssetPath}/mainmenu.wav"));
        _musics.Add("invasion", new Music($"{AssetPath}/invasion.wav"));
        _musics.Add("finale", new Music($"{AssetPath}/finale.wav"));
    }

    public static Texture LoadTexture(string name) => _textures[name];

    public static Font LoadFont(string name) => _fonts[name];

    public static Sound LoadSound(string name) => new(_soundBuffers[name]);

    public static Music OpenMusic(string name) => _musics[name];
}