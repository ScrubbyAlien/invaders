using SFML.Graphics;
using SFML.System;
using invaders.entity;
using invaders.entity.GUI;

namespace invaders;

// Scene class mostly the same as in lab project 4
public static class Scene
{
    public static bool LoadNextLevel;
    public static int LevelCounter;

    public const int MarginSide = 24;
    public const int SpawnInterval = 100;
    
    public static float AmbientScroll = 30;
    
    private static List<Entity> _entities = new();
    private static List<GUI> _guiElements = new();
    private static List<Entity> _spawnQueue = new();
    private static List<(List<char> c, int timer)> _currentLevel = new();
    private static float _waveTimer = -1f;
    private static int _currentWave = -1;
    // background is continuous throughout the game so is not added to _entities;
    private static Background _background;

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
        _background = new Background();
        LoadNextLevel = true;
        LevelCounter = -1;
    }
    
    public static void LoadLevel()
    {
        Clear();
        LevelManager.Load(LevelCounter, ref _currentLevel);
        _currentWave = 0;
        _waveTimer = _currentLevel[_currentWave].timer;
        
        Player player = new();
        player.Position = new Vector2f(
            (Program.ScreenWidth - player.Bounds.Width) / 2,
            Program.ScreenHeight - 72);
        QueueSpawn(player);
        
        LoadLevelGUI();
    }

    private static void LoadLevelGUI()
    {  
        QueueSpawn(new HealthGUI());
    }
    
    public static void QueueSpawn(Entity entity)
    {
        _spawnQueue.Add(entity);
    }

    public static void ProcessSpawnQueue()
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
        _background.Update(deltaTime);
        if (LoadNextLevel)
        {
            LevelCounter++;
            LoadLevel();
            LoadNextLevel = false;
        }
        
        ProcessSpawnQueue();
        
        SpawnNextEnemyWave(deltaTime);

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

    private static void SpawnNextEnemyWave(float deltaTime)
    {
        if (_currentWave >= 0)
        {
            if (_waveTimer >= _currentLevel[_currentWave].timer)
            {
                List<char> enemies = _currentLevel[_currentWave].c;
                while (enemies.Count > 0)
                {
                    int randomIndex = new Random().Next(0, enemies.Count);
                    AbstractEnemy enemy = LevelManager.Constructors[enemies[randomIndex]](_currentWave);
                    enemy.Position = new Vector2f(
                        new Random().Next(MarginSide, Program.ScreenWidth - MarginSide - (int)enemy.Bounds.Width),
                        new Random().Next(-MarginSide - SpawnInterval, -MarginSide));
                    QueueSpawn(enemy);
                    enemies.RemoveAt(randomIndex);
                }

                _currentWave++;
                _waveTimer = 0f;
                
                if (_currentWave == _currentLevel.Count)
                { // reached last wave
                    _currentWave = -1;
                    _waveTimer = -1f;
                }
            }
            else
            {
                _waveTimer += deltaTime;
            }
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

public enum CollisionLayer
{
    Player, Enemy, None
}