using SFML.Graphics;
using SFML.System;
using invaders.enums;
using invaders.interfaces;
using invaders.sceneobjects;

namespace invaders;

// Scene class mostly the same as in lab project 4
public static class Scene
{
    private static List<SceneObject> _sceneObjects = new();
    private readonly static List<SceneObject> _spawnQueue = new();
    private readonly static List<SceneObject> _destroyQueue = new();
    private readonly static List<DeferredMethodCall> _deferredCalls = new();
    private static string _nextLevel = "";
    private static RenderWindow _window;

    public static void SetWindow(RenderWindow window)
    {
        _window = window;
    }

    public static void CloseWindow()
    {
        _window.Close();
    }
    
    public static void LoadFirstLevel()
    {
        LoadLevel(Settings.StartLevel);
    }
    
    public static void LoadLevel(string levelName)
    {
        _nextLevel = levelName;
    }

    private static void ProcessLoadLevel()
    {
        if (_nextLevel != "")
        {
            Clear();
            List<SceneObject> initialLevelObjects = LevelManager.LoadLevel(_nextLevel);
            QueueSpawn(initialLevelObjects);
            _nextLevel = "";
        }
    }
    
    public static void QueueSpawn(SceneObject o) { _spawnQueue.Add(o); }
    public static void QueueSpawn(List<SceneObject> o) { _spawnQueue.AddRange(o); }
    public static void QueueDestroy(SceneObject o) { _destroyQueue.Add(o); }
    public static void QueueDestroy(List<SceneObject> o) { _destroyQueue.AddRange(o); }
    
    private static void ProcessSpawnQueue()
    {
        _sceneObjects.AddRange(_spawnQueue);
        _spawnQueue.Clear();
        
        _sceneObjects.ForEach( o =>
        {
            if (!o.Initialized) o.FullInitialize();
        });
    }
    private static void ProcessDestroyQueue()
    {
        _destroyQueue.ForEach(o => o.Dead = true);
        _destroyQueue.Clear();
    }
    private static void ProcessDeferredCalls()
    {
        _deferredCalls.ForEach(c => c.Invoke());
        _deferredCalls.Clear();
    }
    private static void UpdateSceneObjects(float deltaTime)
    {
        _sceneObjects.ForEach(o => { if(!o.Dead) o.Update(deltaTime); });
    }
    
    public static void Clear()
    {
        _sceneObjects.ForEach(o =>
        {
            if (!o.DontDestroyOnClear) o.Destroy();
        });
        _sceneObjects = _sceneObjects.Where(o => o.DontDestroyOnClear).ToList();
    }

    public static void Bury()
    {
        _sceneObjects.ForEach(
            _sceneObject =>
            {
                if (_sceneObject.Dead)
                {
                    _sceneObject.Destroy();
                }
            }
        );
        _sceneObjects = _sceneObjects.Where(o => !o.Dead).ToList();
    }
    
    public static void UpdateAll(float deltaTime)
    {
        ProcessLoadLevel();
        ProcessSpawnQueue();
        ProcessDeferredCalls();
        UpdateSceneObjects(deltaTime);
        EventManager.BroadcastEvents();
        ProcessDestroyQueue();
    }

    public static void RenderAll(RenderTarget target)
    {
        List<RenderObject> renderables = _sceneObjects.OfType<RenderObject>().ToList();
        renderables.Sort(RenderObject.CompareByZIndex);
        foreach (RenderObject renderable in renderables)
        {
            if(!renderable.Dead && !renderable.Hidden) renderable.Render(target);
        }
    }
    

    // borrowed from lab project 4
    public static IEnumerable<IntersectResult<T>> FindIntersectingEntities<T>(FloatRect bounds, CollisionLayer layer) where T : RenderObject
    {
        int lastEntity = _sceneObjects.Count - 1;

        for (int i = lastEntity; i >= 0; i--)
        {
            SceneObject sceneObject = _sceneObjects[i];
            if (sceneObject is not RenderObject e) continue;
            if (e is not T t) continue; 
            if (t.Dead) continue;
            if (t.Layer != layer) continue;
            if (t.Bounds.IntersectsOutDiff(bounds, out Vector2f diff))
            {
                yield return new IntersectResult<T>(t, diff);
            }
        }
    }

    /// <summary>
    /// Find the first entity of type T in _entities.
    /// </summary>
    /// <param name="typed">Reference to the found T typed entity if it exists, otherwise null</param>
    /// <typeparam name="T">The type to search for.</typeparam>
    /// <returns>Returns the first entity of type T in _entities</returns>
    public static bool FindByType<T>(out T? typed) where T : SceneObject
    {
        foreach (SceneObject sceneObject in _sceneObjects)
        {
            if (sceneObject is T t)
            {
                typed = t;
                return true;
            }
        }
        typed = null;
        return false;
    }
    
    public static bool FindByTag<T>(SceneObjectTag tag, out T? typed) where T : SceneObject
    {
        foreach (SceneObject sceneObject in _sceneObjects)
        {
            if (sceneObject is T t && sceneObject.Tag == tag)
            {
                typed = t;
                return true;
            }
        }
        typed = null;
        return false;
    }
    
    public static IEnumerable<T> FindAllByType<T>() where T : SceneObject
    {
        foreach (SceneObject sceneObject in _sceneObjects)
        {
            if (sceneObject is T t)
            {
                yield return t;
            }
        }
    }

    /// <summary>
    /// Invokes methodName on instance with parameters at the beginning of the next frame.
    /// </summary>
    /// <param name="instance">The instance on which methodName should be invoked</param>
    /// <param name="methodName">The name of the method that will be invoked. Case sensitive!</param>
    /// <param name="arguments">The arguments that the method will be invoked with</param>
    public static void DeferredCall(Object instance, string methodName, object[] arguments)
    {
        _deferredCalls.Add(new DeferredMethodCall(instance, methodName, arguments));
    }
    
    public static IClickable.ClickedEvent LoadLevelListener(string level)
    {
        return _ => LoadLevel(level);
    }

    public static IClickable.ClickedEvent CloseWindowListener()
    {
        return _ => CloseWindow();
    }
}

