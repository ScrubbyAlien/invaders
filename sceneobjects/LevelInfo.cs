namespace invaders.sceneobjects;

public sealed class LevelInfo<T> : SceneObject
{
    private T _info;
    private string _name;
    public string Name => _name;
    
    public LevelInfo(T info, string name)
    {
        _name = name;
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
    
    public static T Catch(string name, T defaulValue)
    {
        Scene.FindByType(out LevelInfo<T>? info);
        if (info == null) return defaulValue;
        if (info.Name != name) return defaulValue;
        return info.Extract();
    }

}