using SFML.Graphics;
using SFML.System;
using invaders.entity;
using invaders.interfaces;

namespace invaders;

// Scene class mostly the same as in lab project 4
public static class Scene
{
    public static bool LoadNextLevel;
    public static int LevelCounter;

    public const int MarginTop = 50;
    public const int MarginSide = 24;
    public const int SpawnInterval = 100;
    public const float AmbientScroll = 30;

    public const float MaxEnemySpeed = 30f;
    
    private static List<Entity> _entities = new();
    private static List<Entity> _spawnQueue = new();
    private static List<(List<char> c, int timer)> _currentLevel = new();
    private static float _waveTimer = -1f;
    private static int _currentWave = -1;

    // background is continuous throughout the game so is not added to _entities;
    private static Background _background;  
    
    static Scene()
    {
        _background = new Background();
        LoadNextLevel = true;
        LevelCounter = -1;
    }
    
    public static void LoadLevel()
    {
        SceneLoader.Load(LevelCounter, ref _currentLevel);
        _currentWave = 0;
        _waveTimer = _currentLevel[_currentWave].timer;
        
        Player player = new();
        player.Position = new Vector2f(
            (Program.ScreenWidth - player.Bounds.Width) / 2,
            Program.ScreenHeight - 72);
        Spawn(player);
    }

    private static void Spawn(Entity entity)
    {
        _entities.Add(entity);
        entity.Init();
        if (entity is IAnimatable animatable)
        {
            Animator.InitAnimatable(animatable);
        }
    }

    public static void QueueSpawn(Entity entity)
    {
        _spawnQueue.Add(entity);
    }

    public static void Clear()
    {
        for (int i = _entities.Count - 1; i >= 0; i--) // iterate backwards
        {
            Entity entity = _entities[i];

            if (!entity.DontDestroyOnLoad)
            {
                _entities.RemoveAt(i);
                entity.Destroy();    
            }
        }
        Animator.ClearAnimatables();
    }

    public static void Clean()
    {
        for (int i = 0; i < _entities.Count();)
        {
            if (_entities[i].Dead)
            {
                _entities[i].Destroy();
                _entities.RemoveAt(i);
                continue;
            }
            i++;
        }
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

        SpawnNextEnemyWave(deltaTime);

        foreach (Entity entity in _spawnQueue)
        {
            Spawn(entity);
        }
        _spawnQueue.Clear();

        foreach (Entity entity in _entities)
        {
            if (!entity.Dead) entity.Update(deltaTime);
        }   

        EventManager.BroadcastEvents();
        Animator.Animate(deltaTime);
    }

    public static void RenderAll(RenderTarget target)
    {
        _background.Render(target);
        _entities.Sort(Entity.CompareByZIndex);
        foreach (Entity entity in _entities)
        {
            if (!entity.Dead) entity.Render(target);
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
                    AbstractEnemy enemy = SceneLoader.Constructors[enemies[randomIndex]](_currentWave);
                    enemy.Position = new Vector2f(
                        new Random().Next(MarginSide, Program.ScreenWidth - MarginSide - (int)enemy.Bounds.Width),
                        new Random().Next(-MarginSide - SpawnInterval, -MarginSide));
                    Spawn(enemy);
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
}

public enum CollisionLayer
{
    Player, Enemy, None
}