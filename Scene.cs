using SFML.Graphics;

namespace invaders;

public class Scene
{
    private string _nextLevel;
    private string _currentLevel;

    public const int MarginTop = 50;
    
    private List<Entity> _entities = new();
    private SceneLoader _loader = new();

    public Scene()
    {
        _nextLevel = "level0";
        _currentLevel = "";
    }
    
    public void LoadLevel()
    {
        _loader.Load(this, _nextLevel);
        _currentLevel = _nextLevel;
        _nextLevel = "";
    }

    public void Spawn(Entity entity)
    {
        _entities.Add(entity);
        entity.Init();
    }

    public void Clear()
    {
        for (int i = _entities.Count - 1; i >= 0; i--) // iterate backwards
        {
            Entity entity = _entities[i];

            if (!entity.DontDestroyOnLoad)
            {
                _entities.RemoveAt(i);
                entity.Destroy(this);    
            }
        }
    }
    
    public void UpdateAll(float deltaTime)
    {
        if (_nextLevel != "") LoadLevel();
        
        foreach (Entity entity in _entities)
        {
            entity.Update(deltaTime);
        }
        
        // broadcast events here
    }

    public void RenderAll(RenderTarget target)
    {
        foreach (Entity entity in _entities)
        {
            entity.Render(target);
        }
    }
}