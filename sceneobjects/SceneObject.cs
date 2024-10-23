using invaders.enums;

namespace invaders.sceneobjects;

public abstract class SceneObject
{
    private readonly HashSet<SceneObjectTag> _tags = new();
    public bool Dead = false;
    public bool DontDestroyOnClear = false;
    public bool Initialized { get; private set; }
    private bool _paused;
    public virtual bool Active => !_paused;

    /// <summary>
    /// Any functionality that requires references to other entities should be called from Initialize,
    /// such as FindByType calls, or event handlers/listerners
    /// </summary>
    protected virtual void Initialize() { }

    public void FullInitialize() {
        Initialize();
        _paused = HasTag(SceneObjectTag.PauseMenuItem);
        Initialized = true;
    }

    public virtual void Destroy() { }

    public virtual void Update(float deltaTime) { }

    public void AddTag(SceneObjectTag tag) => _tags.Add(tag);

    public bool HasTag(SceneObjectTag tag) => _tags.Contains(tag);

    public virtual void Pause() => _paused = true;
    public virtual void Unpause() => _paused = false;
}