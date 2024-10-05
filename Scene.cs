using SFML.Graphics;

namespace invaders;

// Scene class mostly the same as in lab project 4
public static class Scene
{
    public static bool LoadNextLevel;
    public static int LevelCounter;

    public const int MarginTop = 50;
    public const int MarginSide = 24;

    public const float MaxEnemySpeed = 30f;
    
    private static List<Entity> _entities = new();
    private static List<List<AbstractEnemy>> enemies = new();

    static Scene()
    {
        LoadNextLevel = true;
        LevelCounter = -1;
    }
    
    public static void LoadLevel()
    {
        SceneLoader.Load(LevelCounter, ref enemies);
    }

    public static void Spawn(Entity entity)
    {
        _entities.Add(entity);
        entity.Init();
        if (entity is IAnimatable animatable)
        {
            Animator.InitAnimatable(animatable);
        }
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
    
    public static void UpdateAll(float deltaTime)
    {
        if (LoadNextLevel)
        {
            LevelCounter++;
            LoadLevel();
            LoadNextLevel = false;
        }
        
        foreach (Entity entity in _entities)
        {
            entity.Update(deltaTime);
        }
        
        EventManager.BroadcastEvents();
        Animator.Animate(deltaTime);
    }

    public static void RenderAll(RenderTarget target)
    {
        foreach (Entity entity in _entities)
        {
            entity.Render(target);
        }
    }
}