using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public abstract class Entity
{
    protected Sprite sprite = new();
    protected string textureName;
    public const int SpriteWidth = 0;
    public const int SpriteHeight = 0;
    protected const float Scale = 3;
    public bool Dead = false;
    public bool DontDestroyOnLoad = false;
    public int zIndex;

    public Entity(string textureName, IntRect initRect, float scale) 
    {
        this.textureName = textureName;
        AssetManager.LoadTexture(textureName, initRect, ref sprite);
        sprite.Scale = new Vector2f(scale, scale); // scale is constructor parameter so it can be changed by children
    }
    
    public Vector2f Position
    {
        get => sprite.Position;
        set => sprite.Position = value;
    }

    public virtual FloatRect Bounds => sprite.GetGlobalBounds();

    public virtual CollisionLayer Layer => CollisionLayer.None;
    
    public virtual void Init() {}

    public virtual void Destroy() {}
        
    public virtual void Update(float deltaTime) { }

    public virtual void Render(RenderTarget target)
    {
        target.Draw(sprite);
    }
    
    protected enum ScreenState
    {
        OutSideLeft,
        OutSideRight,
        OutSideTop,
        OutSideBottom,
        Inside
    }

    public static int CompareByZIndex(Entity? e1, Entity? e2)
    {
        if (e1 == null && e2 == null) return 0;
        if (e1 == null) return -1;
        if (e2 == null) return 1;
        return e1.zIndex - e2.zIndex;
    }
}