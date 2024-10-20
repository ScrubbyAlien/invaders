namespace invaders.sceneobjects;

public sealed class LevelInfo<T> : SceneObject
{
    private T _info;
    
    public LevelInfo(T info)
    {
        _info = info;
        DontDestroyOnClear = true;
    }
    
    public T Extract() { return _info; }

    protected override void Initialize()
    {
        // will be destroyed at the end of the first frame that a new level is loaded
        Scene.LevelLoaded += DestroyOnLevelLoad;
    }

    public override void Destroy()
    {
        Scene.LevelLoaded -= DestroyOnLevelLoad;
    }

    private void DestroyOnLevelLoad()
    {
        Scene.QueueDestroy(this);
    }
    
}