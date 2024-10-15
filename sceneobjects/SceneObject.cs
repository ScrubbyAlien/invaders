namespace invaders.sceneobjects;

public abstract class SceneObject
{
    public bool Dead = false;
    public bool DontDestroyOnClear = false;
    private bool _initialized;
    public bool Initialized => _initialized;
    
    /// <summary>
    /// Any functionality that requires references to other entities should be called from Initialize,
    /// such as FindByType calls, or event handlers/listerners
    /// </summary>
    protected virtual void Initialize() { }

    public void FullInitialize()
    {
        Initialize();
        _initialized = true;
    }

    public virtual void Destroy() {}
    
    public virtual void Update(float deltaTime) { }
    
    
}