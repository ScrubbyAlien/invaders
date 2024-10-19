namespace invaders.sceneobjects;

public sealed class LevelInfo<T> : SceneObject
{
    private T _info;
    
    public LevelInfo(T info)
    {
        _info = info;
        DontDestroyOnClear = true;
        Scene.QueueSpawn(this);
    }
    
    public T Extract() { return _info; }
    
    public override void Update(float deltaTime) { Scene.QueueDestroy(this); }

    public static void Create<U>(U info)
    {
        LevelInfo<U> i = new LevelInfo<U>(info);
        Scene.QueueSpawn(i);
    }
}