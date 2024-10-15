using SFML.Graphics;
using SFML.System;
using invaders.enums;
using invaders.sceneobjects;

namespace invaders;

// Scene class mostly the same as in lab project 4
public static class Scene
{
    private static List<SceneObject> _sceneObjects = new();
    private static List<SceneObject> _spawnQueue = new();

    public static void LoadFirstLevel()
    {
        LoadLevel(Settings.StartLevel);
    }
    
    public static void LoadLevel(string levelName)
    {
        Clear();
        List<SceneObject> initialLevelObjects = LevelManager.LoadLevel(levelName);
        QueueSpawn(initialLevelObjects);
    }
    
    public static void QueueSpawn(SceneObject o) { _spawnQueue.Add(o); }
    public static void QueueSpawn(List<SceneObject> o) { _spawnQueue.AddRange(o); }

    private static void ProcessSpawnQueue()
    {
        _sceneObjects.AddRange(_spawnQueue);
        _spawnQueue.Clear();
        
        _sceneObjects.ForEach( o =>
        {
            if (!o.Initialized) o.FullInitialize();
        });
    }

    public static void Clear()
    {
        _sceneObjects.ForEach(o => o.Destroy());
        _sceneObjects.Clear();
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
        ProcessSpawnQueue();

        foreach (SceneObject sceneObject in _sceneObjects)
        {
            if (!sceneObject.Dead) sceneObject.Update(deltaTime);
        }
        
        EventManager.BroadcastEvents();
    }

    public static void RenderAll(RenderTarget target)
    {
        List<Entity> renderables = _sceneObjects.OfType<Entity>().ToList();
        renderables.Sort(Entity.CompareByZIndex);
        foreach (Entity renderable in renderables)
        {
            if(!renderable.Dead) renderable.Render(target);
        }
    }
    

    // borrowed from lab project 4
    public static IEnumerable<IntersectResult<T>> FindIntersectingEntities<T>(FloatRect bounds, CollisionLayer layer) where T : Entity
    {
        int lastEntity = _sceneObjects.Count - 1;

        for (int i = lastEntity; i >= 0; i--)
        {
            SceneObject sceneObject = _sceneObjects[i];
            if (sceneObject is not Entity e) continue;
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
}

