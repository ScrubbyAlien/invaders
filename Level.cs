using invaders.sceneobjects;

namespace invaders;

public abstract class Level(string name)
{
    public readonly string Name = name;
    private List<SceneObject> _initialObjects = new();

    public List<SceneObject> GetInitialObjects()
    {
        return _initialObjects;
    }

    public void AddObject(SceneObject o) { _initialObjects.Add(o); }
    public void AddObject(List<SceneObject> o) { _initialObjects.AddRange(o); }
    public void AddObject(SceneObject[] o) { _initialObjects.AddRange(o); }

    public void CreateLevel()
    {
        ClearObjects();
        LoadObjects();
    }
    protected abstract void LoadObjects();
    
    protected void ClearObjects()
    {
        _initialObjects.Clear();
    }

    protected void SetBackgroundMusic(string music = "")
    {
        if (music == "")
        {
            MusicManager.StopMusic();
            return;
        }
        MusicManager.ChangeMusic(music);
    }

    protected void SetBackground()
    {
        if (!Scene.FindByType(out Background? background))
        {
            AddObject(new Background());
        }
        else
        {
            background!.Unpause();
        }
    }
}