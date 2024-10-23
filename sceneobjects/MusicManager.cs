using SFML.Audio;

namespace invaders.sceneobjects;

public static class MusicManager
{
    private static Music? _music;
    private static string _name = "";

    public static void PlayMusic(string name) {
        _name = name;
        _music = AssetManager.OpenMusic(name);
        _music.Loop = true;
        _music.Play();
    }

    public static void StopMusic() {
        _name = "";
        _music?.Stop();
        _music = null;
    }

    public static void ChangeMusic(string name) {
        if (name == _name) return;
        _music?.Stop();
        PlayMusic(name);
    }
}