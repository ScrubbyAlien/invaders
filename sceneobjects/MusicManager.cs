using SFML.Audio;

namespace invaders.sceneobjects;

public class MusicManager(string name) : SceneObject
{
    private Music _music = AssetManager.OpenMusic(name);
    private string _name = name;

    protected override void Initialize()
    {
        DontDestroyOnClear = true;
        _music.Loop = true;
        _music.Play();
    }

    public void StopMusic()
    {
        _music.Stop();
        DontDestroyOnClear = false;
    }

    public void ChangeMusic(string name)
    {
        if (name == _name) return;
        _name = name;
        _music.Stop();
        _music = AssetManager.OpenMusic(name);
        _music.Play();
    }
}