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

    public void AddInitialObject(SceneObject o) { _initialObjects.Add(o); }
    public void AddInitialObject(List<SceneObject> o) { _initialObjects.AddRange(o); }
    public void AddInitialObject(SceneObject[] o) { _initialObjects.AddRange(o); }

    public abstract void CreateLevel();
}