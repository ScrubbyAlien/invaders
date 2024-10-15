using SFML.Graphics;
using SFML.System;
using invaders.entity;
using invaders.entity.GUI;
using invaders.enums;

namespace invaders;

// Scene class mostly the same as in lab project 4
public static class Scene
{
    public static int LevelCounter;
    public static bool LoadNextLevel;
    
    public const int MarginSide = 24;
    public const int SpawnInterval = 100;
    public const float AmbientScrollInLevel = 30;
    public const float AmbientScrollInBuffer = 3000;
    public static float AmbientScroll;
    
    private static List<Entity> _entities = new();
    private static List<GUI> _guiElements = new();
    private static List<Entity> _spawnQueue = new();
   
    // background is continuous throughout the game so is not added to _entities;
    private static Background _background;
    private static LevelManager _levelManager;
    
    /// <summary>
    /// Returns a list of all elements in _entities and _guiElements.
    /// Does not preserve their original position in their respective lists.
    /// Do not use this to modify _entities or _guiElements. For that use ForAllEntities().
    /// </summary>
    private static List<Entity> _allEntities {
        get
        {
            List<Entity> all = new();
            all.AddRange(_entities);
            all.AddRange(_guiElements);
            return all;
        }
    }
    
    static Scene()
    {
        AmbientScroll = AmbientScrollInLevel;
        _background = new Background();
        _levelManager = new LevelManager();
        LevelCounter = 0;
        LoadNextLevel = true;
    }
    
    public static void StartLevel(int level)
    {
        // Clear();
        _levelManager.StartLevel(level);

        if (FindByType(out Player? player))
        {
            player?.Reset();
        }
        else
        {
            Player p = new();
            p.Position = new Vector2f(
                (Program.ScreenWidth - p.Bounds.Width) / 2,
                Program.ScreenHeight - 72);
            QueueSpawn(p);
            LoadLevelGUI();
        }
    }

    private static void LoadLevelGUI()
    {  
        QueueSpawn(new HealthGUI());
    }

    public static void NextLevel()
    {
        LevelCounter++;
        LoadNextLevel = true;
    }
    
    public static void QueueSpawn(Entity entity) { _spawnQueue.Add(entity); }
    public static void QueueSpawn(List<Entity> entities) { _spawnQueue.AddRange(entities); }

    private static void ProcessSpawnQueue()
    {
        // spawn all entities in the queue;  
        foreach (Entity entity in _spawnQueue)
        {
            if (entity is GUI gui) _guiElements.Add(gui);
            else _entities.Add(entity);
        }
        _spawnQueue.Clear();

        // initialize all entities and gui elements, gui elements always last
        ForAllEntities((List<Entity> entities, ref int index) =>
        {
            if (!entities[index].Initialized) entities[index].FullInitialize();
        });
    }

    public static void Clear()
    {
        ForAllEntities(
            (List<Entity> entities, ref int index) =>
            {
                if (!entities[index].DontDestroyOnClear)
                {
                    entities[index].Destroy();
                    entities.RemoveAt(index);
                    index--;
                }
            }
        );
    }

    public static void Bury()
    {
        ForAllEntities(
            (List<Entity> entities, ref int index) =>
            {
                if (entities[index].Dead)
                {
                    entities[index].Destroy();
                    entities.RemoveAt(index);
                    index--;
                }
            }
        );
    }
    
    public static void UpdateAll(float deltaTime)
    {
        // these level managing statements should probably be in level manangers update method
        _levelManager.ProgressLevelBuffer(deltaTime);
        
        if (LoadNextLevel)
        {
            StartLevel(LevelCounter);
            LoadNextLevel = false;
        }
        
        _background.Update(deltaTime);
        _levelManager.Update(deltaTime);
        
        
        QueueSpawn(_levelManager.GetNewEntities());
        ProcessSpawnQueue();

        foreach (Entity entity in _allEntities)
        {
            if (!entity.Dead) entity.Update(deltaTime);
        }
        
        EventManager.BroadcastEvents();
    }

    public static void RenderAll(RenderTarget target)
    {
        _background.Render(target);
        _entities.Sort(Entity.CompareByZIndex);
        _guiElements.Sort(Entity.CompareByZIndex);
        foreach (Entity entity in _allEntities)
        {
            if(!entity.Dead) entity.Render(target);
        }
    }
    

    // borrowed from lab project 4
    public static IEnumerable<IntersectResult<T>> FindIntersectingEntities<T>(FloatRect bounds, CollisionLayer layer) where T : Entity
    {
        int lastEntity = _entities.Count - 1;

        for (int i = lastEntity; i >= 0; i--)
        {
            Entity entity = _entities[i];
            if (entity is not T t) continue; 
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
    public static bool FindByType<T>(out T? typed) where T : Entity
    {
        foreach (Entity entity in _entities)
        {
            if (entity is T t)
            {
                typed = t;
                return true;
            }
        }
        typed = null;
        return false;
    }

    /// <summary>
    /// A delegate that takes a list of entities and an index and performs some action on the element at index.
    /// </summary>
    private delegate void EntitiesIteratorAction(List<Entity> entities, ref int index);
    
    /// <summary>
    /// Perform an action on all elements of _entities and _guiElements.
    /// </summary>
    /// <param name="iteratorAction">The function to be called.</param>
    private static void ForAllEntities(EntitiesIteratorAction iteratorAction)
    {
        for (int i = 0; i < _entities.Count; i++)
        {
            iteratorAction(_entities, ref i);
        }
        for (int i = 0; i < _guiElements.Count; i++)
        {
            List<Entity> guiElementsAsEntity = _guiElements.Select(gui => gui as Entity).ToList();
            iteratorAction(guiElementsAsEntity, ref i);
        }
    }
}

